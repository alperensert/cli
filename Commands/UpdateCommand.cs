using System.Diagnostics;
using Octokit;
using Spectre.Console;
using Utilities.Attributes;
using FileMode = System.IO.FileMode;

namespace Commands;

[Command("update", "Update the CLI to newer version.")]
public sealed class UpdateCommand : CliCommand
{
    private static readonly HttpClient httpClient = new();
    private readonly GitHubClient _gitHubClient = new(new ProductHeaderValue("quasm-cli"));

    public override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        var releases = await AnsiConsole.Status()
            .StartAsync("Checking updates..", async _ => await _gitHubClient.Repository.Release.GetAll("alperensert", "cli"));
        var latestRelease = releases[0];
        var latestVersion = latestRelease.TagName;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "quasm",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var currentVersion = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (latestVersion == currentVersion[..5])
        {
            AnsiConsole.MarkupLine($"[bold green]You are already using the latest version of the CLI: {currentVersion}[/]");
            return;
        }

        var nupkgFileName = $"quasm.{latestVersion}.nupkg";
        var downloadUrl = latestRelease.Assets.First(asset => asset.Name == nupkgFileName)?.BrowserDownloadUrl;

        if (string.IsNullOrEmpty(downloadUrl))
        {
            AnsiConsole.MarkupLine("[bold red]Can't find download URL for the new version.[/]");
            return;
        }

        var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[bold red]Error occured while downloading the tool.[/]");
            return;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var isToolDownloaded = await DownloadNewVersionAsync(nupkgFileName, response, stream, cancellationToken);
        if (!isToolDownloaded)
        {
            AnsiConsole.MarkupLine("[bold red]Error occured while downloading the tool.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[bold green]Download completed![/]");

        var isToolUpdated = await UpdateTheToolAsync(cancellationToken);
        AnsiConsole.MarkupLine(!isToolUpdated
            ? "[bold red]Error occured while updating the tool.[/]"
            : $"[bold green]'quasm' tool is updated from {currentVersion} to {latestVersion} successfully.[/]");

        var downloadedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nupkgFileName);
        if (File.Exists(downloadedFilePath))
            File.Delete(downloadedFilePath);
    }

    /// <summary>
    ///     Updates the tool asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token used to cancel the operation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task will contain true if the tool update was successful;
    ///     otherwise, false.
    /// </returns>
    private static async Task<bool> UpdateTheToolAsync(CancellationToken cancellationToken = default)
    {
        return await AnsiConsole.Status()
            .StartAsync("Updating the tool", async _ =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(300, 800)), cancellationToken);

                    // Run the dotnet tool update -g --add-source ./ command
                    var updateProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = "tool update -g --add-source ./ quasm",
                            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    updateProcess.Start();
                    await updateProcess.WaitForExitAsync(cancellationToken);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    ///     Downloads a new version of a file asynchronously.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="stream">The stream containing the file data.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    ///     The task result is true if the download is successful; otherwise, false.
    /// </returns>
    private static async Task<bool> DownloadNewVersionAsync(string fileName, HttpResponseMessage response, Stream stream,
        CancellationToken cancellationToken = default)
    {
        return await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                try
                {
                    var downloadTask = ctx.AddTask("Downloading new version");

                    await using var fileStream =
                        new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), FileMode.Create,
                            FileAccess.Write, FileShare.None, 1048576, true);
                    var totalReadBytes = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;
                    if (response.Content.Headers.ContentLength == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(8.5), cancellationToken);
                        downloadTask.Increment(100);
                        return true;
                    }

                    do
                    {
                        var read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, read, cancellationToken);

                            totalReadBytes += read;
                            var progress = (double)totalReadBytes / response.Content.Headers.ContentLength.Value * 100;
                            downloadTask.Increment(progress);
                        }
                    } while (isMoreToRead);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }
}