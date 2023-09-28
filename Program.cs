using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemVerBumper;

await Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddIniFile("version.ini", optional: false, reloadOnChange: false);
        configurationBuilder.AddCommandLine(args, new Dictionary<string, string>()
        {
            { "-b", nameof(AppOptions.BumpPosition) }
        });
    })
    .ConfigureServices((context, collection) =>
    {
        collection.AddHostedService<MainService>();
        collection.Configure<VersionSettings>(context.Configuration);
        collection.Configure<AppOptions>(context.Configuration);
    })
    .RunConsoleAsync(options =>
    {
        options.SuppressStatusMessages = true;
    });