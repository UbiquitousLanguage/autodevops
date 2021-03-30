using Pulumi;
using Pulumi.Crds.Monitoring.V1;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Pulumi.Kubernetes.Types.Inputs.Monitoring.V1;

namespace AutoDevOps.Stack.Addons {
    public static class Prometheus {
        public static PodMonitor CreatePodMonitor(
            AutoDevOpsSettings                   settings,
            Pulumi.Kubernetes.Apps.V1.Deployment deployment,
            Namespace                            kubeNamespace,
            ProviderResource?                    providerResource = null
        )
            => new(
                settings.PulumiName("podMonitor"),
                new PodMonitorArgs {
                    Metadata = new ObjectMetaArgs {
                        Namespace = kubeNamespace.Metadata.Apply(x => x.Name),
                        Name      = settings.FullName(),
                        Labels    = new InputMap<string> {{"tier", "web"}}
                    },
                    Spec = new PodMonitorSpecArgs {
                        Selector = new PodMonitorSpecSelectorArgs {
                            MatchLabels = deployment.Spec.Apply(x => x.Selector.MatchLabels)
                        },
                        NamespaceSelector = new PodMonitorSpecNamespaceSelectorArgs {
                            MatchNames = kubeNamespace.Metadata.Apply(x => x.Name)
                        },
                        PodMetricsEndpoints = new PodMonitorSpecPodMetricsEndpointsArgs {
                            Port     = "web",
                            Path     = "/metrics",
                            Interval = "15s"
                        }
                    }
                },
                new CustomResourceOptions {
                    Provider = providerResource
                }
            );

        public static ServiceMonitor CreateServiceMonitor(
            AutoDevOpsSettings settings,
            Service            service,
            Namespace          kubeNamespace,
            ProviderResource?  providerResource = null
        )
            => new(
                settings.PulumiName("serviceMonitor"),
                new ServiceMonitorArgs {
                    Metadata = new ObjectMetaArgs {
                        Namespace = kubeNamespace.Metadata.Apply(x => x.Name),
                        Name      = settings.FullName(),
                        Labels    = new InputMap<string> {{"tier", "web"}}
                    },
                    Spec = new ServiceMonitorSpecArgs {
                        Selector = new ServiceMonitorSpecSelectorArgs {
                            MatchLabels = service.Metadata.Apply(x => x.Labels)
                        },
                        NamespaceSelector = new ServiceMonitorSpecNamespaceSelectorArgs {
                            MatchNames = kubeNamespace.Metadata.Apply(x => x.Name)
                        },
                        Endpoints = new ServiceMonitorSpecEndpointsArgs {
                            Port     = "web",
                            Path     = settings.Prometheus.Path,
                            Interval = "15s"
                        }
                    }
                },
                new CustomResourceOptions {
                    Provider = providerResource
                }
            );
    }
}