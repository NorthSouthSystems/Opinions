using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace NorthSouthSystems.Scrutor;

public interface IScanRegisterCustom
{
    static abstract void Register(IServiceCollection services);
}

internal class ScanRegisterCustomStrategy : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
    {
        var implementationType = descriptor?.ImplementationType;

        if (implementationType is null)
            throw new UnreachableException(nameof(descriptor));

        if (implementationType.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException(
                string.Create(InvariantCulture, $"Type '{implementationType}' is an open generic."));
        }

        RegisterMethod.MakeGenericMethod(implementationType).Invoke(null, [services]);
    }

    private static readonly MethodInfo RegisterMethod = typeof(ScanRegisterCustomStrategy)
        .GetMethod(nameof(Register), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static void Register<T>(IServiceCollection services)
        where T : IScanRegisterCustom =>
        T.Register(services);
}
