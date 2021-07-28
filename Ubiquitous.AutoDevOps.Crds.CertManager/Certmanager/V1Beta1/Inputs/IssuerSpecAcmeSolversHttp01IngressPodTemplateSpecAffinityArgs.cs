// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1Beta1.Inputs
{

    /// <summary>
    /// If specified, the pod's scheduling constraints
    /// </summary>
    public class IssuerSpecAcmeSolversHttp01IngressPodTemplateSpecAffinityArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// Describes node affinity scheduling rules for the pod.
        /// </summary>
        [Input("nodeAffinity")]
        public Input<IssuerSpecAcmeSolversHttp01IngressPodTemplateSpecAffinityNodeAffinityArgs>? NodeAffinity { get; set; }

        /// <summary>
        /// Describes pod affinity scheduling rules (e.g. co-locate this pod in the same node, zone, etc. as some other pod(s)).
        /// </summary>
        [Input("podAffinity")]
        public Input<IssuerSpecAcmeSolversHttp01IngressPodTemplateSpecAffinityPodAffinityArgs>? PodAffinity { get; set; }

        /// <summary>
        /// Describes pod anti-affinity scheduling rules (e.g. avoid putting this pod in the same node, zone, etc. as some other pod(s)).
        /// </summary>
        [Input("podAntiAffinity")]
        public Input<IssuerSpecAcmeSolversHttp01IngressPodTemplateSpecAffinityPodAntiAffinityArgs>? PodAntiAffinity { get; set; }

        public IssuerSpecAcmeSolversHttp01IngressPodTemplateSpecAffinityArgs()
        {
        }
    }
}