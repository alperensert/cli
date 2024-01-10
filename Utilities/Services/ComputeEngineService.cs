using Google.Apis.Auth.OAuth2;
using Google.Apis.Compute.v1;
using Google.Apis.Compute.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Utilities.Configurations;

namespace Utilities.Services;

public sealed class ComputeEngineService(IOptions<ComputeEngineConfiguration> ceOptions) : IComputeEngineService
{
    private readonly ComputeService _computeService = Authenticate();

    public async Task StartInstanceAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        await _computeService.Instances.Start(ceOptions.Value.ProjectId, ceOptions.Value.Zone, instanceName)
            .ExecuteAsync(cancellationToken);
    }

    public async Task StopInstanceAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        await _computeService.Instances.Stop(ceOptions.Value.ProjectId, ceOptions.Value.Zone, instanceName)
            .ExecuteAsync(cancellationToken);
    }

    public async Task<List<Instance>> ListInstancesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _computeService.Instances.List(ceOptions.Value.ProjectId, ceOptions.Value.Zone)
            .ExecuteAsync(cancellationToken);
        return response.Items == null ? [] : response.Items.ToList();
    }

    /// <summary>
    ///     Authenticates the user via GCloud application default credentials and returns an instance of the ComputeService
    ///     class.
    /// </summary>
    /// <remarks>
    ///     This method uses GoogleCredential to authenticate the user.
    ///     It first retrieves the application default credentials asynchronously,
    ///     then checks if creating a scoped credential is required.
    ///     If required, it creates a scoped credential with the CloudPlatform scope.
    ///     Finally, it initializes the ComputeService class with the authenticated credentials
    ///     and the desired application name.
    /// </remarks>
    /// <returns>
    ///     An instance of the ComputeService class authenticated with the user's credentials.
    /// </returns>
    private static ComputeService Authenticate()
    {
        var credential = GoogleCredential.GetApplicationDefaultAsync().Result;
        if (credential.IsCreateScopedRequired)
            credential = credential.CreateScoped(ComputeService.Scope.CloudPlatform);
        return new ComputeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Compute Engine Service"
        });
    }
}