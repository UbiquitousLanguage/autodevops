// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using System.Collections.Immutable;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Outputs
{

    [OutputType]
    public sealed class PodMonitorSpec
    {
        /// <summary>
        /// The label to use to retrieve the job name from.
        /// </summary>
        public readonly string JobLabel;
        /// <summary>
        /// Selector to select which namespaces the Endpoints objects are discovered from.
        /// </summary>
        public readonly PodMonitorSpecNamespaceSelector NamespaceSelector;
        /// <summary>
        /// A list of endpoints allowed as part of this PodMonitor.
        /// </summary>
        public readonly ImmutableArray<PodMonitorSpecPodMetricsEndpoints> PodMetricsEndpoints;
        /// <summary>
        /// PodTargetLabels transfers labels on the Kubernetes Pod onto the target.
        /// </summary>
        public readonly ImmutableArray<string> PodTargetLabels;
        /// <summary>
        /// SampleLimit defines per-scrape limit on number of scraped samples that will be accepted.
        /// </summary>
        public readonly int SampleLimit;
        /// <summary>
        /// Selector to select Pod objects.
        /// </summary>
        public readonly PodMonitorSpecSelector Selector;
        /// <summary>
        /// TargetLimit defines a limit on the number of scraped targets that will be accepted.
        /// </summary>
        public readonly int TargetLimit;

        [OutputConstructor]
        private PodMonitorSpec(
            string jobLabel,

            PodMonitorSpecNamespaceSelector namespaceSelector,

            ImmutableArray<PodMonitorSpecPodMetricsEndpoints> podMetricsEndpoints,

            ImmutableArray<string> podTargetLabels,

            int sampleLimit,

            PodMonitorSpecSelector selector,

            int targetLimit)
        {
            JobLabel = jobLabel;
            NamespaceSelector = namespaceSelector;
            PodMetricsEndpoints = podMetricsEndpoints;
            PodTargetLabels = podTargetLabels;
            SampleLimit = sampleLimit;
            Selector = selector;
            TargetLimit = targetLimit;
        }
    }
}
