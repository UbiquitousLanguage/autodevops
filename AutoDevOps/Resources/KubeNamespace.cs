using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

namespace AutoDevOps.Resources {
    static class KubeNamespace {
        internal static Namespace Create(string name) {
            // var existing = Namespace.Get(name);

            var x = new Namespace(
                name,
                new NamespaceArgs {
                    Metadata = new ObjectMetaArgs {
                        Name = name
                    }
                }
            );

            return x;
        }
    }
}
