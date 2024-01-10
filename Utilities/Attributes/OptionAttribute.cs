namespace Utilities.Attributes;

/// <summary>
///     Attribute class used to mark a parameter as an option.
/// </summary>
/// <remarks>
///     This attribute should be used to mark parameters of a method or constructor that represent command line options.
///     The attribute provides a way to specify the name and description of the option.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public class OptionAttribute(string name, string? description = null) : Attribute
{
    /// <summary>
    ///     Gets the name of the property.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; } = name;

    /// <summary>
    ///     Gets the description of the property.
    /// </summary>
    /// <value>
    ///     The description of the property. It can be null.
    /// </value>
    public string? Description { get; } = description;
}