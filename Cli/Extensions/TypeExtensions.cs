using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Commands;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Attributes;

namespace Cli.Extensions;

/// <summary>
///     A static class containing extension methods for the Type class.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Builds a command based on the provided type and service provider.
    /// </summary>
    /// <param name="type">The type representing the command.</param>
    /// <param name="serviceProvider">The service provider used to create instances of the command.</param>
    /// <returns>The built command.</returns>
    public static Command BuildCommand(this Type type, IServiceProvider serviceProvider)
    {
        var attribute = (CommandAttribute)Attribute.GetCustomAttributes(type, typeof(CommandAttribute)).First();
        if (ActivatorUtilities.CreateInstance(serviceProvider, type) is not CliCommand cliCommand)
            throw new ApplicationException($"{attribute.Name} command is not inherits the {nameof(CliCommand)}");
        if (string.IsNullOrWhiteSpace(attribute.Name))
            throw new ApplicationException("A command has null or whitespace name.");
        var command = new Command(attribute.Name, attribute.Description);

        var subCommandMethods = type.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(SubcommandAttribute), false).Length != 0);

        foreach (var subCommandMethod in subCommandMethods)
        {
            var subCommandAttribute =
                (SubcommandAttribute)subCommandMethod.GetCustomAttributes(typeof(SubcommandAttribute), true).First();
            var subCommand = new Command(subCommandAttribute.Name, subCommandAttribute.Description);

            var parameterInfos = subCommandMethod.GetParameters();

            foreach (var pi in parameterInfos)
            {
                var optionAttribute =
                    (OptionAttribute?)pi.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault();
                if (optionAttribute == null) continue;
                var option = optionAttribute.CreateOption(pi.ParameterType);
                subCommand.AddOption(option);
            }

            subCommand.Handler = CommandHandler.Create(subCommandMethod.CreateCommandDelegate(cliCommand));
            command.AddCommand(subCommand);
        }

        var onExecuteAsyncInfo = type.GetMethod("OnExecuteAsync");

        if (onExecuteAsyncInfo != null && onExecuteAsyncInfo.DeclaringType != typeof(CliCommand))
            command.Handler = CommandHandler.Create(
                (Func<CancellationToken, Task>)Delegate.CreateDelegate(
                    typeof(Func<CancellationToken, Task>), cliCommand, onExecuteAsyncInfo.Name));
        else
            command.Handler = CommandHandler.Create(
                (Action)Delegate.CreateDelegate(
                    typeof(Action), cliCommand, "OnExecute"));

        return command;
    }
}