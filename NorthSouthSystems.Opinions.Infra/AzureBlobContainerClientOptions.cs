using System.Text.RegularExpressions;

namespace NorthSouthSystems.Infra;

[ScanRegisterConventionOptions]
public sealed partial class AzureBlobContainerClientOptions
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }

    public string? ConnectionStringNoContainerName
    {
        get
        {
            var match = ConnectionStringContainerNameRegex().Match(ConnectionString ?? string.Empty);

            if (!match.Success)
                return ConnectionString;

            string name = match.Groups["name"].Value;

            return string.Equals(name, ContainerName, StringComparison.OrdinalIgnoreCase)
                ? ConnectionStringContainerNameRegex().Replace(ConnectionString!, _ => string.Empty)
                : throw new InvalidOperationException("ConnectionString ContainerName mismatch.");
        }
    }

    [GeneratedRegex(@";ContainerName=(?<name>[^;]*)")]
    private static partial Regex ConnectionStringContainerNameRegex();
}

[ScanRegisterSingleton]
public sealed class AzureBlobContainerClientOptionsValidator : AbstractValidator<AzureBlobContainerClientOptions>
{
    public AzureBlobContainerClientOptionsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
        RuleFor(x => x.ContainerName).NotEmpty();
    }
}
