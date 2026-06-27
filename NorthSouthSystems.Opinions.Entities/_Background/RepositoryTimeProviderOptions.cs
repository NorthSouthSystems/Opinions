namespace NorthSouthSystems.Entities;

[ConventionOptions]
public sealed class RepositoryTimeProviderOptions
{
    public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMinutes(5);
}

[ConventionSingleton]
public sealed class RepositoryTimeProviderOptionsValidator : AbstractValidator<RepositoryTimeProviderOptions>
{
    public RepositoryTimeProviderOptionsValidator()
    {
        RuleFor(x => x.PollingDelay).NotEmpty();
    }
}