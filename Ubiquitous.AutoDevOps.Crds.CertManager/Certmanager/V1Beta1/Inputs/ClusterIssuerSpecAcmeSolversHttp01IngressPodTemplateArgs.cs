// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1Beta1.Inputs
{

    /// <summary>
    /// Optional pod template used to configure the ACME challenge solver pods used for HTTP01 challenges
    /// </summary>
    public class ClusterIssuerSpecAcmeSolversHttp01IngressPodTemplateArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// ObjectMeta overrides for the pod used to solve HTTP01 challenges. Only the 'labels' and 'annotations' fields may be set. If labels or annotations overlap with in-built values, the values here will override the in-built values.
        /// </summary>
        [Input("metadata")]
        public Input<ClusterIssuerSpecAcmeSolversHttp01IngressPodTemplateMetadataArgs>? Metadata { get; set; }

        /// <summary>
        /// PodSpec defines overrides for the HTTP01 challenge solver pod. Only the 'priorityClassName', 'nodeSelector', 'affinity', 'serviceAccountName' and 'tolerations' fields are supported currently. All other fields will be ignored.
        /// </summary>
        [Input("spec")]
        public Input<ClusterIssuerSpecAcmeSolversHttp01IngressPodTemplateSpecArgs>? Spec { get; set; }

        public ClusterIssuerSpecAcmeSolversHttp01IngressPodTemplateArgs()
        {
        }
    }
}