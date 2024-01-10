using Google.Apis.Compute.v1.Data;
using Spectre.Console;
using Utilities.Attributes;
using Utilities.Services;

namespace Commands;

[Command("compute-engine", "Control compute engine vm instances.", "ce", "vm")]
public sealed class ComputeEngineCommand(IComputeEngineService computeEngineService) : CliCommand
{
    [Subcommand("list", "Lists all compute engine VM instances.")]
    public async Task ListComputeEnginesAsync(CancellationToken cancellationToken)
    {
        var instances = new List<Instance>();
        await AnsiConsole.Status()
                        .StartAsync("Loading...",
                                        async _ => instances =
                                                        await computeEngineService.ListInstancesAsync(cancellationToken));
        var table = new Table()
                        .RoundedBorder()
                        .AddColumn("Id")
                        .AddColumn("Name")
                        .AddColumn("Status")
                        .AddColumn("Last Started")
                        .AddColumn("Deletion Protection");
        foreach (var instance in instances)
        {
            var statusMarkup = instance.Status == "RUNNING" ? "[green]" : "[red]";
            var deletionProtectionState = instance.DeletionProtection switch
            {
                            true => "[green]PROTECTING[/]",
                            false => "[red]NOT PROTECTING[/]",
                            null => "[white]UNKNOWN[/]"
            };
            table.AddRow(instance.Id.ToString() ?? string.Empty, instance.Name, $"{statusMarkup}{instance.Status}[/]",
                            instance.LastStartTimestamp, deletionProtectionState);
        }

        AnsiConsole.Write(table);
    }

    [Subcommand("stop", "Stop one of the compute engines at once.")]
    public async Task StopComputeEngineAsync(CancellationToken cancellationToken)
    {
        var instances = await computeEngineService.ListInstancesAsync(cancellationToken);
        instances = instances.Where(i => i.Status == "RUNNING").ToList();
        var instance = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                                        .Title("Select an instance to stop:")
                                        .PageSize(12)
                                        .MoreChoicesText("[grey](Move up and down to reveal more instances)[/]")
                                        .AddChoices(instances.Select(i => $"{i.Name}")));
        await AnsiConsole.Status()
                        .StartAsync("Sending the request...",
                                        async _ =>
                                        {
                                            await computeEngineService.StopInstanceAsync(instance, cancellationToken);
                                        });
        AnsiConsole.Write(new Text($"{instance} is stopping.", new Style(Color.Red1)));
    }

    [Subcommand("start", "Start one of the compute engines at once.")]
    public async Task StartComputeEngineAsync(CancellationToken cancellationToken)
    {
        var instances = await computeEngineService.ListInstancesAsync(cancellationToken);
        instances = instances.Where(i => i.Status is "STOPPING" or "TERMINATED").ToList();
        var instance = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                                        .Title("Select an instance to start:")
                                        .PageSize(12)
                                        .MoreChoicesText("[grey](Move up and down to reveal more instances)[/]")
                                        .AddChoices(instances.Select(i => $"{i.Name}")));
        await AnsiConsole.Status()
                        .StartAsync("Sending the request...",
                                        async _ =>
                                        {
                                            await computeEngineService.StartInstanceAsync(instance, cancellationToken);
                                        });
        AnsiConsole.Write(new Text($"{instance} is starting.", new Style(Color.Green1)));
    }
}