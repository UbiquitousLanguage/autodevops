using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Ubiquitous.AutoDevOps.Env;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps; 

public static class Settings {
    public static GitLabSettings GitLabSettings()
        => new(ProjectName, EnvironmentSlug, Env.Environment, ProjectVisibility);

    public static RegistrySettings RegistrySettings()
        => new(Registry, DeployRegistryUser, DeployRegistryPassword, UserEmail);

    public static DeploySettings DeploySettings(string image, int percentage, string track)
        => new(KubeNamespace, Env.Environment, Replicas(track), percentage, image, EnvironmentUrl);

    static int Replicas(string track) {
        return int.TryParse(EnvReplicas(track), out var envReplicas) ? Adjust(envReplicas) :
            int.TryParse(Env.Replicas, out var defaultReplicas)      ? Adjust(defaultReplicas) : 1;

        static int Adjust(int count) => count == 0 ? 1 : count;
    }

    public static async Task<DeploymentSettings> GetDeploymentSettings() {
        var valuesFile = Path.Join(".pulumi", "values.yaml");

        if (!File.Exists(valuesFile)) {
            Log.Information("Using default deployment settings");

            return new DeploymentSettings {
                Ingress    = new IngressSettings {Enabled    = EnvironmentUrl != null},
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
    public ServiceSettings    Service    { get; init; } = default!;
    public IngressSettings    Ingress    { get; init; } = default!;
    public PrometheusSettings Prometheus { get; init; } = default!;
}