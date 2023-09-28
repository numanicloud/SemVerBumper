namespace SemVerBumper;

internal sealed class AppOptions
{
    public const string MajorBump = "major";
    public const string MinorBump = "minor";
    public const string PatchBump = "patch";
    public const string PreReleaseBump = "pre";

    public const string SemVerMode = "version";
    public const string IniMode = "ini";

    public string BumpPosition { get; set; } = "";
    public string Mode { get; set; } = "";

    public bool IsPreReleaseBump => BumpPosition.ToLower() == PreReleaseBump;
};