namespace NorthSouthSystems.Entities;

[ConventionOptions]
public sealed class ConventionDbContextOptions
{
    public string? ReadOnlyConnectionString { get; set; }
    public string? ReadWriteConnectionString { get; set; }
}

[ConventionSingleton]
public sealed class ConventionDbContextOptionsValidator : AbstractValidator<ConventionDbContextOptions>
{
    public ConventionDbContextOptionsValidator()
    {
        RuleFor(x => x.ReadOnlyConnectionString).NotEmpty();
        RuleFor(x => x.ReadWriteConnectionString).NotEmpty();
    }
}