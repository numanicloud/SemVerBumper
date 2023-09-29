using Newtonsoft.Json;
using SemVerBumper;

ConsoleApp.Run<Commands>(args);

public class Commands : ConsoleAppBase
{
    public const string MajorBump = "major";
    public const string MinorBump = "minor";
    public const string PatchBump = "patch";
    public const string PreReleaseBump = "pre";

    [RootCommand]
    // ReSharper disable once UnusedMember.Global
    public async Task<int> RunAsync(
        [Option("v", "version file path.")]string versionFile,
        [Option("b", "SemVer position to bump. (major|minor|patch|pre)")]string bumpPosition)
    {
        bumpPosition = bumpPosition.ToLower();

        var version = await LoadVersionAsync(versionFile);
        if (version is null)
        {
            await Console.Error.WriteLineAsync("Invalid version file.");
            return 1;
        }

        var bumped = Bump(bumpPosition, version);
        if (bumped is null)
        {
            await Console.Error.WriteLineAsync("Invalid bumpPosition.");
            return 2;
        }

        Console.WriteLine(bumped.GetText(bumpPosition == PreReleaseBump));
        await SaveVersion(versionFile, bumped);
        return 0;
    }

    private static VersionSettings? Bump(string bumpPosition, VersionSettings version)
    {
        return bumpPosition switch
        {
            MajorBump => version.BumpMajor(),
            MinorBump => version.BumpMinor(),
            PatchBump => version.BumpPatch(),
            PreReleaseBump => version.BumpPreRelease(),
            _ => null
        };
    }

    private static async Task SaveVersion(string versionFile, VersionSettings bumped)
    {
        await using var writer = new StreamWriter(versionFile);
        var json = JsonConvert.SerializeObject(bumped, Formatting.Indented);
        await writer.WriteLineAsync(json);
    }

    private static async Task<VersionSettings?> LoadVersionAsync(string versionFile)
    {
        using var reader = new StreamReader(versionFile);
        return JsonConvert.DeserializeObject<VersionSettings>(await reader.ReadToEndAsync());
    }
}