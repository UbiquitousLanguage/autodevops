using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static AutoDevOps.Env;
using static AutoDevOps.Stack.AutoDevOpsSettings;

namespace AutoDevOps {
    public static class Settings {
        public static GitLabSettings GitLabSettings()
            => new(ProjectName, EnvironmentSlug, Env.Environment, EnvironmentUrl, ProjectVisibility);

        public static RegistrySettings RegistrySettings()
            => new(Registry, DeployRegistryUser, DeployRegistryPassword, UserEmail);

        public static DeploySettings DeploySettings(string image, string tag, int percentage) {
            return new(KubeNamespace, Env.Environment, 1, percentage, image, tag, EnvironmentUrl);
        }

        public static async Task<DeploymentSettings> GetDeploymentSettings() {
            var valuesFile = Path.Join(".pulumi", "values.yaml");

            if (!File.Exists(valuesFile)) {
                Serilog.Log.Information("Using default deployment settings");

                return new DeploymentSettings {
                    Ingress    = new IngressSettings {Enabled    = false},
                    Service    = new ServiceSettings {Enabled    = true, Type = "ClusterIP", ExternalPort = 5000},
                    Prometheus = new PrometheusSettings {Metrics = false}
                };
            }

            Serilog.Log.Information("Using custom deployment settings from {ValuesFile}", valuesFile);

            var serializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var settingsString = await File.ReadAllTextAsync(valuesFile);
            return serializer.Deserialize<DeploymentSettings>(settingsString);
        }

        public static string GetImageTag() {
            const string versionFileName = "version.sh";
            if (!File.Exists(versionFileName)) return ImageTag();

            var versionFile = File.ReadAllText(versionFileName);
            var variable    = versionFile.Replace("export", "", StringComparison.InvariantCultureIgnoreCase).Trim();
            var split       = variable.Split('=');

            if (split[0] == AppVersionVar)
                System.Environment.SetEnvironmentVariable(AppVersionVar, split[1].Replace("\"", ""));

            return ImageTag();
        }
    }

    public class DeploymentSettings {
        public ServiceSettings    Service    { get; init; }
        public IngressSettings    Ingress    { get; init; }
        public PrometheusSettings Prometheus { get; init; }
    }
}