namespace NorthSouthSystems.Scrutor;

public sealed class ConventionTransientAttribute : ConventionLifetimeAttribute;
public sealed class ConventionScopedAttribute : ConventionLifetimeAttribute;
public sealed class ConventionSingletonAttribute : ConventionLifetimeAttribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public abstract class ConventionLifetimeAttribute : Attribute;