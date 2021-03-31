using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Networking.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Addons;
using Ubiquitous.AutoDevOps.Stack.Resources;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;
using static System.Environment;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InvertIf

namespace Ubiquitous.AutoDevOps.Stack {
    public class AutoDevOps {
        public Result DeploymentResult { get; }

        public AutoDevOps(
            AutoDevOpsSettings          settings,
            IEnumerable<ContainerArgs>? sidecars             = null,
            Action<ContainerArgs>?      configureContainer   = null,
            Action<PodSpecArgs>?        configurePod         = null,
            Action<DeploymentArgs>?     configureDeployment  = null,
            Dictionary<string, string>? serviceAnnotations   = null,
            Dictionary<string, string>? ingressAnnotations   = null,
            Dictionary<string, string>? namespaceAnnotations = null,
            Action<Namespace>?          deployExtras         = null,
            ProviderResource?           provider             = null
        ) {
            var @namespace    = KubeNamespace.Create(settings.Deploy.Namespace, namespaceAnnotations, provider);
            var namespaceName = @namespace.Metadata.Apply(x => x.Name);
            deployExtras?.Invoke(@namespace);

            var imagePullSecret = settings.GitLab.Visibility != "public" && settings.Registry != null
                ? KubeSecret.CreateRegistrySecret(namespaceName, settings.Registry, provider)
                : null;

            var replicas = settings.Deploy.Percentage > 0 || settings.Deploy.Replicas == 0
                ? GetReplicas(settings.Application.Track, settings.Deploy.Percentage)
                : settings.Deploy.Replicas;

            if (replicas == 0) {
                DeploymentResult = new Result {Namespace = @namespace};
                return;
            }

            var appSecret = KubeSecret.CreateAppSecret(namespaceName, settings, provider);

            var deployment = KubeDeployment.Create(
                namespaceName,
                settings,
                replicas,
                imagePullSecret,
                appSecret,
                sidecars,
                configureContainer,
                configurePod,
                configureDeployment,
                provider
            );

            var service = settings.Service.Enabled
                ? KubeService.Create(namespaceName, settings, deployment, serviceAnnotations, provider)
                : null;

            var ingress = settings.Ingress.Enabled && !settings.Deploy.Url.IsEmpty()
                ? KubeIngress.Create(namespaceName, settings, settings.Ingress.Class, ingressAnnotations, provider)
                : null;

            if (settings.Prometheus.Metrics && settings.Prometheus.Operator) {
                if (service != null) {
                    Prometheus.CreateServiceMonitor(settings, service, @namespace, provider);
                }
                else {
                    Prometheus.CreatePodMonitor(settings, deployment, @namespace, provider);
                }
            }

            DeploymentResult = new Result {
                Namespace  = @namespace,
                Deployment = deployment,
                Service    = service,
                Ingress    = ingress
            };
        }

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

            var envVar = track == "stable" || track == "rollout"
                ? $"{envSlug}_REPLICAS"
                : $"{envTrack}_{envSlug}_REPLICAS";
            var envReplicas = GetEnvironmentVariable(envVar);
            var newReplicas = envReplicas.IntOr(replicas.IntOr(1));

            var rep = newReplicas * percentage / 100;
            return newReplicas == 0 ? 0 : rep > 1 ? rep : 1;
        }
    }
}