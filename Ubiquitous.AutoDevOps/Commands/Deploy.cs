using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Ubiquitous.AutoDevOps.Stack;
using Pulumi.Automation;
using static Serilog.Log;

namespace Ubiquitous.AutoDevOps.Commands {
    class Deploy : Command {
        public Deploy() : base("deploy", "Deploy (update) the stack") {
            Delegate d = new Func<string, string, string, string, string, string, string, int, Task<int>>(DeployStack);
            Handler = CommandHandler.Create(d);

            AddOption(new Option<string>("--tier", () => "web", "Application tier"));
            AddOption(new Option<string>("--track", () => "stable", "Application track"));
            AddOption(new Option<string>("--image", Settings.GetImageRegistry, "Image registry"));
            AddOption(new Option<string>("--tag", Settings.GetImageTag, "Image tag"));
            AddOption(new Option<int>("--percentage", () => 100, "Deployment percentage"));
            AddOption(new Option<string>("--version", () => Env.ApplicationVersion, "Application version"));
        }

        static async Task<int> DeployStack(
            string stack,
            string name,
            string tier,
            string track,
            string version,
            string image,
            string tag,
            int    percentage
        ) {
            var currentDir = Directory.GetCurrentDirectory();

            Information("Starting with {Name} {Stack} in {CurrentDir}", name, stack, currentDir);

            using var workspace = await LocalWorkspace.CreateAsync(
                new LocalWorkspaceOptions {
                    Program         = PulumiFn.Create<DefaultStack>(),
                    ProjectSettings = new ProjectSettings(name, ProjectRuntimeName.Dotnet),
                    WorkDir         = currentDir
                }
            );
            var appStack = await WorkspaceStack.CreateOrSelectAsync(stack, workspace);
            await appStack.RefreshAsync();

            Information("Configuring stack {Stack}", stack);

            var appSettings = new AutoDevOpsSettings.AppSettings(name, tier, track, version);

            var deploymentSettings = await Settings.GetDeploymentSettings();

            await appStack.SetJsonConfig("gitlab", Settings.GitLabSettings());
            await appStack.SetJsonConfig("registry", Settings.RegistrySettings(), true);
            await appStack.SetJsonConfig("app", appSettings);
            await appStack.SetJsonConfig("deploy", Settings.DeploySettings(image, tag, percentage));
            await appStack.SetJsonConfig("service", deploymentSettings.Service);
            await appStack.SetJsonConfig("ingress", deploymentSettings.Ingress);
            await appStack.SetJsonConfig("prometheus", deploymentSettings.Prometheus);

            Information("Installing plugins");

            await appStack.Workspace.InstallPluginAsync("kubernetes", "v3.0.0");

            Information("Deploying stack {Stack}", stack);

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