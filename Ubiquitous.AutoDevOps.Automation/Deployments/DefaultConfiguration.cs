using Pulumi.Automation;
using Serilog;
using Ubiquitous.AutoDevOps.Stack;

namespace Ubiquitous.AutoDevOps.Deployments;

public class DefaultConfiguration : IStackConfiguration<DefaultOptions> {
    public Task InstallPlugins(Workspace workspace)
        => workspace.InstallPluginAsync("kubernetes", "v3.9.0");

    public async Task ConfigureStack(
        WorkspaceStack                 appStack,
        AutoDevOpsSettings.AppSettings appSettings,
        DefaultOptions                 options
    ) {
        var deploymentSettings = await Settings.GetDeploymentSettings();

        await appStack.SetGitLabSettings();
        await appStack.SetRegistrySettings();
        await appStack.SetJsonConfig("app", appSettings);
        await appStack.SetJsonConfig("deploy", Settings.DeploySettings(options.Image, options.Percentage, appSettings.Track));
        await appStack.SetJsonConfig("service", deploymentSettings.Service);
        await appStack.SetJsonConfig("ingress", deploymentSettings.Ingress);
        await appStack.SetJsonConfig("prometheus", deploymentSettings.Prometheus);
    }

    public LocalWorkspaceOptions GetStackArgs(
        string name,
        string stack,
        string currentDir
    ) {
        var customStackPath  = Path.Combine(currentDir, "deploy");
        var usingCustomStack = Directory.Exists(customStackPath);

        if (usingCustomStack)
            Log.Information("Using the custom stack in {Directory}", customStackPath);

        return usingCustomStack
            ? new LocalProgramArgs(stack, customStackPath)
            : new InlineProgramArgs(name, stack, PulumiFn.Create<DefaultStack>()) {
                WorkDir = currentDir
            };
    }
}