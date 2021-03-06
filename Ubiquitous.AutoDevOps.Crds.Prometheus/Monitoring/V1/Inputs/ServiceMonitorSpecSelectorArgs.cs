// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs
{

    /// <summary>
    /// Selector to select Endpoints objects.
    /// </summary>
    public class ServiceMonitorSpecSelectorArgs : Pulumi.ResourceArgs
    {
        [Input("matchExpressions")]
        private InputList<ServiceMonitorSpecSelectorMatchExpressionsArgs>? _matchExpressions;

        /// <summary>
        /// matchExpressions is a list of label selector requirements. The requirements are ANDed.
        /// </summary>
        public InputList<ServiceMonitorSpecSelectorMatchExpressionsArgs> MatchExpressions
        {
            get => _matchExpressions ?? (_matchExpressions = new InputList<ServiceMonitorSpecSelectorMatchExpressionsArgs>());
            set => _matchExpressions = value;
        }

        [Input("matchLabels")]
        private InputMap<string>? _matchLabels;

        /// <summary>
        /// matchLabels is a map of {key,value} pairs. A single {key,value} in the matchLabels map is equivalent to an element of matchExpressions, whose key field is "key", the operator is "In", and the values array contains only "value". The requirements are ANDed.
        /// </summary>
        public InputMap<string> MatchLabels
        {
            get => _matchLabels ?? (_matchLabels = new InputMap<string>());
            set => _matchLabels = value;
        }

        public ServiceMonitorSpecSelectorArgs()
        {
        }
    }
}
