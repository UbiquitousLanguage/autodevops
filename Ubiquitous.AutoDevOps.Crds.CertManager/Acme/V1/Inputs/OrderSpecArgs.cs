// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Acme.V1.Inputs
{

    public class OrderSpecArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// CommonName is the common name as specified on the DER encoded CSR. If specified, this value must also be present in `dnsNames` or `ipAddresses`. This field must match the corresponding field on the DER encoded CSR.
        /// </summary>
        [Input("commonName")]
        public Input<string>? CommonName { get; set; }

        [Input("dnsNames")]
        private InputList<string>? _dnsNames;

        /// <summary>
        /// DNSNames is a list of DNS names that should be included as part of the Order validation process. This field must match the corresponding field on the DER encoded CSR.
        /// </summary>
        public InputList<string> DnsNames
        {
            get => _dnsNames ?? (_dnsNames = new InputList<string>());
            set => _dnsNames = value;
        }

        /// <summary>
        /// Duration is the duration for the not after date for the requested certificate. this is set on order creation as pe the ACME spec.
        /// </summary>
        [Input("duration")]
        public Input<string>? Duration { get; set; }

        [Input("ipAddresses")]
        private InputList<string>? _ipAddresses;

        /// <summary>
        /// IPAddresses is a list of IP addresses that should be included as part of the Order validation process. This field must match the corresponding field on the DER encoded CSR.
        /// </summary>
        public InputList<string> IpAddresses
        {
            get => _ipAddresses ?? (_ipAddresses = new InputList<string>());
            set => _ipAddresses = value;
        }

        /// <summary>
        /// IssuerRef references a properly configured ACME-type Issuer which should be used to create this Order. If the Issuer does not exist, processing will be retried. If the Issuer is not an 'ACME' Issuer, an error will be returned and the Order will be marked as failed.
        /// </summary>
        [Input("issuerRef", required: true)]
        public Input<OrderSpecIssuerRefArgs> IssuerRef { get; set; } = null!;

        /// <summary>
        /// Certificate signing request bytes in DER encoding. This will be used when finalizing the order. This field must be set on the order.
        /// </summary>
        [Input("request", required: true)]
        public Input<string> Request { get; set; } = null!;

        public OrderSpecArgs()
        {
        }
    }
}