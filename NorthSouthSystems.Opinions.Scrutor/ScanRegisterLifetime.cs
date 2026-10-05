using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NorthSouthSystems.Scrutor;

public sealed class ScanRegisterTransientAttribute() : ScanRegisterLifetimeAttribute(ServiceLifetime.Transient);

public sealed class ScanRegisterScopedAttribute() : ScanRegisterLifetimeAttribute(ServiceLifetime.Scoped);

public sealed class ScanRegisterSingletonAttribute() : ScanRegisterLifetimeAttribute(ServiceLifetime.Singleton);

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public abstract class ScanRegisterLifetimeAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}

// The ServiceLifetime constructor parameter is required to register instances and factories correctly.
internal class ScanRegisterLifetimeStrategy(ServiceLifetime lifetime) : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
    {
        // Do NOT combine Keyed checks into a pattern because patterns don't guarantee order and Keyed* throw.
        if (descriptor.ImplementationInstance is not null
            || (descriptor.IsKeyedService && descriptor.KeyedImplementationInstance is not null))
        {
            services.Add(descriptor);
            return;
        }

        // Do NOT combine Keyed checks into a pattern because patterns don't guarantee order and Keyed* throw.
        if (descriptor.ImplementationFactory is not null
            || (descriptor.IsKeyedService && descriptor.KeyedImplementationFactory is not null))
        {
            services.Add(
                descriptor.IsKeyedService
                    ? new(
                        descriptor.ServiceType,
                        descriptor.ServiceKey,
                        descriptor.KeyedImplementationFactory!,
                        lifetime)
                    : new(descriptor.ServiceType, descriptor.ImplementationFactory!, lifetime));
            return;
        }

        var implementationType = descriptor.ImplementationType ?? descriptor.KeyedImplementationType!;

        var publicConstructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        // Use default DI behavior.
        if (publicConstructors.Length > 0)
        {
            services.Add(new(descriptor.ServiceType, descriptor.ServiceKey, implementationType, lifetime));
            return;
        }

        // Due to limitations of the Microsoft DI container, a factory cannot be built to construct from an open generic
        // ServiceType because the DI container does not provide the generic type parameters to the factory signature.
        if (implementationType.IsGenericTypeDefinition)
        {
            throw new NotSupportedException(
                string.Create(InvariantCulture, $"Type '{implementationType}' is an open generic."));
        }

        var internalConstructors = implementationType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(c => c.IsAssembly)
            .ToImmutableArray();

        switch (internalConstructors.Length)
        {
            case 0:
                throw new InvalidOperationException(
                    string.Create(
                        InvariantCulture,
                        $"Type '{implementationType}' has no public or internal constructor."));
            case > 1:
                throw new InvalidOperationException(
                    string.Create(
                        InvariantCulture,
                        $"Type '{implementationType}' has multiple internal constructors."));
            default:
                services.Add(
                    new(
                        descriptor.ServiceType,
                        descriptor.ServiceKey,
                        (serviceProvider, serviceKey) =>
                            Construct(internalConstructors[0], serviceProvider, serviceKey),
                        lifetime));
                break;
        }
    }

    private static object Construct(ConstructorInfo constructor, IServiceProvider serviceProvider, object? _)
    {
        var arguments = constructor.GetParameters().Select(p => serviceProvider.GetRequiredService(p.ParameterType));

        return constructor.Invoke([.. arguments]);
    }
}
