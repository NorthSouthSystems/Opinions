using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NorthSouthSystems.Scrutor;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ScanRegisterConventionOptionsAttribute : Attribute;

internal class ScanRegisterConventionOptionsStrategy : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) =>
        AddConventionOptionsMethod
            .MakeGenericMethod(descriptor.ImplementationType ?? descriptor.KeyedImplementationType!)
            .Invoke(null, [services, null, null]);

    private static readonly MethodInfo AddConventionOptionsMethod = typeof(ConventionOptionsExtensions)
        .GetMethod(
            nameof(ConventionOptionsExtensions.AddConventionOptions),
            BindingFlags.Static | BindingFlags.Public)!;
}
