using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Addons;
using Ubiquitous.AutoDevOps.Stack.Factories;
using Ubiquitous.AutoDevOps.Stack.Resources;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;
using static System.Environment;
using Ingress = Pulumi.Kubernetes.Networking.V1.Ingress;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InvertIf

namespace Ubiquitous.AutoDevOps.Stack;

/// <summary>
/// A set of Kubernetes resources, which closely replicate what
/// GitLab AutoDevOps deployment produces by the Helm release
/// </summary>
public class AutoDevOps {
    /// <summary>
    /// Deployment result, which contains the deployed resources
    /// </summary>
    public Result DeploymentResult { get; }

    /// <summary>
    /// The AutoDevOps resource constructor, which takes care of deploying the whole thing
    /// </summary>
    /// <param name="settings">AutoDevOps settings, <see cref="AutoDevOpsSettings"/></param>
    /// <param name="sidecars">Optional: collection of sidecar containers</param>
    /// <param name="configureContainer">Optional: custom application container configuration</param>
    /// <param name="configurePod">Optional: custom pod configuration</param>
    /// <param name="configureDeployment">Optional: custom deployment configuration</param>
    /// <param name="configureService">Optional: custom service configuration</param>
    /// <param name="serviceAnnotations">Optional: service annotations</param>
    /// <param name="ingressAnnotations">Optional: ingress annotations</param>
    /// <param name="namespaceAnnotations">Optional: namespace annotations</param>
    /// <param name="deployExtras">Optional: use this to deploy other resources to the environment namespace,
    /// before the application is deployed.</param>
    /// <param name="extraAppVars">Additional data for the application secret</param>
    /// <param name="provider">Optional: custom Kubernetes resource provider</param>
    public AutoDevOps(
        AutoDevOpsSettings          settings,
        IEnumerable<ContainerArgs>? sidecars             = null,
        Action<ContainerArgs>?      configureContainer   = null,
        Action<PodSpecArgs>?        configurePod         = null,
        Action<DeploymentArgs>?     configureDeployment  = null,
        Action<ServiceArgs>?        configureService     = null,
        Dictionary<string, string>? serviceAnnotations   = null,
        Dictionary<string, string>? ingressAnnotations   = null,
        Dictionary<string, string>? namespaceAnnotations = null,
        Action<Namespace>?          deployExtras         = null,
        Dictionary<string, string>? extraAppVars         = null,
        ProviderResource?           provider             = null
    ) {
        var @namespace = KubeNamespace.Create(settings.Deploy.Namespace, namespaceAnnotations, provider);
        deployExtras?.Invoke(@namespace);

        var imagePullSecret = settings.GitLab.Visibility != "public" && settings.Registry != null
            ? KubeSecret.CreateRegistrySecret(@namespace, settings.Registry, provider)
            : null;

        var replicas = settings.Deploy.Percentage > 0 || settings.Deploy.Replicas == 0
            ? GetReplicas(settings.Application.Track, settings.Deploy.Percentage)
            : settings.Deploy.Replicas;

        if (replicas == 0) {
            DeploymentResult = new Result { Namespace = @namespace };
            return;
        }

        ResourceName.SetBaseName(settings.Application.Name);
        PulumiName.SetBaseName(settings.Application.Name);

        var appSecret = KubeSecret.CreateAppSecret(@namespace, "appsecret", settings, extraAppVars, provider);

        var deployment = KubeDeployment.Create(
            @namespace,
            "deployment",
            settings.Application,
            settings.Deploy with { Replicas = replicas },
            settings.GitLab,
            imagePullSecret,
            appSecret,
            sidecars,
            configureContainer,
            configurePod,
            configureDeployment,
            provider
        );

        var service = settings.Service.Enabled
            ? KubeService.Create(
                @namespace,
                "service",
                settings.Application,
                settings.Deploy,
                settings.Service,
                settings.GitLab,
                deployment,
                settings.Prometheus,
                serviceAnnotations,
                configureService,
                provider
            )
            : null;

        var ingress = settings.Ingress.Enabled && !settings.Deploy.Url!.IsEmpty() && service != null
            ? KubeIngress.Create(
                @namespace,
                "ingress",
                settings.Application,
                settings.Deploy,
                settings.Ingress,
                service,
                settings.Prometheus.Metrics,
                ingressAnnotations,
                provider
            )
            : null;

        if (settings.Prometheus.Metrics && settings.Prometheus.Operator) {
            if (service != null) {
                Prometheus.CreateServiceMonitor("service-monitor", settings.Prometheus, service, @namespace, provider);
            }
            else {
                Prometheus.CreatePodMonitor("pod-monitor", deployment, @namespace, provider);
            }
        }

        DeploymentResult = new Result {
            Namespace  = @namespace,
            Deployment = deployment,
            Service    = service,
            Ingress    = ingress
        };
    }

    /// <summary>
    /// The deployment result, which contains references to all the deployed resources.
    /// </summary>
    public class Result {
        public Deployment? Deployment { get; init; }
        public Service?    Service    { get; init; }
        public Ingress?    Ingress    { get; init; }
        public Namespace?  Namespace  { get; init; }
    }

    static int GetReplicas(string? track = null, int percentage = 100) {
        var localTrack = track.Or("stable");
        var envTrack   = localTrack.ToUpper();
        var envSlug    = GetEnvironmentVariable("CI_ENVIRONMENT_SLUG")?.ToUpper();
        var replicas   = GetEnvironmentVariable("REPLICAS");

        var envVar = track is "stable" or "rollout"
            ? $"{envSlug}_REPLICAS"
            : $"{envTrack}_{envSlug}_REPLICAS";
        var envReplicas = GetEnvironmentVariable(envVar);
        var newReplicas = envReplicas.IntOr(replicas.IntOr(1));

        var rep = newReplicas * percentage / 100;
        return newReplicas == 0 ? 0 : rep > 1 ? rep : 1;
    }
}