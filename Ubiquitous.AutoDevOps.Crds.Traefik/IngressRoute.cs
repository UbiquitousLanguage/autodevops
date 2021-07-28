using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.ApiExtensions;
using CustomResource = Pulumi.Kubernetes.ApiExtensions.CustomResource;

namespace Ubiquitous.AutoDevOps.Crds.Traefik {
    [PublicAPI]
    public class IngressRouteSpecArgs : ResourceArgs {
        [Input("entryPoints", true)]
        public InputList<string>? EntryPoints { get; set; }

        [Input("routes", true)]
        public InputList<IngressRouteRoutesArgs>? Routes { get; set; }
    }

    [PublicAPI]
    public class IngressRouteRouteMiddlewaresArgs : ResourceArgs {
        [Input("name", true)]
        public Input<string>? Name { get; set; }
        
        [Input("namespace")]
        public Input<string>? Namespace { get; set; }
    }

    [PublicAPI]
    public class IngressRouteRouteServicesArgs : ResourceArgs {
        [Input("kind", true)]
        public Input<string>? Kind { get; set; } = "Service";
        
        [Input("name", true)]
        public Input<string>? Name { get; set; }
        
        [Input("namespace")]
        public Input<string>? Namespace { get; set; }
        
        [Input("port", true)]
        public Input<int>? Port { get; set; }
        
        [Input("scheme")]
        public Input<string>? Scheme { get; set; }
        
        [Input("strategy")]
        public Input<string>? Strategy { get; set; }
        
        [Input("serversTransport")]
        public Input<string>? ServersTransport { get; set; }
    }

    [PublicAPI]
    public class IngressRouteRoutesArgs : ResourceArgs {
        [Input("kind", true)]
        public Input<string>? Kind { get; set; } = "Rule";
        
        [Input("match", true)]
        public Input<string>? Match { get; set; }
        
        [Input("priority")]
        public Input<int>? Priority { get; set; }

        [Input("middlewares")]
        public InputList<IngressRouteRouteMiddlewaresArgs>? Middlewares { get; set; }
        
        [Input("services", true)]
        public InputList<IngressRouteRouteServicesArgs>? Services { get; set; }
    }

    [PublicAPI]
    public class IngressRouteArgs : CustomResourceArgs {
        [Input("spec")]
        public Input<IngressRouteSpecArgs>? Spec { get; set; }

        public IngressRouteArgs() : base("traefik.containo.us/v1alpha1", "IngressRoute") { }
    }

    [PublicAPI]
    public class IngressRoute : CustomResource {
        public IngressRoute(string name, IngressRouteArgs args, CustomResourceOptions? options = null) : base(name, args, options) { }
    }
}