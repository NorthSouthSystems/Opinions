using VerifyXunit;

public class T_VendorFiles
{
    [Fact]
    public async Task DownloadAndMergeAsync()
    {
        string generatedSource = await NorthSouthSystems.Opinions.BCL.VendorFiles.DownloadAndMergeAsync(
            "NorthSouthSystems.Rules",
            "ArgumentExceptionX.cs",
            "ObjectX.cs",
            "RandomX.cs",
            "Throw.cs",
            "TypeX.cs",
            "Buffers/Binary/BinaryRoundTrip.cs",
            "IO/PathX.cs",
            "IO/Hashing/XxHash128X.cs",
            "Linq/ReverseNoBuffer.cs",
            "Reflection/PropertyInfoX.cs");

        await Verifier.Verify(generatedSource)
#if !GITHUB_ACTIONS
                .AutoVerify(false, true)
#endif
            ;
    }
}
