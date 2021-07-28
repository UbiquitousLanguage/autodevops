// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using System.Collections.Immutable;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.Prometheus.Monitoring.V1.Inputs
{

    /// <summary>
    /// PodMetricsEndpoint defines a scrapeable endpoint of a Kubernetes Pod serving Prometheus metrics.
    /// </summary>
    public class PodMonitorSpecPodMetricsEndpointsArgs : Pulumi.ResourceArgs
    {
        /// <summary>
        /// BasicAuth allow an endpoint to authenticate over basic authentication. More info: https://prometheus.io/docs/operating/configuration/#endpoint
        /// </summary>
        [Input("basicAuth")]
        public Input<PodMonitorSpecPodMetricsEndpointsBasicAuthArgs>? BasicAuth { get; set; }

        /// <summary>
        /// Secret to mount to read bearer token for scraping targets. The secret needs to be in the same namespace as the pod monitor and accessible by the Prometheus Operator.
        /// </summary>
        [Input("bearerTokenSecret")]
        public Input<PodMonitorSpecPodMetricsEndpointsBearerTokenSecretArgs>? BearerTokenSecret { get; set; }

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
        private InputList<PodMonitorSpecPodMetricsEndpointsMetricRelabelingsArgs>? _metricRelabelings;

        /// <summary>
        /// MetricRelabelConfigs to apply to samples before ingestion.
        /// </summary>
        public InputList<PodMonitorSpecPodMetricsEndpointsMetricRelabelingsArgs> MetricRelabelings
        {
            get => _metricRelabelings ?? (_metricRelabelings = new InputList<PodMonitorSpecPodMetricsEndpointsMetricRelabelingsArgs>());
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
        /// Name of the pod port this endpoint refers to. Mutually exclusive with targetPort.
        /// </summary>
        [Input("port")]
        public Input<string>? Port { get; set; }

        /// <summary>
        /// ProxyURL eg http://proxyserver:2195 Directs scrapes to proxy through this endpoint.
        /// </summary>
        [Input("proxyUrl")]
        public Input<string>? ProxyUrl { get; set; }

        [Input("relabelings")]
        private InputList<PodMonitorSpecPodMetricsEndpointsRelabelingsArgs>? _relabelings;

        /// <summary>
        /// RelabelConfigs to apply to samples before ingestion. More info: https://prometheus.io/docs/prometheus/latest/configuration/configuration/#relabel_config
        /// </summary>
        public InputList<PodMonitorSpecPodMetricsEndpointsRelabelingsArgs> Relabelings
        {
            get => _relabelings ?? (_relabelings = new InputList<PodMonitorSpecPodMetricsEndpointsRelabelingsArgs>());
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
        /// Deprecated: Use 'port' instead.
        /// </summary>
        [Input("targetPort")]
        public Input<PodMonitorSpecPodMetricsEndpointsTargetPortArgs>? TargetPort { get; set; }

        /// <summary>
        /// TLS configuration to use when scraping the endpoint.
        /// </summary>
        [Input("tlsConfig")]
        public Input<PodMonitorSpecPodMetricsEndpointsTlsConfigArgs>? TlsConfig { get; set; }

        public PodMonitorSpecPodMetricsEndpointsArgs()
        {
        }
    }
}