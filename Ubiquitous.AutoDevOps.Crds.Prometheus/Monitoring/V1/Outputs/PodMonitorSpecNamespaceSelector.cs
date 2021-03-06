// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using System.Collections.Immutable;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Outputs
{

    [OutputType]
    public sealed class PodMonitorSpecNamespaceSelector
    {
        /// <summary>
        /// Boolean describing whether all namespaces are selected in contrast to a list restricting them.
        /// </summary>
        public readonly bool Any;
        /// <summary>
        /// List of namespace names.
        /// </summary>
        public readonly ImmutableArray<string> MatchNames;

        [OutputConstructor]
        private PodMonitorSpecNamespaceSelector(
            bool any,

            ImmutableArray<string> matchNames)
        {
            Any = any;
            MatchNames = matchNames;
        }
    }
}
