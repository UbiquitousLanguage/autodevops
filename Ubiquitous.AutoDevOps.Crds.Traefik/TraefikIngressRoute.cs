using System;
using System.Linq;
using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;

namespace Ubiquitous.AutoDevOps.Crds.Traefik {
    [PublicAPI]
    public class TraefikIngressRoute {
        public TraefikIngressRoute(
            ResourceName   ingressRouteName,
            Uri            environmentUri,
            Service        service,
            Output<string> namespaceName,
            string         endpoint,
            bool           tls
        ) {
            InputList<string>                           endpoints   = new() {endpoint};
            InputList<IngressRouteRouteMiddlewaresArgs> middlewares = new();

            if (tls) {
                endpoints.Add($"{endpoint}-https");
                middlewares.Add(new IngressRouteRouteMiddlewaresArgs {Name = "http-to-https", Namespace = "default"});
            }

            IngressRule = new IngressRoute(
                ingressRouteName.AsPulumiName(),
                new IngressRouteArgs {
                    Metadata = Meta.GetMeta(ingressRouteName, namespaceName),
                    Spec = new IngressRouteSpecArgs {
                        EntryPoints = endpoints,
                        Routes = new[] {
                            new IngressRouteRoutesArgs {
                                Match       = $"Host(`{environmentUri.Host}`)",
                                Middlewares = middlewares,
                                Services = new[] {
                                    new IngressRouteRouteServicesArgs {
                                        Name      = service.Metadata.Apply(x => x.Name),
                                        Namespace = namespaceName,
                                        Port      = service.Spec.Apply(x => x.Ports.First().Port),
                                        Scheme    = "http"
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }

        public IngressRoute IngressRule { get; }
    }
}