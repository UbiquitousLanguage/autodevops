using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;
using Ingress = Pulumi.Kubernetes.Networking.V1.Ingress;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

public static class KubeIngress {
    public static Ingress Create(
        Namespace                   kubens,
        ResourceName                resourceName,
        AppSettings                 appSettings,
        DeploySettings              deploySettings,
        IngressSettings             ingressSettings,
        Service                     service,
        bool                        metricsEnabled,
        Dictionary<string, string>? annotations      = null,
        ProviderResource?           providerResource = null
    ) {
        var ingressLabels = Meta.BaseLabels(appSettings, resourceName, deploySettings.Release);
        var tlsEnabled    = ingressSettings.Tls?.Enabled == true;

        var ingressAnnotations = (annotations ?? new Dictionary<string, string>())
            .AsInputMap()
            .AddPair("kubernetes.io/ingress.class", ingressSettings.Class)
            .AddPairIf(tlsEnabled, "kubernetes.io/tls-acme", "true")
            .AddPairIf(
                metricsEnabled && ingressSettings.Class.Contains("nginx"),
                "nginx.ingress.kubernetes.io/server-snippet",
                "location /metrics { deny all; }"
            );

        var uri = new Uri(deploySettings.Url!);

        var ingress = new IngressArgs {
            Metadata = Meta.GetMeta(
                resourceName,
                kubens.GetName(),
                ingressAnnotations,
                ingressLabels
            ),
            Spec = new IngressSpecArgs {
                Rules = Factories.Ingress.IngressRule(
                    uri.Host,
                    service.Metadata.Apply(x => x.Name),
                    service.Spec.Apply(x => x.Ports[0].Port)
                ),
                Tls = tlsEnabled
                    ? new[] {
                        new IngressTLSArgs {
                            SecretName = ingressSettings.Tls!.SecretName ?? $"{resourceName}-tls",
                            Hosts      = new[] { uri.Host }
                        }
                    }
                    : new InputList<IngressTLSArgs>()
            }
        };

        return new Ingress(resourceName.AsPulumiName(), ingress, providerResource.AsResourceOptions());
    }
}