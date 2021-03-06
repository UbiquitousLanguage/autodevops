// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using System.Collections.Immutable;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs
{

    /// <summary>
    /// Endpoint defines a scrapeable endpoint serving Prometheus metrics.
    /// </summary>
    public class ServiceMonitorSpecEndpointsArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// BasicAuth allow an endpoint to authenticate over basic authentication More info: https://prometheus.io/docs/operating/configuration/#endpoints
        /// </summary>
        [Input("basicAuth")]
        public Input<ServiceMonitorSpecEndpointsBasicAuthArgs>? BasicAuth { get; set; }

        /// <summary>
        /// File to read bearer token for scraping targets.
        /// </summary>
        [Input("bearerTokenFile")]
        public Input<string>? BearerTokenFile { get; set; }

        /// <summary>
        /// Secret to mount to read bearer token for scraping targets. The secret needs to be in the same namespace as the service monitor and accessible by the Prometheus Operator.
        /// </summary>
        [Input("bearerTokenSecret")]
        public Input<ServiceMonitorSpecEndpointsBearerTokenSecretArgs>? BearerTokenSecret { get; set; }

        /// <summary>
        /// HonorLabels chooses the metric's labels on collisions with target labels.
        /// </summary>
        [Input("honorLabels")]
        public Input<bool>? HonorLabels { get; set; }

        /// <summary>
        /// HonorTimestamps controls whether Prometheus respects the timestamps present in scraped data.
        /// </summary>
        [Input("honorTimestamps")]
        public Input<bool>? HonorTimestamps { get; set; }

        /// <summary>
        /// Interval at which metrics should be scraped
        /// </summary>
        [Input("interval")]
        public Input<string>? Interval { get; set; }

        [Input("metricRelabelings")]
        private InputList<ServiceMonitorSpecEndpointsMetricRelabelingsArgs>? _metricRelabelings;

        /// <summary>
        /// MetricRelabelConfigs to apply to samples before ingestion.
        /// </summary>
        public InputList<ServiceMonitorSpecEndpointsMetricRelabelingsArgs> MetricRelabelings
        {
            get => _metricRelabelings ?? (_metricRelabelings = new InputList<ServiceMonitorSpecEndpointsMetricRelabelingsArgs>());
            set => _metricRelabelings = value;
        }

        [Input("params")]
        private InputMap<ImmutableArray<string>>? _params;

        /// <summary>
        /// Optional HTTP URL parameters
        /// </summary>
        public InputMap<ImmutableArray<string>> Params
        {
            get => _params ?? (_params = new InputMap<ImmutableArray<string>>());
            set => _params = value;
        }

        /// <summary>
        /// HTTP path to scrape for metrics.
        /// </summary>
        [Input("path")]
        public Input<string>? Path { get; set; }

        /// <summary>
        /// Name of the service port this endpoint refers to. Mutually exclusive with targetPort.
        /// </summary>
        [Input("port")]
        public Input<string>? Port { get; set; }

        /// <summary>
        /// ProxyURL eg http://proxyserver:2195 Directs scrapes to proxy through this endpoint.
        /// </summary>
        [Input("proxyUrl")]
        public Input<string>? ProxyUrl { get; set; }

        [Input("relabelings")]
        private InputList<ServiceMonitorSpecEndpointsRelabelingsArgs>? _relabelings;

        /// <summary>
        /// RelabelConfigs to apply to samples before scraping. More info: https://prometheus.io/docs/prometheus/latest/configuration/configuration/#relabel_config
        /// </summary>
        public InputList<ServiceMonitorSpecEndpointsRelabelingsArgs> Relabelings
        {
            get => _relabelings ?? (_relabelings = new InputList<ServiceMonitorSpecEndpointsRelabelingsArgs>());
            set => _relabelings = value;
        }

        /// <summary>
        /// HTTP scheme to use for scraping.
        /// </summary>
        [Input("scheme")]
        public Input<string>? Scheme { get; set; }

        /// <summary>
        /// Timeout after which the scrape is ended
        /// </summary>
        [Input("scrapeTimeout")]
        public Input<string>? ScrapeTimeout { get; set; }

        /// <summary>
        /// Name or number of the target port of the Pod behind the Service, the port must be specified with container port property. Mutually exclusive with port.
        /// </summary>
        [Input("targetPort")]
        public Input<ServiceMonitorSpecEndpointsTargetPortArgs>? TargetPort { get; set; }

        /// <summary>
        /// TLS configuration to use when scraping the endpoint
        /// </summary>
        [Input("tlsConfig")]
        public Input<ServiceMonitorSpecEndpointsTlsConfigArgs>? TlsConfig { get; set; }

        public ServiceMonitorSpecEndpointsArgs()
        {
        }
    }
}
