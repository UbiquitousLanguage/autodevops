using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    public static class KubeDeployment {
        public static Deployment Create(
            Output<string>              namespaceName,
            AutoDevOpsSettings          settings,
            int                         replicas,
            Secret?                     imagePullSecret     = null,
            Secret?                     appSecret           = null,
            IEnumerable<ContainerArgs>? sidecars            = null,
            Action<ContainerArgs>?      configureContainer  = null,
            Action<PodSpecArgs>?        configurePod        = null,
            Action<DeploymentArgs>?     configureDeployment = null,
            ProviderResource?           providerResource    = null
        ) {
            var appLabels = settings
                .BaseLabels()
                .AddPair("track", settings.Application.Track)
                .AddPair("tier", settings.Application.Tier)
                .AddPair("version", settings.Application.Version);

            var gitLabAnnotations = settings.GitLabAnnotations();

            var containers = CreateArgs.GetAppContainers(settings, appSecret, sidecars, configureContainer);

            var podSpec = new PodSpecArgs {
                ImagePullSecrets = CreateArgs.ImagePullSecrets(imagePullSecret?.Metadata.Apply(x => x.Name)),
                Containers = containers,
                TerminationGracePeriodSeconds = 60
            };
            configurePod?.Invoke(podSpec);

            var deployment = new DeploymentArgs {
                Metadata = CreateArgs.GetMeta(settings.FullName(), namespaceName, gitLabAnnotations, appLabels),
                Spec = new DeploymentSpecArgs {
                    Selector = new LabelSelectorArgs {MatchLabels = appLabels},
                    Replicas = replicas,
                    Template = new PodTemplateSpecArgs {
                        Metadata = new ObjectMetaArgs {
                            Labels      = appLabels,
                            Annotations = gitLabAnnotations,
                            Namespace   = namespaceName
                        },
                        Spec = podSpec
                    }
                }
            };
            configureDeployment?.Invoke(deployment);

            return new Deployment(
                settings.PulumiName("deployment"),
                deployment,
                new CustomResourceOptions {Provider = providerResource}
            );
        }
    }
}