using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AutoDevOps.Stack;
using Pulumi;
using Pulumi.Automation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AutoDevOps.Commands {
    class Deploy : Command {
        public Deploy() : base("deploy", "Deploy (update) the stack") {
            Delegate d = new Func<string, string, string, string, string, string, string, int, Task<int>>(DeployStack);
            Handler = CommandHandler.Create(d);

            AddOption(new Option<string>("--tier", () => "web", "Application tier"));
            AddOption(new Option<string>("--track", () => "stable", "Application track"));
            AddOption(new Option<string>("--image", () => Env.ImageRepository, "Image repository"));
            AddOption(new Option<string>("--tag", () => Env.ImageTag, "Image tag"));
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
            var valuesFile = Path.Join(".pulumi", "values.yaml");

            if (!File.Exists(valuesFile))
                throw new FileNotFoundException("Mandatory deployment config not found");

            var serializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var currentDir = Directory.GetCurrentDirectory();

            Console.WriteLine($"Starting with {name} {stack} in {currentDir}");

            // var program = PulumiFn.Create(
            //     () => {
            //         var config   = new Config();
            //         var settings = new AutoDevOpsSettings(config);
            //         var _        = new Stack.AutoDevOps(settings);
            //     }
            // );
            // var stackArgs = new InlineProgramArgs(projectName, stack, program);
            // using var appStack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
            using var workspace = await LocalWorkspace.CreateAsync(
                new LocalWorkspaceOptions {
                    Program         = PulumiFn.Create<DefaultStack>(),
                    ProjectSettings = new ProjectSettings(name, ProjectRuntimeName.Dotnet),
                    WorkDir         = currentDir
                }
            );
            var appStack = await WorkspaceStack.CreateOrSelectAsync(stack, workspace);

            Console.WriteLine($"Configuring stack {stack}");

            var appSettings = new AutoDevOpsSettings.AppSettings(name, tier, track, version);

            var settingsString     = await File.ReadAllTextAsync(valuesFile);
            var deploymentSettings = serializer.Deserialize<DeploymentSettings>(settingsString);

            await appStack.SetJsonConfig("gitlab", Settings.GitLabSettings());
            await appStack.SetJsonConfig("registry", Settings.RegistrySettings(), true);
            await appStack.SetJsonConfig("app", appSettings);
            await appStack.SetJsonConfig("deploy", Settings.DeploySettings(image, tag, percentage));
            await appStack.SetJsonConfig("service", deploymentSettings.Service);
            await appStack.SetJsonConfig("ingress", deploymentSettings.Ingress);
            await appStack.SetJsonConfig("prometheus", deploymentSettings.Prometheus);

            Console.WriteLine("Installing plugins");

            await appStack.Workspace.InstallPluginAsync("kubernetes", "v2.8.4");

            Console.WriteLine($"Deploying stack {stack}");

            var result = await appStack.UpAsync(
                new UpOptions {
                    OnStandardOutput = Console.Out.WriteLine,
                    OnStandardError  = Console.Error.WriteLine
                }
            );
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
        class DeploymentSettings {
            public AutoDevOpsSettings.ServiceSettings    Service    { get; init; }
            public AutoDevOpsSettings.IngressSettings    Ingress    { get; init; }
            public AutoDevOpsSettings.PrometheusSettings Prometheus { get; init; }
        }
    }
}