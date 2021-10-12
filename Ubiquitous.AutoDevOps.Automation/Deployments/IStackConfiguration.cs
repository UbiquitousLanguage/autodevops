using Pulumi.Automation;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Deployments; 

public interface IStackConfiguration<T> where T : IDeploymentOptions {
    Task InstallPlugins(Workspace workspace);

    Task ConfigureStack(WorkspaceStack appStack, AppSettings appSettings, T options);

    LocalWorkspaceOptions GetStackArgs(string name, string stack, string currentDir);
}