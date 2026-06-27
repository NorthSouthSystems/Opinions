namespace NorthSouthSystems.Entities;

[ConventionOptions]
public sealed class RepositoryTrackedCacheVersionProviderOptions
{
    public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMinutes(1);
}

[ConventionSingleton]
public sealed class RepositoryTrackedCacheVersionProviderOptionsValidator
    : AbstractValidator<RepositoryTrackedCacheVersionProviderOptions>
{
    public RepositoryTrackedCacheVersionProviderOptionsValidator()
    {
        RuleFor(x => x.PollingDelay).NotEmpty();
    }
}