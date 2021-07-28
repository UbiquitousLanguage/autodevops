// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1Beta1.Outputs
{

    [OutputType]
    public sealed class ClusterIssuerSpecAcmeSolversDns01Cloudflare
    {
        /// <summary>
        /// API key to use to authenticate with Cloudflare. Note: using an API token to authenticate is now the recommended method as it allows greater control of permissions.
        /// </summary>
        public readonly ClusterIssuerSpecAcmeSolversDns01CloudflareApiKeySecretRef ApiKeySecretRef;
        /// <summary>
        /// API token used to authenticate with Cloudflare.
        /// </summary>
        public readonly ClusterIssuerSpecAcmeSolversDns01CloudflareApiTokenSecretRef ApiTokenSecretRef;
        /// <summary>
        /// Email of the account, only required when using API key based authentication.
        /// </summary>
        public readonly string Email;

        [OutputConstructor]
        private ClusterIssuerSpecAcmeSolversDns01Cloudflare(
            ClusterIssuerSpecAcmeSolversDns01CloudflareApiKeySecretRef apiKeySecretRef,

            ClusterIssuerSpecAcmeSolversDns01CloudflareApiTokenSecretRef apiTokenSecretRef,

            string email)
        {
            ApiKeySecretRef = apiKeySecretRef;
            ApiTokenSecretRef = apiTokenSecretRef;
            Email = email;
        }
    }
}