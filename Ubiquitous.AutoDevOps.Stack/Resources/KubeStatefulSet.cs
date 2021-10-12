using System;
using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Apps.V1;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

[PublicAPI]
public static class KubeStatefulSet {
    public static StatefulSet Create(
        Namespace                   kubens,
        ResourceName                resourceName,
        AppSettings                 appSettings,
        DeploySettings              deploySettings,
        GitLabSettings              gitLabSettings,
        Secret?                     imagePullSecret      = null,
        Secret?                     appSecret            = null,
        IEnumerable<ContainerArgs>? sidecars             = null,
        Action<ContainerArgs>?      configureContainer   = null,
        Action<PodSpecArgs>?        configurePod         = null,
        Action<StatefulSetArgs>?    configureStatefulSet = null,
        ProviderResource?           providerResource     = null
    ) {
        if (deploySettings.StatefulSetService.IsEmpty())
            throw new ArgumentNullException(
                nameof(deploySettings.StatefulSetService),
                "Stateful set must have service name"
            );

        var appLabels         = Meta.AppLabels(appSettings, resourceName, deploySettings.Release);
        var gitLabAnnotations = gitLabSettings.GitLabAnnotations();

        var containers = Pods.GetAppContainers(
            resourceName,
            appSettings,
            deploySettings,
            gitLabSettings,
            appSecret,
            sidecars,
            configureContainer
        );

        var statefulSetArgs = new StatefulSetArgs {
            Metadata = Meta.GetMeta(resourceName, kubens.GetName(), gitLabAnnotations, appLabels),
            Spec = new StatefulSetSpecArgs {
                Selector = new LabelSelectorArgs { MatchLabels = appLabels },
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
                ServiceName = deploySettings.StatefulSetService!
            }
        };
        configureStatefulSet?.Invoke(statefulSetArgs);

        return new StatefulSet(
            resourceName.AsPulumiName(),
            statefulSetArgs,
            new CustomResourceOptions { Provider = providerResource }
        );
    }
}