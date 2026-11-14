using VerifyXunit;

public class T_ScriptUpdatedUtcVersionTriggers
{
    [Fact(Skip = "TODO: Create a test SQL Server Database project.")]
    public async Task Generate()
    {
        string generatedScript =
            NorthSouthSystems.Opinions.Entities.ScriptUpdatedUtcVersionTriggers.Generate(null!);

        await Verifier.Verify(generatedScript)
#if !GITHUB_ACTIONS
                .AutoVerify(false, true)
#endif
            ;
    }
}
