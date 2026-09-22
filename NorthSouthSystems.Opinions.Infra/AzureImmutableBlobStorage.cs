using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.DependencyInjection;
using NorthSouthSystems.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NorthSouthSystems.Infra;

[ScanRegisterSingleton]
public sealed class AzureImmutableBlobStorage(
    [FromKeyedServices(nameof(AzureImmutableBlobStorage))] BlobContainerClient client,
    CompressionPickler pickler)
    : IImmutableBlobStorage
{
    public Task<ImmutableBlobPickled> ReadAsync(UInt128 xxHash128, CancellationToken cancellationToken) =>
        _readSingleFlight.GetOrCreateAsync(xxHash128, ReadFlightAsync).WaitAsync(cancellationToken);

    private readonly SingleFlight<UInt128, ImmutableBlobPickled> _readSingleFlight = new();

    private async Task<ImmutableBlobPickled> ReadFlightAsync(UInt128 xxHash128, CancellationToken cancellationToken)
    {
        string blobName = GetBlobName(xxHash128);
        var blobClient = Throw.IfNull(client).GetBlobClient(blobName);

        var download = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        var pickled = ImmutableCollectionsMarshal.AsImmutableArray(download.Value.Content.ToArray());
        var bytes = CompressionPickler.Unpickle(pickled);
        var blob = new ImmutableBlob(bytes);

        return xxHash128 == blob.XxHash128
            ? new(blob, pickled)
            : throw new UnreachableException("Hash mismatch.");
    }

    public async Task<ImmutableBlobStorageWriteResult> WriteIfNotExistsAsync(
        ImmutableBlob blob,
        IDictionary<string, string>? tags,
        CancellationToken cancellationToken)
    {
        Throw.IfNull(blob);

        bool wrote;

        string blobName = GetBlobName(blob);
        var blobClient = Throw.IfNull(client).GetBlobClient(blobName);

        var pickled = Throw.IfNull(pickler).Pickle(blob.Bytes);

        try
        {
            _ = await blobClient.UploadAsync(
                    new BinaryData(pickled),
                    new BlobUploadOptions { Conditions = IfNotExistsConditions, Tags = tags },
                    cancellationToken)
                .ConfigureAwait(false);

            wrote = true;
        }
        catch (RequestFailedException ex) when (ConditionNotMet(ex) || BlobAlreadyExists(ex))
        {
            wrote = false;
        }

        return new(wrote, new(blob, pickled));

        // What we expect from Azure Storage in Production.
        static bool ConditionNotMet(RequestFailedException exception) => exception.Status == 412;

        // What we expect from Azurite in Development due to this bug: https://github.com/azure/azurite/issues/2589
        static bool BlobAlreadyExists(RequestFailedException exception) =>
            exception.Status == 409 && exception.ErrorCode == "BlobAlreadyExists";
    }

    private static readonly BlobRequestConditions IfNotExistsConditions = new() { IfNoneMatch = ETag.All };

    // Using the XxHash128 (i.e. entropy) as the Blob's name allows for the most efficient use of Azure Blob storage.
    private static string GetBlobName(ImmutableBlob blob) => GetBlobName(blob.XxHash128);
    private static string GetBlobName(UInt128 xxHash128) => xxHash128.ToBase64UrlBigEndian();
}
