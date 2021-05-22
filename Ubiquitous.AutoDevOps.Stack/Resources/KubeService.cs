using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    public static class KubeService {
        public static Service Create(
            Output<string>                       namespaceName,
            AutoDevOpsSettings                   settings,
            Pulumi.Kubernetes.Apps.V1.Deployment deployment,
            Dictionary<string, string>?          annotations      = null,
            Action<ServiceArgs>?                 configureService = null,
            ProviderResource?                    providerResource = null
        ) {
            var selector = deployment.Spec.Apply(x => x.Selector.MatchLabels);
            return Create(namespaceName, settings, selector, annotations, configureService, providerResource);
        }

        public static Service Create(
            Output<string>                        namespaceName,
            AutoDevOpsSettings                    settings,
            Pulumi.Kubernetes.Apps.V1.StatefulSet statefulSet,
            Dictionary<string, string>?           annotations      = null,
            Action<ServiceArgs>?                  configureService = null,
            ProviderResource?                     providerResource = null
        ) {
            var selector = statefulSet.Spec.Apply(x => x.Selector.MatchLabels);
            return Create(namespaceName, settings, selector, annotations, configureService, providerResource);
        }

        static Service Create(
            Output<string>              namespaceName,
            AutoDevOpsSettings          settings,
            InputMap<string>            selector,
            Dictionary<string, string>? annotations      = null,
            Action<ServiceArgs>?        configureService = null,
            ProviderResource?           providerResource = null
        ) {
            var serviceLabels = settings.BaseLabels();

            var serviceAnnotations = (annotations ?? new Dictionary<string, string>())
                .AsInputMap();

            if (settings.Prometheus.Metrics && !settings.Prometheus.Operator) {
                serviceAnnotations
                    .AddPair("prometheus.io/scrape", "true")
                    .AddPair("prometheus.io/path", settings.Prometheus.Path)
                    .AddPair("prometheus.io/port", settings.Service.ExternalPort.ToString());
            }

            var serviceArgs =
                new ServiceArgs {
                    Metadata =
                        CreateArgs.GetMeta(settings.FullName(), namespaceName, serviceAnnotations, serviceLabels),
                    Spec = new ServiceSpecArgs {
                        Type = settings.Service.Type,
                        Ports = new List<ServicePortArgs> {
                            new() {
                                Name       = "web",
                                Port       = settings.Service.ExternalPort,
                                TargetPort = settings.Application.Port,
                                Protocol   = "TCP"
                            }
                        },
                        Selector = selector
                    }
                };
            configureService?.Invoke(serviceArgs);

            return new Service(
                settings.PulumiName("service"),
                serviceArgs,
                new CustomResourceOptions {Provider = providerResource}
            );
        }
    }
}