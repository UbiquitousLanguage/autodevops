// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs
{

    /// <summary>
    /// Struct containing the CA cert to use for the targets.
    /// </summary>
    public class PodMonitorSpecPodMetricsEndpointsTlsConfigCaArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// ConfigMap containing data to use for the targets.
        /// </summary>
        [Input("configMap")]
        public Input<PodMonitorSpecPodMetricsEndpointsTlsConfigCaConfigMapArgs>? ConfigMap { get; set; }

        /// <summary>
        /// Secret containing data to use for the targets.
        /// </summary>
        [Input("secret")]
        public Input<PodMonitorSpecPodMetricsEndpointsTlsConfigCaSecretArgs>? Secret { get; set; }

        public PodMonitorSpecPodMetricsEndpointsTlsConfigCaArgs()
        {
        }
    }
}
