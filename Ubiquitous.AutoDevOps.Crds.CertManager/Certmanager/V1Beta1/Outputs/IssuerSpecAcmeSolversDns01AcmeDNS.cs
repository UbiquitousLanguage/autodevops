// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1Beta1.Outputs
{

    [OutputType]
    public sealed class IssuerSpecAcmeSolversDns01AcmeDNS
    {
        /// <summary>
        /// A reference to a specific 'key' within a Secret resource. In some instances, `key` is a required field.
        /// </summary>
        public readonly IssuerSpecAcmeSolversDns01AcmeDNSAccountSecretRef AccountSecretRef;
        public readonly string Host;

        [OutputConstructor]
        private IssuerSpecAcmeSolversDns01AcmeDNS(
            IssuerSpecAcmeSolversDns01AcmeDNSAccountSecretRef accountSecretRef,

            string host)
        {
            AccountSecretRef = accountSecretRef;
            Host = host;
        }
    }
}