using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

[PublicAPI]
public static class KubeService {
    public static Service Create(
        Namespace                            kubens,
        ResourceName                         resourceName,
        AppSettings                          appSettings,
        DeploySettings                       deploySettings,
        ServiceSettings                      serviceSettings,
        GitLabSettings                       gitLabSettings,
        Pulumi.Kubernetes.Apps.V1.Deployment deployment,
        PrometheusSettings?                  prometheusSettings = null,
        Dictionary<string, string>?          annotations        = null,
        Action<ServiceArgs>?                 configureService   = null,
        ProviderResource?                    providerResource   = null
    ) {
        var selector = deployment.Spec.Apply(x => x.Selector.MatchLabels);

        return Create(
            kubens,
            resourceName,
            appSettings,
            deploySettings,
            serviceSettings,
            gitLabSettings,
            prometheusSettings,
            selector,
            annotations,
            configureService,
            providerResource
        );
    }

    public static Service Create(
        Namespace                             kubens,
        ResourceName                          resourceName,
        AppSettings                           appSettings,
        DeploySettings                        deploySettings,
        ServiceSettings                       serviceSettings,
        GitLabSettings                        gitLabSettings,
        Pulumi.Kubernetes.Apps.V1.StatefulSet statefulSet,
        PrometheusSettings?                   prometheusSettings = null,
        Dictionary<string, string>?           annotations        = null,
        Action<ServiceArgs>?                  configureService   = null,
        ProviderResource?                     providerResource   = null
    ) {
        var selector = statefulSet.Spec.Apply(x => x.Selector.MatchLabels);

        return Create(
            kubens,
            resourceName,
            appSettings,
            deploySettings,
            serviceSettings,
            gitLabSettings,
            prometheusSettings,
            selector,
            annotations,
            configureService,
            providerResource
        );
    }

    public static Service Create(
        Namespace                   kubens,
        ResourceName                resourceName,
        AppSettings                 appSettings,
        DeploySettings              deploySettings,
        ServiceSettings             serviceSettings,
        GitLabSettings              gitLabSettings,
        PrometheusSettings?         prometheusSettings,
        InputMap<string>            selector,
        Dictionary<string, string>? annotations      = null,
        Action<ServiceArgs>?        configureService = null,
        ProviderResource?           providerResource = null
    ) {
        var serviceLabels = Meta.BaseLabels(appSettings, resourceName, deploySettings.Release);

        var serviceAnnotations = (annotations ?? new Dictionary<string, string>())
            .AsInputMap();

        if (prometheusSettings != null && prometheusSettings.Metrics && !prometheusSettings.Operator) {
            serviceAnnotations
                .AddPair("prometheus.io/scrape", "true")
                .AddPair("prometheus.io/path", prometheusSettings.Path)
                .AddPair("prometheus.io/port", serviceSettings.ExternalPort.ToString());
        }

        var serviceArgs =
            new ServiceArgs {
                Metadata =
                    Meta.GetMeta(
                        resourceName,
                        kubens.GetName(),
                        serviceAnnotations,
                        serviceLabels
                    ),
                Spec = new ServiceSpecArgs {
                    Type = serviceSettings.Type,
                    Ports = new List<ServicePortArgs> {
                        new() {
                            Name       = serviceSettings.PortName,
                            Port       = serviceSettings.ExternalPort,
                            TargetPort = appSettings.Port,
                            Protocol   = serviceSettings.Protocol
                        }
                    },
                    Selector = selector
                }
            };
        configureService?.Invoke(serviceArgs);

        return new Service(resourceName.AsPulumiName(), serviceArgs, providerResource.AsResourceOptions());
    }
}