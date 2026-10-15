using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace NorthSouthSystems.Entities;

// This class is a nice-to-have precaution against client code using the GlobalBootstrapper TenantUser outside of Tests
// to SaveChanges. There is no valid use case for doing so. Unfortunately, this interceptor only runs for SaveChanges
// and not for sprocs. To add this precaution to sprocs, an application can do so in its implementation of
// ITrackCreatedUpdatedByProvider. Unfortunately, that will not protect sprocs that do not take any @*By parameters.
[ScanRegisterScoped]
internal sealed class GlobalBootstrapperInterceptor(
    IHostEnvironment environment,
    ITrackCreatedUpdatedByProvider byProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ThrowIfInvalid();

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ThrowIfInvalid();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ThrowIfInvalid()
    {
        if (!environment.IsEnvironment("Tests") && byProvider.CurrentId == TenantUserId.GlobalBootstrapper)
            throw new InvalidOperationException(nameof(TenantUserId.GlobalBootstrapper) + " is only valid for Tests.");
    }
}
