namespace Ubiquitous.AutoDevOps.Deployments; 

public interface IStackDeployment<T> where T : IDeploymentOptions {
    Task<CommandResult> DeployStack(IStackConfiguration<T> configuration, T options);
}