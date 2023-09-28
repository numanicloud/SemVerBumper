#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace SemVerBumper;

internal sealed class MainService : IHostedService
{
    private readonly AppOptions _config;
    private readonly VersionSettings _versionSettings;
    private readonly IHostApplicationLifetime _lifetime;

    public MainService(
        IOptions<VersionSettings> version,
        IOptions<AppOptions> options,
        IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime;
        _versionSettings = version.Value;
        _config = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            Run();
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void Run()
    {
        var bumped = _config.BumpPosition.ToLower() switch
        {
            AppOptions.MajorBump => _versionSettings.BumpMajor(),
            AppOptions.MinorBump => _versionSettings.BumpMinor(),
            AppOptions.PatchBump => _versionSettings.BumpPatch(),
            AppOptions.PreReleaseBump => _versionSettings.BumpPreRelease(),
            _ => null
        };

        if (bumped is null)
        {
            Console.Error.WriteLine(GetHelp());
            return;
        }

        switch (_config.Mode)
        {
        case AppOptions.SemVerMode:
            Console.WriteLine(bumped.GetText(_config.IsPreReleaseBump));
            break;

        case AppOptions.IniMode:
            Console.WriteLine($"""
                    { nameof(VersionSettings.Major)} ={ bumped.Major} 
                    { nameof(VersionSettings.Minor)} ={ bumped.Minor} 
                    { nameof(VersionSettings.Patch)} ={ bumped.Patch} 
                    { nameof(VersionSettings.PreRelease)} ={ bumped.PreRelease} 
                    { nameof(VersionSettings.Suffix)} ={ bumped.Suffix} 
                    """ );
            break;

        default:
            Console.Error.WriteLine(GetHelp());
            break;
        }
    }

    private string GetHelp()
    {
        var bumpOptions = $"{AppOptions.MajorBump}|{AppOptions.MinorBump}|{AppOptions.PatchBump}|{AppOptions.PreReleaseBump}";
        var modeOptions = $"{AppOptions.SemVerMode}|{AppOptions.IniMode}";
        return $"""
            Usage: SemVerBumper.exe -b|--BumpPosition {bumpOptions} -m|--Mode {modeOptions}
            """  ;
    }
}