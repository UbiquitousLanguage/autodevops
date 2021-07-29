using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1;
using Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Addons {
    [PublicAPI]
    public static class Prometheus {
        public static PodMonitor CreatePodMonitor(
            ResourceName                         resourceName,
            Pulumi.Kubernetes.Apps.V1.Deployment deployment,
            Namespace                            kubeNamespace,
            ProviderResource?                    providerResource = null
        )
            => new(
                resourceName.AsPulumiName(),
                new PodMonitorArgs {
                    Metadata = new ObjectMetaArgs {
                        Namespace = kubeNamespace.Metadata.Apply(x => x.Name),
                        Name      = resourceName,
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
            ResourceName       resourceName,
            PrometheusSettings prometheusSettings,
            Service            service,
            Namespace          kubeNamespace,
            ProviderResource?  providerResource = null
        )
            => new(
                resourceName.AsPulumiName(),
                new ServiceMonitorArgs {
                    Metadata = new ObjectMetaArgs {
                        Namespace = kubeNamespace.Metadata.Apply(x => x.Name),
                        Name      = resourceName,
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
                            Path     = prometheusSettings.Path,
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