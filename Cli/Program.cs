using System.CommandLine;
using Cli.Extensions;
using Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Utilities.Attributes;
using Utilities.Configurations;
using Utilities.Services;

namespace Cli;

internal static class Program
{
    /// <summary>
    ///     Main method is the entry point of the program.
    ///     It initializes the necessary services, configuration, and command objects.
    ///     It then invokes the root command with the given command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the program.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        try
        {
            var services = new ServiceCollection();
            IConfiguration configuration;
            try
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                return;
            }

            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            var assembly = typeof(ICliCommand).Assembly;

            var rootCommand = new RootCommand();

            var commandTypes = assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(CommandAttribute), false).Length != 0 &&
                               type.IsSubclassOf(typeof(CliCommand)));

            foreach (var type in commandTypes)
            {
                var command = type.BuildCommand(serviceProvider);
                rootCommand.AddCommand(command);
            }

            await rootCommand.InvokeAsync(args);
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ComputeEngineConfiguration>(configuration.GetRequiredSection("ComputeEngine"));
        services.AddOptions();
        services.AddScoped<IComputeEngineService, ComputeEngineService>();
    }
}