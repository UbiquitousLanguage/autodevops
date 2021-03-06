using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Batch.V1Beta1;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Batch.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;
using CronJobArgs = Pulumi.Kubernetes.Types.Inputs.Batch.V1Beta1.CronJobArgs;
using CronJobSpecArgs = Pulumi.Kubernetes.Types.Inputs.Batch.V1Beta1.CronJobSpecArgs;
using JobTemplateSpecArgs = Pulumi.Kubernetes.Types.Inputs.Batch.V1Beta1.JobTemplateSpecArgs;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

[PublicAPI]
public class KubeCronJob {
    public static CronJob Create(
        Namespace              kubens,
        ResourceName           resourceName,
        AppSettings            appSettings,
        DeploySettings         deploySettings,
        GitLabSettings         gitLabSettings,
        string                 schedule,
        Secret                 imagePullSecret,
        Secret?                appSecret,
        Action<ContainerArgs>? configureContainer = null,
        Action<PodSpecArgs>?   configurePod       = null,
        ProviderResource?      providerResource   = null
    ) {
        var appLabels         = Meta.AppLabels(appSettings, resourceName, deploySettings.Release);
        var gitLabAnnotations = gitLabSettings.GitLabAnnotations();

        var containers = Pods.GetAppContainers(
            resourceName,
            appSettings,
            deploySettings,
            gitLabSettings,
            appSecret,
            configureContainer: cfg => {
                configureContainer?.Invoke(cfg);
                cfg.LivenessProbe  = null;
                cfg.ReadinessProbe = null;
            }
        );

        var podTemplate = Pods.GetPodTemplate(
            kubens,
            containers,
            imagePullSecret,
            appLabels,
            gitLabAnnotations,
            configurePod: pod => {
                configurePod?.Invoke(pod);
                pod.RestartPolicy = "OnFailure";
            }
        );

        return new CronJob(
            resourceName.AsPulumiName(),
            new CronJobArgs {
                Metadata = Meta.GetMeta(resourceName, kubens.GetName(), gitLabAnnotations, appLabels),
                Spec = new CronJobSpecArgs {
                    Schedule          = schedule,
                    ConcurrencyPolicy = "Forbid",
                    JobTemplate = new JobTemplateSpecArgs {
                        Spec = new JobSpecArgs { Template = podTemplate }
                    }
                }
            },
            providerResource.AsResourceOptions()
        );
    }
}