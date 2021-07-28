using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Apps.V1;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    [PublicAPI]
    public static class KubeStatefulSet {
        public static StatefulSet Create(
            Namespace                   kubens,
            AppSettings                 appSettings,
            DeploySettings              deploySettings,
            GitLabSettings              gitLabSettings,
            Secret?                     imagePullSecret     = null,
            Secret?                     appSecret           = null,
            IEnumerable<ContainerArgs>? sidecars            = null,
            Action<ContainerArgs>?      configureContainer  = null,
            Action<PodSpecArgs>?        configurePod        = null,
            Action<StatefulSetArgs>?    configureStatefulSet = null,
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

            var statefulSetArgs = new StatefulSetArgs {
                Metadata = Meta.GetMeta(deploySettings.ResourceName, kubens.GetName(), gitLabAnnotations, appLabels),
                Spec = new StatefulSetSpecArgs {
                    Selector = new LabelSelectorArgs {MatchLabels = appLabels},
                    Replicas = deploySettings.Replicas,
                    Template = Pods.GetPodTemplate(
                        kubens,
                        containers,
                        imagePullSecret,
                        appLabels,
                        gitLabAnnotations,
                        60,
                        configurePod
                    ),
                    ServiceName = deploySettings.StatefulSetService
                }
            };
            configureStatefulSet?.Invoke(statefulSetArgs);

            return new StatefulSet(
                deploySettings.PulumiName("statefulSet"),
                statefulSetArgs,
                new CustomResourceOptions {Provider = providerResource}
            );
        }
    }
}