using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Networking.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1;

namespace AutoDevOps.Resources {
    static class KubeIngress {
        internal static Ingress Create(
            Output<string> namespaceName, AutoDevOpsSettings settings, string ingressClass, Dictionary<string, string>? annotations
        ) {
            var ingressLabels = settings.BaseLabels();
            var tlsEnabled    = settings.Ingress.Tls?.Enabled == true;

            var ingressAnnotations = (annotations ?? new Dictionary<string, string>())
                .AsInputMap()
                .AddPair("kubernetes.io/ingress.class", ingressClass)
                .AddPairIf(tlsEnabled, "kubernetes.io/tls-acme", "true")
                .AddPairIf(
                    settings.Prometheus.Metrics && ingressClass.Contains("nginx"),
                    "nginx.ingress.kubernetes.io/server-snippet",
                    "location /metrics { deny all; }"
                );

            var uri = new Uri(settings.Deploy.Url);

            var ingress = new IngressArgs {
                Metadata = new ObjectMetaArgs {
                    Name        = settings.FullName(),
                    Namespace   = namespaceName,
                    Annotations = ingressAnnotations,
                    Labels      = ingressLabels
                },
                Spec = new IngressSpecArgs {
                    Rules = CreateArgs.IngressRule(uri.Host, settings.FullName(), settings.Service.ExternalPort),
                    Tls = tlsEnabled
                        ? new[] {
                            new IngressTLSArgs {
                                SecretName = settings.Ingress.Tls!.SecretName ?? $"{settings.Application.Name}-tls",
                                Hosts      = new[] {uri.Host}
                            }
                        }
                        : new InputList<IngressTLSArgs>()
                }
            };

            return new Ingress(settings.PulumiName("ingress"), ingress);
        }
    }
}
