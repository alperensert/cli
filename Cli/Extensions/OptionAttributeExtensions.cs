using System.CommandLine;
using Utilities.Attributes;

namespace Cli.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="OptionAttribute" /> class.
/// </summary>
public static class OptionAttributeExtensions
{
    /// <summary>
    ///     Creates an instance of <see cref="Option" /> based on the provided <paramref name="optionAttribute" /> and
    ///     <paramref name="type" />.
    /// </summary>
    /// <param name="optionAttribute">The <see cref="OptionAttribute" />.</param>
    /// <param name="type">The <see cref="Type" /> to be used for creating the generic type.</param>
    /// <returns>An instance of <see cref="Option" /> with the specified name and description.</returns>
    public static Option CreateOption(this OptionAttribute optionAttribute, Type type)
    {
        return ((Option)Activator.CreateInstance(typeof(Option<>).MakeGenericType(type), optionAttribute.Name,
            optionAttribute.Description)!)!;
    }
}