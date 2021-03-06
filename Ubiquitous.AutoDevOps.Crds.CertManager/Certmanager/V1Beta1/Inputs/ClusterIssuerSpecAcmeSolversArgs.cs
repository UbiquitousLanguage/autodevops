// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1Beta1.Inputs
{

    /// <summary>
    /// Configures an issuer to solve challenges using the specified options. Only one of HTTP01 or DNS01 may be provided.
    /// </summary>
    public class ClusterIssuerSpecAcmeSolversArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// Configures cert-manager to attempt to complete authorizations by performing the DNS01 challenge flow.
        /// </summary>
        [Input("dns01")]
        public Input<ClusterIssuerSpecAcmeSolversDns01Args>? Dns01 { get; set; }

        /// <summary>
        /// Configures cert-manager to attempt to complete authorizations by performing the HTTP01 challenge flow. It is not possible to obtain certificates for wildcard domain names (e.g. `*.example.com`) using the HTTP01 challenge mechanism.
        /// </summary>
        [Input("http01")]
        public Input<ClusterIssuerSpecAcmeSolversHttp01Args>? Http01 { get; set; }

        /// <summary>
        /// Selector selects a set of DNSNames on the Certificate resource that should be solved using this challenge solver. If not specified, the solver will be treated as the 'default' solver with the lowest priority, i.e. if any other solver has a more specific match, it will be used instead.
        /// </summary>
        [Input("selector")]
        public Input<ClusterIssuerSpecAcmeSolversSelectorArgs>? Selector { get; set; }

        public ClusterIssuerSpecAcmeSolversArgs()
        {
        }
    }
}
