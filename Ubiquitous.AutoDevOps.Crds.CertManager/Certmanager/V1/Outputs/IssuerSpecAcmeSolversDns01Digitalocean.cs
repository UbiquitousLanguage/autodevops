// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1.Outputs
{

    [OutputType]
    public sealed class IssuerSpecAcmeSolversDns01Digitalocean
    {
        /// <summary>
        /// A reference to a specific 'key' within a Secret resource. In some instances, `key` is a required field.
        /// </summary>
        public readonly IssuerSpecAcmeSolversDns01DigitaloceanTokenSecretRef TokenSecretRef;

        [OutputConstructor]
        private IssuerSpecAcmeSolversDns01Digitalocean(IssuerSpecAcmeSolversDns01DigitaloceanTokenSecretRef tokenSecretRef)
        {
            TokenSecretRef = tokenSecretRef;
        }
    }
}
