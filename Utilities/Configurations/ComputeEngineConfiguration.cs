namespace Utilities.Configurations;

/// <summary>
///     Represents the configuration for compute-engine command.
/// </summary>
public class ComputeEngineConfiguration
{
    /// <summary>
    ///     The zone property represents the specific zone of an object.
    ///     The value of this property should provide information about the zone.
    /// </summary>
    /// <value>
    ///     A <see cref="string" /> representing the zone.
    /// </value>
    /// <remarks>
    ///     The zone can be any text-based identifier or description.
    ///     It can be used to categorize or group objects based on their specific zone.
    /// </remarks>
    public string Zone { get; init; } = null!;

    /// <summary>
    ///     Gets or sets the project ID.
    /// </summary>
    /// <value>
    ///     The project ID.
    /// </value>
    public string ProjectId { get; init; } = null!;
}