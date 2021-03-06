// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using System.Collections.Immutable;
using JetBrains.Annotations;
using Pulumi;
using Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs;
using Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Outputs;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1 {
    /// <summary>
    /// PodMonitor defines monitoring for a set of pods.
    /// </summary>
    [PublicAPI]
    public partial class PodMonitor : KubernetesResource {
        [Output("apiVersion")]
        public Output<string> ApiVersion { get; private set; } = null!;

        [Output("kind")]
        public Output<string> Kind { get; private set; } = null!;

        [Output("metadata")]
        public Output<Pulumi.Kubernetes.Types.Outputs.Meta.V1.ObjectMeta> Metadata { get; private set; } = null!;

        /// <summary>
        /// Specification of desired Pod selection for target discovery by Prometheus.
        /// </summary>
        [Output("spec")]
        public Output<PodMonitorSpec> Spec { get; private set; } = null!;

        /// <summary>
        /// Create a PodMonitor resource with the given unique name, arguments, and options.
        /// </summary>
        ///
        /// <param name="name">The unique name of the resource</param>
        /// <param name="args">The arguments used to populate this resource's properties</param>
        /// <param name="options">A bag of options that control this resource's behavior</param>
        public PodMonitor(string name, PodMonitorArgs? args = null, CustomResourceOptions? options = null)
            : base(
                "kubernetes:monitoring.coreos.com/v1:PodMonitor",
                name,
                MakeArgs(args),
                MakeResourceOptions(options, "")
            ) { }

        internal PodMonitor(
            string name, ImmutableDictionary<string, object?> dictionary, CustomResourceOptions? options = null
        )
            : base(
                "kubernetes:monitoring.coreos.com/v1:PodMonitor",
                name,
                new DictionaryResourceArgs(dictionary),
                MakeResourceOptions(options, "")
            ) { }

        PodMonitor(string name, Input<string> id, CustomResourceOptions? options = null)
            : base("kubernetes:monitoring.coreos.com/v1:PodMonitor", name, null, MakeResourceOptions(options, id)) { }

        static PodMonitorArgs MakeArgs(PodMonitorArgs? args) {
            args            ??= new PodMonitorArgs();
            args.ApiVersion =   "monitoring.coreos.com/v1";
            args.Kind       =   "PodMonitor";
            return args;
        }

        static CustomResourceOptions MakeResourceOptions(CustomResourceOptions? options, Input<string>? id) {
            var defaultOptions = new CustomResourceOptions {
                Version = Utilities.Version,
            };
            var merged = CustomResourceOptions.Merge(defaultOptions, options);
            // Override the ID if one was specified for consistency with other language SDKs.
            merged.Id = id ?? merged.Id;
            return merged;
        }

        /// <summary>
        /// Get an existing PodMonitor resource's state with the given name, ID, and optional extra
        /// properties used to qualify the lookup.
        /// </summary>
        ///
        /// <param name="name">The unique name of the resulting resource.</param>
        /// <param name="id">The unique provider ID of the resource to lookup.</param>
        /// <param name="options">A bag of options that control this resource's behavior</param>
        public static PodMonitor Get(string name, Input<string> id, CustomResourceOptions? options = null)
            => new(name, id, options);
    }

    public class PodMonitorArgs : ResourceArgs {
        [Input("apiVersion")]
        public Input<string>? ApiVersion { get; set; }

        [Input("kind")]
        public Input<string>? Kind { get; set; }

        [Input("metadata")]
        public Input<Pulumi.Kubernetes.Types.Inputs.Meta.V1.ObjectMetaArgs>? Metadata { get; set; }

        /// <summary>
        /// Specification of desired Pod selection for target discovery by Prometheus.
        /// </summary>
        [Input("spec")]
        public Input<PodMonitorSpecArgs>? Spec { get; set; }

        public PodMonitorArgs() { }
    }
}