using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Rbac.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Pulumi.Kubernetes.Types.Inputs.Rbac.V1;

namespace AutoDevOps.Addons {
    public class Jaeger {
        public static RoleBinding AddJaeger(
            Namespace kubeNamespace, 
            string jaegerNamespace = "observability",
            ProviderResource? providerResource = null)
            => new(
                "jaeger-roleBiding",
                new RoleBindingArgs {
                    Kind = "RoleBinding",
                    Metadata = new ObjectMetaArgs {
                        Name      = "jaeger-operator-binding",
                        Namespace = kubeNamespace.Metadata.Apply(x => x.Name)
                    },
                    Subjects = new[] {
                        new SubjectArgs {
                            Kind      = "ServiceAccount",
                            Name      = "jaeger-operator",
                            Namespace = jaegerNamespace
                        }
                    },
                    RoleRef = new RoleRefArgs {
                        Kind     = "Role",
                        Name     = "jaeger-operator",
                        ApiGroup = "rbac.authorization.k8s.io"
                    }
                },
                new CustomResourceOptions {
                    Provider = providerResource
                }
            );
    }
}
