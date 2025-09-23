[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

[TestClass]
public static class MSTestSettings
{
    [AssemblyInitialize]
    public static void Init(TestContext context)
    {
        // Kennzeichen für Testumgebung setzen
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_TEST", "true");
    }
}