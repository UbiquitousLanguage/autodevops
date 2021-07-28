// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Certmanager.V1.Outputs
{

    [OutputType]
    public sealed class ClusterIssuerSpecVault
    {
        /// <summary>
        /// Auth configures how cert-manager authenticates with the Vault server.
        /// </summary>
        public readonly ClusterIssuerSpecVaultAuth Auth;
        /// <summary>
        /// PEM encoded CA bundle used to validate Vault server certificate. Only used if the Server URL is using HTTPS protocol. This parameter is ignored for plain HTTP protocol connection. If not set the system root certificates are used to validate the TLS connection.
        /// </summary>
        public readonly string CaBundle;
        /// <summary>
        /// Name of the vault namespace. Namespaces is a set of features within Vault Enterprise that allows Vault environments to support Secure Multi-tenancy. e.g: "ns1" More about namespaces can be found here https://www.vaultproject.io/docs/enterprise/namespaces
        /// </summary>
        public readonly string Namespace;
        /// <summary>
        /// Path is the mount path of the Vault PKI backend's `sign` endpoint, e.g: "my_pki_mount/sign/my-role-name".
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// Server is the connection address for the Vault server, e.g: "https://vault.example.com:8200".
        /// </summary>
        public readonly string Server;

        [OutputConstructor]
        private ClusterIssuerSpecVault(
            ClusterIssuerSpecVaultAuth auth,

            string caBundle,

            string @namespace,

            string path,

            string server)
        {
            Auth = auth;
            CaBundle = caBundle;
            Namespace = @namespace;
            Path = path;
            Server = server;
        }
    }
}