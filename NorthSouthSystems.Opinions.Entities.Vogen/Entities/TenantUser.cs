using Vogen;

namespace NorthSouthSystems.Entities;

[ValueObject<int>]
public readonly partial struct TenantUserId : IIntValueObject<TenantUserId>
{
    public static TenantUserId GlobalBootstrapper { get; } = new(1);

    private static Validation Validate(int id) => id > 0 ? Validation.Ok : Validation.Invalid();

    private static int NormalizeInput(int input) => input; // No-op
}

[ValueObject<Guid>]
public readonly partial struct TenantUserIdForPublic : IGuidValueObject<TenantUserIdForPublic>
{
    public static TenantUserIdForPublic GlobalBootstrapper { get; } =
        From(Guid.Parse("11111111-1111-1111-1111-111111111111", InvariantCulture));

    private static Validation Validate(Guid id) => id != Guid.Empty ? Validation.Ok : Validation.Invalid();

    private static Guid NormalizeInput(Guid input) => input; // No-op
}