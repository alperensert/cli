using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Utilities.Attributes;
using Utilities.Configurations;

namespace Commands;

[Command("config", "Configure CLI settings.")]
public sealed class ConfigCommand(IOptions<ComputeEngineConfiguration> computeEngineConfig) : CliCommand
{
    [Subcommand("compute-engine", "Configure compute engine command settings.", "ce", "vm")]
    public async Task ChangeComputeEngineSettings([Option("--zone", "Zone setting")] string? zone = null,
        [Option("--project-id", "Project ID setting")]
        string? projectId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(zone) && string.IsNullOrWhiteSpace(projectId))
        {
            var currentConfigTable = new Table()
                .RoundedBorder()
                .AddColumns("Name", "Value");
            var computeEngineConfiguration = computeEngineConfig.Value;
            currentConfigTable.AddRow("Zone", computeEngineConfiguration.Zone);
            currentConfigTable.AddRow("Project ID", computeEngineConfiguration.ProjectId);
            AnsiConsole.Write(currentConfigTable);
            return;
        }

        if (zone is not null)
            await UpdateAppSettings("ComputeEngine." + nameof(ComputeEngineConfiguration.Zone), zone,
                cancellationToken);

        if (projectId is not null)
            await UpdateAppSettings("ComputeEngine." + nameof(ComputeEngineConfiguration.ProjectId), projectId,
                cancellationToken);
    }

    /// <summary>
    ///     Updates the value of a specified key in the appsettings.json file.
    /// </summary>
    /// <param name="key">The key whose value needs to be updated.</param>
    /// <param name="value">The new value to set for the key.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    private static async Task UpdateAppSettings(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var path = Directory.GetCurrentDirectory() + "\\appsettings.json";

            var json = await File.ReadAllTextAsync(path, cancellationToken);
            var jsonObj = JsonConvert.DeserializeObject<JObject>(json) ?? throw new InvalidOperationException();

            var keys = key.Split('.');
            SetNestedValues(jsonObj, keys, value);
            var output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            await AnsiConsole.Status()
                .StartAsync($"{key}'s value is updating..",
                    async _ => await File.WriteAllTextAsync(path, output, cancellationToken));
            AnsiConsole.MarkupLine($":check_mark: [green1]{key} is updated.[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($":red_exclamation_mark: [red]{key} couldn't be updated.[/]");
            AnsiConsole.WriteException(e);
        }
    }

    /// <summary>
    ///     Sets a value in a nested JSON object using a list of keys.
    /// </summary>
    /// <param name="jsonObj">The JSON object to update.</param>
    /// <param name="keys">The list of keys to traverse to reach the desired value.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="index">The starting index in the list of keys (optional, default is 0).</param>
    /// <remarks>
    ///     This method allows you to set a nested value in a JSON object by specifying a list of keys.
    ///     It starts from the given index in the list of keys and traverses the JSON object using each key,
    ///     until it reaches the last key in the list. Then it updates the value at that key with the specified value.
    /// </remarks>
    private static void SetNestedValues(JObject jsonObj, IReadOnlyList<string> keys, string value, int index = 0)
    {
        while (true)
        {
            var key = keys[index];

            if (index == keys.Count - 1)
            {
                jsonObj[key] = value;
                return;
            }

            var nextObj = (JObject)jsonObj[key]!;
            jsonObj = nextObj;
            index += 1;
        }
    }
}