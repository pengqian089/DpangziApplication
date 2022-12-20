using System.Reflection;
using Microsoft.Extensions.Logging;
using Dpz.Client.Data;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

namespace Dpz.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "logs", ".log"),
                rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            

            var services = builder.Services;

            services.AddMauiBlazorWebView();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Logging.AddSerilog();

            services.AddSingleton(sp => new HttpClient()
            {
                BaseAddress = new Uri("https://api.dpangzi.com")
            });
            
            services.RegisterDefaultServices();

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            
            Log.Information("start application");
            
            return builder.Build();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Host terminated unexpectedly");
            return null;
        }
    }

    private static void RegisterDefaultServices(this IServiceCollection services)
    {
        var allTypes = Assembly.GetExecutingAssembly().GetTypes();
        var serviceTypes = allTypes
            .Where(x => x.Namespace == "Dpz.Client.Data" && x.IsPublic && !x.IsAbstract && !x.IsSealed)
            .ToList();
        foreach (var type in serviceTypes)
        {
            services.AddSingleton(type);
        }
    }
}