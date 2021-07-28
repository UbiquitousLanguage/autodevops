using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.ApiExtensions;
using CustomResource = Pulumi.Kubernetes.ApiExtensions.CustomResource;

namespace Ubiquitous.AutoDevOps.Crds.Traefik {
    [PublicAPI]
    public class MiddlewareSpecStripPrefixArgs : ResourceArgs {
        [Input("prefixes", true)]
        InputList<string>? Prefixes { get; set; }
    }
    
    [PublicAPI]
    public class MiddlewareSpecArgs : ResourceArgs {
        [Input("stripPrefix", true)]
        public Input<MiddlewareSpecStripPrefixArgs>? StripPrefix { get; set; }
    }
    
    [PublicAPI]
    public class MiddlewareArgs : CustomResourceArgs {
        public MiddlewareArgs() : base("traefik.containo.us/v1alpha1", "Middleware") { }
    }
    
    [PublicAPI]
    public class Middleware : CustomResource {
        public Middleware(string name, MiddlewareArgs args, CustomResourceOptions? options = null) : base(name, args, options) { }
    }
}