using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Ubiquitous.AutoDevOps.Env;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps {
    public static class Settings {
        public static GitLabSettings GitLabSettings()
            => new(ProjectName, EnvironmentSlug, Env.Environment, EnvironmentUrl, ProjectVisibility);

        public static RegistrySettings RegistrySettings()
            => new(Registry, DeployRegistryUser, DeployRegistryPassword, UserEmail);

        public static DeploySettings DeploySettings(string image, string tag, int percentage)
            => new(KubeNamespace, Env.Environment, 1, percentage, image, tag, EnvironmentUrl);

        public static async Task<DeploymentSettings> GetDeploymentSettings() {
            var valuesFile = Path.Join(".pulumi", "values.yaml");

            if (!File.Exists(valuesFile)) {
                Log.Information("Using default deployment settings");

                return new DeploymentSettings {
                    Ingress    = new IngressSettings {Enabled    = false},
                    Service    = new ServiceSettings {Enabled    = true, Type = "ClusterIP", ExternalPort = 5000},
                    Prometheus = new PrometheusSettings {Metrics = false}
                };
            }

            Log.Information("Using custom deployment settings from {ValuesFile}", valuesFile);

            var serializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var settingsString = await File.ReadAllTextAsync(valuesFile);
            return serializer.Deserialize<DeploymentSettings>(settingsString);
        }

        public static string GetImageTag()
            => ParseShFile("version.sh", "Application version", AppVersionVar, ImageTag());

        public static string GetImageRegistry()
            => ParseShFile("docker-image.sh", "Image registry", AppRepositoryVar, ImageRegistry);

        static string ParseShFile(string fileName, string whatIsIt, string expectedVar, string defaultValue) {
            var value = defaultValue;
            if (!File.Exists(fileName)) {
                Log.Information("{What} artefact not found, using the default tag {Tag}", whatIsIt, defaultValue);
                return defaultValue;
            }

            var versionFile = File.ReadAllText(fileName);
            var variable    = versionFile.Replace("export", "", StringComparison.InvariantCultureIgnoreCase).Trim();
            Log.Debug("{What}: {Version}", whatIsIt, variable);
            var split = variable.Split('=');

            if (split[0] == expectedVar) value = split[1].Replace("\"", "");

            Log.Information("{What} artefact found, using {Tag}", whatIsIt, value);

            return value;
        }
    }

    public class DeploymentSettings {
        public ServiceSettings    Service    { get; init; }
        public IngressSettings    Ingress    { get; init; }
        public PrometheusSettings Prometheus { get; init; }
    }
}