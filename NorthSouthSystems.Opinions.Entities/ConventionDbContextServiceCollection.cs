using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NorthSouthSystems.Entities;

public static class ConventionDbContextServiceCollectionExtensions
{
    public static IServiceCollection AddConventionDbContext
        <TDbContext, TAtomicCommandRepository, TReadOnlyRepository, TUnitOfWorkRepository>(
            this IServiceCollection services, string name)
        where TDbContext : ConventionDbContext<TDbContext>, TAtomicCommandRepository, TReadOnlyRepository, TUnitOfWorkRepository
        where TAtomicCommandRepository : class, IAtomicCommandRepository
        where TReadOnlyRepository : class, IReadOnlyRepository
        where TUnitOfWorkRepository : class, IUnitOfWorkRepository
    {
        Throw.IfNullOrWhiteSpace(name);

        services.AddScoped<TrackCreatedUpdatedInterceptor>();

        services.AddTransient<IReadOnlyRepository>(ReadOnlyFactory);
        services.AddTransient(ReadOnlyFactory);

        services.AddTransient<IAtomicCommandRepository>(AtomicCommandFactory);
        services.AddTransient(AtomicCommandFactory);

        services.AddTransient<IUnitOfWorkRepository>(UnitOfWorkFactory);
        services.AddTransient(UnitOfWorkFactory);

        return services;

        TReadOnlyRepository ReadOnlyFactory(IServiceProvider sp)
        {
            // We keep the default Entity Tracking behavior to allow Entity Framework to patch navigation properties
            // after manually LoadAsync'ing several sets of Entities of different Types.
            var builder = new DbContextOptionsBuilder<TDbContext>()
                .UseSqlServer(
                    GetOptions(sp, name).ReadOnlyConnectionString,
                    sql =>
                    {
                        sql.EnableRetryOnFailure();
                        sql.CommandTimeout(30);
                    })
                .AddInterceptors(ReadOnlyInterceptor.Singleton);

            return ActivatorUtilities.CreateInstance<TDbContext>(sp, builder.Options);
        }

        TAtomicCommandRepository AtomicCommandFactory(IServiceProvider sp)
        {
            // By definition, every action against this interface should be atomic with no calls to SaveChanges;
            // therefore, we can use NoTracking and the ReadOnlyInterceptor.
            var builder = new DbContextOptionsBuilder<TDbContext>()
                .UseSqlServer(
                    GetOptions(sp, name).ReadWriteConnectionString,
                    sql =>
                    {
                        sql.EnableRetryOnFailure();
                        sql.CommandTimeout(60);
                    })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(ReadOnlyInterceptor.Singleton);

            return ActivatorUtilities.CreateInstance<TDbContext>(sp, builder.Options);
        }

        TUnitOfWorkRepository UnitOfWorkFactory(IServiceProvider sp)
        {
            var builder = new DbContextOptionsBuilder<TDbContext>()
                .UseSqlServer(
                    GetOptions(sp, name).ReadWriteConnectionString,
                    sql =>
                    {
                        sql.EnableRetryOnFailure();
                        sql.CommandTimeout(60);
                    })
                .AddInterceptors(
                    // Update ConventionDbContext.DryRunChanges._interceptors as Interceptors are added or removed.
                    WhiteSpaceToNullInterceptor.Singleton,
                    sp.GetRequiredService<TrackCreatedUpdatedInterceptor>(),
                    SaveChangesPermissionsInterceptor.Singleton);

            return ActivatorUtilities.CreateInstance<TDbContext>(sp, builder.Options);
        }
    }

    private static ConventionDbContextOptions GetOptions(IServiceProvider serviceProvider, string name) =>
        serviceProvider.GetRequiredService<IOptionsSnapshot<ConventionDbContextOptions>>()
            .Get(name);
}