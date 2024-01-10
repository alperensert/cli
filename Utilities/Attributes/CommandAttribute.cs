namespace Utilities.Attributes;

/// <summary>
///     Represents an attribute that can be applied to a class to define a command.
///     Commands are used in a command-line interface or console application to perform specific actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string name, string? description = null, params string[] aliases) : Attribute
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