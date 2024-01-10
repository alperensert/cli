using Google.Apis.Compute.v1.Data;

namespace Utilities.Services;

/// <summary>
///     Represents a compute engine service that allows starting, stopping, and listing instances.
/// </summary>
public interface IComputeEngineService
{
    /// <summary>
    ///     Starts the instance asynchronously with the specified name.
    /// </summary>
    /// <param name="instanceName">The name of the instance to start.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation. The task will complete when the instance has started
    ///     successfully.
    /// </returns>
    Task StartInstanceAsync(string instanceName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops the instance asynchronously.
    /// </summary>
    /// <param name="instanceName">The name of the instance to be stopped.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StopInstanceAsync(string instanceName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves a list of instances.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of instances.</returns>
    Task<List<Instance>> ListInstancesAsync(CancellationToken cancellationToken = default);
}