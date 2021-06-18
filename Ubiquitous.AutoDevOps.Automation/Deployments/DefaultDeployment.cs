using System.IO;
using System.Threading.Tasks;
using Pulumi.Automation;
using Ubiquitous.AutoDevOps.Stack;
using static Serilog.Log;

namespace Ubiquitous.AutoDevOps.Deployments {
    public class DefaultDeployment<T> : IStackDeployment<T> where T : IDeploymentOptions {
        public async Task<int> DeployStack(IStackConfiguration<T> configuration, T options) {
            var currentDir = Directory.GetCurrentDirectory();

            Information("Starting with {Name} {Stack} in {CurrentDir}", options.Name, options.Stack, currentDir);

            var stackArgs = configuration.GetStackArgs(options.Name, options.Stack, currentDir);

            using var workspace = await LocalWorkspace.CreateAsync(stackArgs);

            var appStack = await WorkspaceStack.CreateOrSelectAsync(options.Stack, workspace);

            Information("Configuring stack {Stack}", options.Stack);

            var appSettings = new AutoDevOpsSettings.AppSettings(
                options.Name,
                options.Tier,
                options.Track,
                options.Version
            );
            await configuration.ConfigureStack(appStack, appSettings, options);

            Information("Refreshing stack {Stack}", options.Stack);
            await appStack.RefreshAsync();

            Information("Installing plugins");
            await configuration.InstallPlugins(appStack.Workspace);

            Information("Deploying stack {Stack}", options.Stack);

            var result = await appStack.UpAsync(
                new UpOptions {
                    OnStandardOutput = Information,
                    OnStandardError  = Error
                }
            );
            Information("Deployment result: {Result}", result.Summary.Message);

            if (!Env.EnvironmentUrl.IsEmpty()) {
                Information("Environment URL: {EnvironmentUrl}", Env.EnvironmentUrl);
            }

            return result.Summary.Result == UpdateState.Succeeded ? 0 : -1;
        }
    }
}