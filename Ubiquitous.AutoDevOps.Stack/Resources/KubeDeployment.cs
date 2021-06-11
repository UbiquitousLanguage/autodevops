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
            var appLabels         = settings.AppLabels();
            var gitLabAnnotations = settings.GitLabAnnotations();

            var containers = CreateArgs.GetAppContainers(
                settings.Application,
                settings.Deploy,
                settings.GitLab,
                appSecret,
                sidecars,
                configureContainer
            );

            var deployment = new DeploymentArgs {
                Metadata = CreateArgs.GetMeta(settings.FullName(), namespaceName, gitLabAnnotations, appLabels),
                Spec = new DeploymentSpecArgs {
                    Selector = new LabelSelectorArgs {MatchLabels = appLabels},
                    Replicas = replicas,
                    Template = CreateArgs.GetPodTemplate(
                        namespaceName,
                        containers,
                        imagePullSecret,
                        appLabels,
                        gitLabAnnotations,
                        60,
                        configurePod
                    )
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