using Pulumi;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Ubiquitous.AutoDevOps.Stack {
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

        public EnvVar[]?          Env         { get; }
        public DeploySettings     Deploy      { get; }
        public AppSettings        Application { get; }
        public GitLabSettings     GitLab      { get; }
        public RegistrySettings?  Registry    { get; }
        public ServiceSettings    Service     { get; }
        public IngressSettings    Ingress     { get; }
        public PrometheusSettings Prometheus  { get; }

        public record DeploySettings(
            string  ResourceName,
            string  Namespace,
            string  Release,
            int     Replicas,
            int     Percentage,
            string  Image,
            string? Url
        );

        public record AppSettings(
            string  Name,
            string  Tier,
            string  Track,
            string? Version,
            int     Port           = 5000,
            string  ReadinessProbe = "/ping",
            string  LivenessProbe  = "/health"
        );

        public record GitLabSettings(
            string  App        = "",
            string  Env        = "development",
            string  EnvName    = "development",
            string  Visibility = ""
        );

        public record RegistrySettings(string Server, string User, string Password, string Email);

        public record ServiceSettings {
            public bool   Enabled      { get; init; }
            public string Type         { get; init; } = "ClusterIP";
            public int    ExternalPort { get; init; }
        }

        public record TlsSettings {
            public bool    Enabled    { get; init; }
            public string? SecretName { get; init; }
        }

        public record IngressSettings {
            public bool         Enabled { get; init; }
            public TlsSettings? Tls     { get; init; }
            public string       Class   { get; init; }
        }

        public record PrometheusSettings {
            public bool   Metrics  { get; init; }
            public string Path     { get; init; }
            public bool   Operator { get; init; }
        }

        public record EnvVar(string Name, string Value);
    }
}