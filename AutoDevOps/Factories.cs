using System.Linq;
using Pulumi;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1;

namespace AutoDevOps {
    public static class CreateArgs {
        public static InputList<IngressRuleArgs> IngressRule(string hostName, string serviceName, int servicePort, string path = "/")
            => new[] {
                new IngressRuleArgs {
                    Host = hostName,
                    Http = new HTTPIngressRuleValueArgs {
                        Paths = new[] {HttpIngressPath(serviceName, servicePort, path)}
                    }
                }
            };

        public static HTTPIngressPathArgs HttpIngressPath(string serviceName, int servicePort, string path)
            => new() {
                PathType = "Prefix",
                Path = path,
                Backend = new IngressBackendArgs {
                    Service = new IngressServiceBackendArgs {
                        Name = serviceName,
                        Port = new ServiceBackendPortArgs {Number = servicePort}
                    }
                }
            };

        public static InputList<LocalObjectReferenceArgs> ImagePullSecrets(params Output<string>?[] imagePullSecrets)
            => imagePullSecrets
                .Where(x => x != null)
                .Select(x => new LocalObjectReferenceArgs {Name = x!})
                .ToArray();

        public static EnvVarArgs EnvVar(string name, string value) => new() {Name = name, Value = value};

        public static EnvVarArgs FieldFrom(string envName, string field)
            => new() {
                Name = envName, ValueFrom = new EnvVarSourceArgs {FieldRef = new ObjectFieldSelectorArgs {FieldPath = field}}
            };

        public static InputMap<string> Resource(string cpu, string memory)
            => new() {
                {"cpu", cpu},
                {"memory", memory}
            };

        public static ProbeArgs HttpProbe(string path, int port) => new() {
            HttpGet = new HTTPGetActionArgs {
                Path   = path,
                Port   = port,
                Scheme = "HTTP"
            }
        };
    }
}
