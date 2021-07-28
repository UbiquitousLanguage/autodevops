namespace Ubiquitous.AutoDevOps.Deployments {
    public interface IDeploymentOptions {
        string Name    { get; }
        string Stack   { get; }
        string Tier    { get; }
        string Track   { get; }
        string Version { get; }
        bool   Preview { get; }
    }
}