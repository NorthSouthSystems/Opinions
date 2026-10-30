namespace NorthSouthSystems.Entities;

[ScanRegisterConventionOptions]
public sealed class RepositoryTrackedCacheVersionProviderOptions
{
    public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMinutes(1);
}

[ScanRegisterSingleton]
public sealed class RepositoryTrackedCacheVersionProviderOptionsValidator
    : AbstractValidator<RepositoryTrackedCacheVersionProviderOptions>
{
    public RepositoryTrackedCacheVersionProviderOptionsValidator() { RuleFor(x => x.PollingDelay).NotEmpty(); }
}
