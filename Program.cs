
using CliFx;
using Microsoft.Extensions.DependencyInjection;
using tricep.cli.services;

namespace tricep.cli;

public class Program
{
    public static async Task<int> Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MonoRepoFileService>();

        services.AddSingleton<InitCommand>();
        services.AddSingleton<NewModuleCommand>();
        services.AddSingleton<NewLiveCommand>();
        services.AddSingleton<NewTemplateCommand>();
        services.AddSingleton<RefCommand>();


        services.AddSingleton<NameValidator>();
        services.AddSingleton<FilenameValidator>();

        var provider = services.BuildServiceProvider();

        try
        {
            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(provider.GetService)
                .Build()
                .RunAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 2;
        }
    }
}