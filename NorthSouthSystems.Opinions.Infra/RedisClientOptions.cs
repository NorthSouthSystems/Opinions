namespace NorthSouthSystems.Infra;

[ScanRegisterConventionOptions]
public sealed class RedisClientOptions
{
    public string? ConfigurationString { get; set; }
    public int Database { get; set; } = -1; // Default value in StackExchange.Redis.ConnectionMultiplexer.GetDatabase.
}

[ScanRegisterSingleton]
public sealed class RedisClientOptionsValidator : AbstractValidator<RedisClientOptions>
{
    public RedisClientOptionsValidator()
    {
        RuleFor(x => x.ConfigurationString).NotEmpty();
        RuleFor(x => x.Database).InclusiveBetween(-1, 15);
    }
}
