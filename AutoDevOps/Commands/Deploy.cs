using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Threading.Tasks;
using AutoDevOps.Stack;
using Pulumi.Automation;

namespace AutoDevOps.Commands {
    class Deploy : Command {
        public Deploy() : base("deploy", "Deploy (update) the stack") {
            Delegate d = new Func<string, string, string, string, string, string, string, int, Task<int>>(DeployStack);
            Handler = CommandHandler.Create(d);

            AddOption(new Option<string>("--name", () => Defaults.GitLabVar("PROJECT_NAME"), "Application name"));
            AddOption(new Option<string>("--tier", () => "web", "Application tier"));
            AddOption(new Option<string>("--track", () => "stable", "Application track"));
            AddOption(new Option<string>("--image", "Image repository"));
            AddOption(new Option<string>("--tag", "Image tag"));
            AddOption(new Option<int>("--percentage", () => 100, "Deployment percentage"));

            AddOption(
                new Option<string>(
                    "--version",
                    () => Environment.GetEnvironmentVariable("APPLICATION_VERSION"),
                    "Application version"
                )
            );
        }

        async Task<int> DeployStack(string stack, string name, string tier, string track, string version, string image, string tag, int percentage) {
            Console.WriteLine($"Deploying {stack}");

            using var workspace = await LocalWorkspace.CreateAsync(
                new LocalWorkspaceOptions {
                    Program = PulumiFn.Create<DefaultStack>(),
                    ProjectSettings = new ProjectSettings(Defaults.ProjectName, ProjectRuntimeName.Dotnet) {
                        Description = Defaults.ProjectDescription,
                        Website     = Defaults.ProjectWebsite
                    }
                }
            );

            var appStack = await WorkspaceStack.CreateOrSelectAsync(Defaults.Environment, workspace);

            await appStack.SetConfigValueAsync("gitlab", new ConfigValue(JsonSerializer.Serialize(Defaults.GitLabSettings)));

            await appStack.SetConfigValueAsync(
                "registry",
                new ConfigValue(JsonSerializer.Serialize(Defaults.RegistrySettings), true)
            );

            var appSettings = new AutoDevOpsSettings.AppSettings(name, tier, track, version);
            await appStack.SetConfigValueAsync("app", new ConfigValue(JsonSerializer.Serialize(appSettings)));

            var deploySettings = new AutoDevOpsSettings.DeploySettings(
                Defaults.EnvVar("KUBE_NAMESPACE"),
                Defaults.GitLabVar("ENVIRONMENT_NAME"),
                1,
                percentage,
                image,
                tag,
                Defaults.GitLabVar("ENVIRONMENT_URL")
            );
            await appStack.SetConfigValueAsync("deploy", new ConfigValue(JsonSerializer.Serialize(deploySettings)));

            var result = await appStack.UpAsync(new UpOptions {
                OnStandardOutput = Console.Out.WriteLine,
                OnStandardError = Console.Error.WriteLine
            });
            Console.WriteLine(result.Summary.Message);
            return result.Summary.Result == UpdateState.Succeeded ? 0 : -1;
        }
        
/*
    pulumi config set --path deploy.Image "$image_repository"
    pulumi config set --path deploy.ImageTag "$image_tag"
    pulumi config set --path deploy.Percentage "$percentage"
    pulumi config set --path deploy.Release "$CI_ENVIRONMENT_NAME"
    pulumi config set --path deploy.Namespace "$KUBE_NAMESPACE"
    pulumi config set --path deploy.Url "$CI_ENVIRONMENT_URL"

    # pulumi config set --path service.CommonName "le-$CI_PROJECT_ID.$KUBE_INGRESS_BASE_DOMAIN"
    # pulumi config set --path service.Url "$CI_ENVIRONMENT_URL"

    pulumi up -y -f -r

    pulumi stack tag set app:version "$APPLICATION_VERSION"
    pulumi stack tag set app:image "$image_repository:$image_tag"
    pulumi stack tag set app:url "$CI_ENVIRONMENT_URL"
    pulumi stack tag set app:namespace "$KUBE_NAMESPACE"
*/
    }
}