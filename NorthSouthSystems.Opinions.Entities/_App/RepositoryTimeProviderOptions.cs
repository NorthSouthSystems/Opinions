namespace NorthSouthSystems.Entities;

[ScanRegisterConventionOptions]
public sealed class RepositoryTimeProviderOptions
{
    public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMinutes(5);
}

[ScanRegisterSingleton]
public sealed class RepositoryTimeProviderOptionsValidator : AbstractValidator<RepositoryTimeProviderOptions>
{
    public RepositoryTimeProviderOptionsValidator() { RuleFor(x => x.PollingDelay).NotEmpty(); }
}
