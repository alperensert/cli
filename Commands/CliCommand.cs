using System.Text;
using Spectre.Console;
using Utilities.Attributes;

namespace Commands;

/// <summary>
///     Represents an abstract class for CLI commands.
/// </summary>
public abstract class CliCommand : ICliCommand
{
    /// <summary>
    ///     Executes the method logic.
    /// </summary>
    public virtual void OnExecute()
    {
        // Get all methods in the current class using reflection
        var methods = GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(SubcommandAttribute), true).Length > 0).ToArray();

        var table = new Table().RoundedBorder()
            .AddColumn("Command")
            .AddColumn("Description")
            .AddColumn("Options");

        foreach (var method in methods)
        {
            var attributes = (SubcommandAttribute[])method.GetCustomAttributes(typeof(SubcommandAttribute), true);

            var optionsStringBuilder = new StringBuilder();

            foreach (var parameter in method.GetParameters())
            {
                var optionAttribute =
                    (OptionAttribute?)parameter.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault();
                if (optionAttribute != null)
                {
                    optionsStringBuilder.Append(optionAttribute.Name);
                    if (parameter.HasDefaultValue)
                    {
                        optionsStringBuilder.Append("=");
                        optionsStringBuilder.Append(parameter.DefaultValue ?? "null");
                    }
                }

                optionsStringBuilder.Append(", ");
            }

            var optionsString = optionsStringBuilder.ToString();
            optionsString = optionsString.TrimEnd(',', ' ');

            if (attributes.Length > 0)
                table.AddRow(attributes[0].Name, attributes[0].Description ?? "No description", optionsString);
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    ///     Executes the method asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}