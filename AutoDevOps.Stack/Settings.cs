using Pulumi;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AutoDevOps.Stack {
    public class AutoDevOpsSettings {
        public AutoDevOpsSettings(Config config) {
            Deploy      = config.RequireObject<DeploySettings>("deploy");
            Application = config.RequireObject<AppSettings>("app");
            GitLab      = config.RequireObject<GitLabSettings>("gitlab");
            Registry    = config.GetObject<RegistrySettings>("registry");
            Service     = config.RequireObject<ServiceSettings>("service");
            Ingress     = config.RequireObject<IngressSettings>("ingress");
            Prometheus  = config.RequireObject<PrometheusSettings>("prometheus");
            Env         = config.GetObject<EnvVar[]>("env");
        }

        public string PulumiName(string resource) => $"{Application.Name}-{resource}";

        public string FullName() => $"{Application.Name}-{GitLab.EnvName}";

        public EnvVar[]? Env { get; }

        public DeploySettings Deploy { get; }

        public AppSettings Application { get; }

        public GitLabSettings GitLab { get; }

        public RegistrySettings? Registry { get; }

        public ServiceSettings Service { get; }

        public IngressSettings Ingress { get; }

        public PrometheusSettings Prometheus { get; }

        public record DeploySettings(
            string Namespace,
            string Release,
            int    Replicas,
            int    Percentage,
            string Image,
            string ImageTag,
            string Url
        );

        public record AppSettings(
            string  Name,
            string  Tier,
            string  Track,
            string? Version,
            int     Port           = 5000,
            string  ReadinessProbe = "/ping",
            string  LivenessProbe    = "/health"
        );

        public record GitLabSettings(
            string  App        = "",
            string  Env        = "development",
            string  EnvName    = "development",
            string? EnvUrl     = null,
            string  Visibility = ""
        );

        public record RegistrySettings(string Server, string User, string Password, string Email);

        public record ServiceSettings(bool Enabled, string Type, int ExternalPort);

        public record TlsSettings(bool Enabled, string? SecretName);

        public record IngressSettings(bool Enabled, TlsSettings? Tls, string Class = "nginx");

        public record PrometheusSettings(bool Metrics, string Path = "/metrics", bool Operator = false);

        public record EnvVar(string Name, string Value);
    }
}