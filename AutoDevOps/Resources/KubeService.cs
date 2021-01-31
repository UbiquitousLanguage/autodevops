using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

namespace AutoDevOps.Resources {
    static class KubeService {
        internal static Service Create(
            Output<string>              namespaceName,
            AutoDevOpsSettings          settings,
            Dictionary<string, string>? annotations
        ) {
            var serviceLabels = settings.BaseLabels();

            var serviceAnnotations = (annotations ?? new Dictionary<string, string>())
                .AsInputMap()
                .AddPairIf(settings.Prometheus.Metrics, "prometheus.io/scrape", "true")
                .AddPairIf(settings.Prometheus.Metrics, "prometheus.io/port", settings.Service.ExternalPort.ToString());

            return new Service(
                settings.PulumiName("service"),
                new ServiceArgs {
                    Metadata = new ObjectMetaArgs {
                        Name        = settings.FullName(),
                        Namespace   = namespaceName,
                        Annotations = serviceAnnotations,
                        Labels      = serviceLabels
                    },
                    Spec = new ServiceSpecArgs {
                        Type = settings.Service.Type,
                        Ports = new[] {
                            new ServicePortArgs {
                                Name       = "web",
                                Port       = settings.Service.ExternalPort,
                                TargetPort = settings.Application.Port,
                                Protocol   = "TCP"
                            }
                        },
                        Selector = new InputMap<string> {
                            {"app", settings.Application.Name},
                            {"tier", settings.Application.Tier}
                        }
                    }
                }
            );
        }
    }
}
