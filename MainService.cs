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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var bumped = _config.BumpPosition.ToLower() switch
        {
            AppOptions.MajorBump => _versionSettings.BumpMajor(),
            AppOptions.MinorBump => _versionSettings.BumpMinor(),
            AppOptions.PatchBump => _versionSettings.BumpPatch(),
            AppOptions.PreReleaseBump => _versionSettings.BumpPreRelease(),
            _ => throw new Exception()
        };

        Console.WriteLine(bumped.GetText(_config.IsPreReleaseBump));

        _lifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}