using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    public static class KubeDeployment {
        public static Deployment Create(
            Output<string>              namespaceName,
            AppSettings                 appSettings,
            DeploySettings              deploySettings,
            GitLabSettings              gitLabSettings,
            Secret?                     imagePullSecret     = null,
            Secret?                     appSecret           = null,
            IEnumerable<ContainerArgs>? sidecars            = null,
            Action<ContainerArgs>?      configureContainer  = null,
            Action<PodSpecArgs>?        configurePod        = null,
            Action<DeploymentArgs>?     configureDeployment = null,
            ProviderResource?           providerResource    = null
        ) {
            var appLabels         = Meta.AppLabels(appSettings, deploySettings);
            var gitLabAnnotations = gitLabSettings.GitLabAnnotations();

            var containers = Pods.GetAppContainers(
                appSettings,
                deploySettings,
                gitLabSettings,
                appSecret,
                sidecars,
                configureContainer
            );

            var deployment = new DeploymentArgs {
                Metadata = Meta.GetMeta(appSettings.Name, namespaceName, gitLabAnnotations, appLabels),
                Spec = new DeploymentSpecArgs {
                    Selector = new LabelSelectorArgs {MatchLabels = appLabels},
                    Replicas = deploySettings.Replicas,
                    Template = Pods.GetPodTemplate(
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
                appSettings.PulumiName("deployment"),
                deployment,
                new CustomResourceOptions {Provider = providerResource}
            );
        }
    }
}