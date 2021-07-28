using System.Threading.Tasks;

namespace Ubiquitous.AutoDevOps.Deployments {
    public interface IStackDeployment<T> where T : IDeploymentOptions {
        Task<int> DeployStack(IStackConfiguration<T> configuration, T options, bool preview = false);
    }
}