namespace SemVerBumper;

internal sealed record VersionSettings
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public int PreRelease { get; set; }
    public string Suffix { get; set; } = "";

    public string GetText(bool isPreReleaseBump) => isPreReleaseBump
        ? $"{Major}.{Minor}.{Patch}-{Suffix}{PreRelease}"
        : $"{Major}.{Minor}.{Patch}";


    public VersionSettings BumpMajor()
    {
        return this with
        {
            Major = Major + 1,
            Minor = 0,
            Patch = 0,
            PreRelease = 0
        };
    }

    public VersionSettings BumpMinor()
    {
        return this with
        {
            Minor = Minor + 1,
            Patch = 0,
            PreRelease = 0
        };
    }

    public VersionSettings BumpPatch()
    {
        return this with
        {
            Patch = Patch + 1,
            PreRelease = 0
        };
    }

    public VersionSettings BumpPreRelease()
    {
        return this with
        {
            Patch = PreRelease == 0
                ? Patch + 1
                : Patch,
            PreRelease = PreRelease + 1
        };
    }
}