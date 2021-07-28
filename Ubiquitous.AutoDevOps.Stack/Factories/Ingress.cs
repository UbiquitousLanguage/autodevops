using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1;

namespace Ubiquitous.AutoDevOps.Stack.Factories {
    [PublicAPI]
    public static class Ingress {
        public static InputList<IngressRuleArgs> IngressRule(
            string hostName, Input<string> serviceName, Input<int> servicePort, string path = "/"
        )
            => new[] {
                new IngressRuleArgs {
                    Host = hostName,
                    Http = new HTTPIngressRuleValueArgs {
                        Paths = new[] {HttpIngressPath(serviceName, servicePort, path)}
                    }
                }
            };

        public static HTTPIngressPathArgs HttpIngressPath(Input<string> serviceName, Input<int> servicePort, string path)
            => new() {
                PathType = "Prefix",
                Path     = path,
                Backend = new IngressBackendArgs {
                    Service = new IngressServiceBackendArgs {
                        Name = serviceName,
                        Port = new ServiceBackendPortArgs {Number = servicePort}
                    }
                }
            };

        
    }
}