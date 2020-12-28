using Pulumi;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AutoDevOps {
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

        public EnvVar[]?          Env         { get; }
        public DeploySettings     Deploy      { get; }
        public AppSettings        Application { get; }
        public GitLabSettings     GitLab      { get; }
        public RegistrySettings?  Registry    { get; }
        public ServiceSettings    Service     { get; }
        public IngressSettings    Ingress     { get; }
        public PrometheusSettings Prometheus  { get; }

        public class DeploySettings {
            public string Namespace       { get; set; } = null!;
            public string Release         { get; set; } = null!;
            public int    Replicas        { get; set; }
            public int    Percentage      { get; set; }
            public string Image           { get; set; } = null!;
            public string ImageTag        { get; set; } = null!;
            public string Url             { get; set; } = null!;
        }

        public class AppSettings {
            public string Name  { get; set; } = null!;
            public string Tier  { get; set; } = null!;
            public string Track { get; set; } = null!;
            public int    Port  { get; set; }
        }

        public class GitLabSettings {
            public string  App        { get; set; } = "";
            public string  Env        { get; set; } = "development";
            public string  EnvName    { get; set; } = "development";
            public string? EnvUrl     { get; set; }
            public string  Visibility { get; set; } = "";
        }

        public class RegistrySettings {
            public string Server   { get; set; }
            public string User     { get; set; }
            public string Password { get; set; }
            public string Email    { get; set; }
        }

        public class ServiceSettings {
            public bool   Enabled      { get; set; }
            public string Type         { get; set; } = null!;
            public int    ExternalPort { get; set; }
        }

        public class TlsSettings {
            public bool    Enabled    { get; set; }
            public string? SecretName { get; set; }
        }

        public class IngressSettings {
            public bool         Enabled { get; set; }
            public string       Class   { get; set; } = "nginx";
            public TlsSettings? Tls     { get; set; }
        }

        public class PrometheusSettings {
            public bool   Metrics { get; set; }
            public string Path    { get; set; }
        }

        public class EnvVar {
            public string Name  { get; set; } = null!;
            public string Value { get; set; } = null!;
        }
    }
}
