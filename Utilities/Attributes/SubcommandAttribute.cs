namespace Utilities.Attributes;

/// <summary>
///     Specifies that a method is a subcommand for a command-line application.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SubcommandAttribute(string name, string? description = null, params string[] aliases) : Attribute
{
    /// <summary>
    ///     Gets the name of the property.
    /// </summary>
    /// <value>
    ///     The name of the property.
    /// </value>
    public string Name { get; } = name;

    /// <summary>
    ///     Gets or sets the description of the property.
    /// </summary>
    /// <value>
    ///     A <see cref="System.String" /> representing the description.
    /// </value>
    public string? Description { get; } = description;

    /// <summary>
    ///     Gets or sets the aliases for the property.
    /// </summary>
    /// <value>
    ///     An array of strings representing the aliases for the property.
    /// </value>
    public string[] Aliases { get; } = aliases;
}