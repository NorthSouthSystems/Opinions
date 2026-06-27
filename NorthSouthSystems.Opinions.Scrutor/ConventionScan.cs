using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NorthSouthSystems.Scrutor;

public static class ConventionScanExtensions
{
    public static IServiceCollection ConventionScanAssembly(this IServiceCollection services, Assembly assembly)
    {
        Throw.IfNull(assembly);

        return Throw.IfNull(services)
            .Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(filter => filter.WithAttribute<ConventionSingletonAttribute>(), false)
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                    .AddClasses(filter => filter.WithAttribute<ConventionScopedAttribute>(), false)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                    .AddClasses(filter => filter.WithAttribute<ConventionTransientAttribute>(), false)
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime()
                    .AddClasses(filter => filter.WithAttribute<ConventionOptionsAttribute>(), false)
                    .UsingRegistrationStrategy(new ConventionOptionsRegistrationStrategy()));
    }

    private class ConventionOptionsRegistrationStrategy : RegistrationStrategy
    {
        public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) =>
            AddConventionOptionsMethod.MakeGenericMethod(descriptor.ServiceType)
                .Invoke(null, [services, null, null]);

        private static readonly MethodInfo AddConventionOptionsMethod = typeof(ConventionOptionsExtensions)
            .GetMethod(nameof(ConventionOptionsExtensions.AddConventionOptions), BindingFlags.Static | BindingFlags.Public)!;
    }
}