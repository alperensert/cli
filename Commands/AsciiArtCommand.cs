using Spectre.Console;
using Utilities.Attributes;

namespace Commands;

[Command("ascii")]
public sealed class AsciiArtCommand : CliCommand
{
    [Subcommand("quasm", "ASCII art for Quasm.")]
    public void QuasmAsciiArt()
    {
        AnsiConsole.Markup("""
                           [green3_1]
                           
                                ██████╗ ██╗   ██╗ █████╗ ███████╗███╗   ███╗
                               ██╔═══██╗██║   ██║██╔══██╗██╔════╝████╗ ████║
                               ██║   ██║██║   ██║███████║███████╗██╔████╔██║
                               ██║▄▄ ██║██║   ██║██╔══██║╚════██║██║╚██╔╝██║
                               ╚██████╔╝╚██████╔╝██║  ██║███████║██║ ╚═╝ ██║
                                ╚══▀▀═╝  ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝.dev ╚═╝
                                
                           [/]
                           """);
    }

    [Subcommand("199x", "ASCII art for 199x.")]
    public void OneNineNineAsciiArt()
    {
        AnsiConsole.Markup("""
                           [blue]
                           
                              ______/\\\______/\\\\\\\\\_________/\\\\\\\\\_________________
                              __/\\\\\\\____/\\\///////\\\_____/\\\///////\\\________________
                               _\/////\\\___/\\\______\//\\\___/\\\______\//\\\_______________
                                _____\/\\\__\//\\\_____/\\\\\__\//\\\_____/\\\\\__/\\\____/\\\_
                                 _____\/\\\___\///\\\\\\\\/\\\___\///\\\\\\\\/\\\_\///\\\/\\\/__
                                  _____\/\\\_____\////////\/\\\_____\////////\/\\\___\///\\\/____
                                   _____\/\\\___/\\________/\\\____/\\________/\\\_____/\\\/\\\___
                                    _____\/\\\__\//\\\\\\\\\\\/____\//\\\\\\\\\\\/____/\\\/\///\\\_
                                     _____\///____\///////////_______\///////////_____\///____\///__
                                  
                           [/]
                           """);
    }

    [Subcommand("custom", "Custom ASCII art.")]
    public void CustomAsciiArt([Option("--text", "Text for ASCII.")] string? text = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            AnsiConsole.MarkupLine(":red_exclamation_mark: [red]No text provided for ASCII art.[/]");
            return;
        }

        AnsiConsole.Write(new FigletText(text)
                        .LeftJustified()
                        .Color(Color.Blue));
    }
}