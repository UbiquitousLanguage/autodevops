using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Factories {
    [PublicAPI]
    public static class Pods {
        public static List<ContainerArgs> GetAppContainers(
            AppSettings                 appSettings,
            DeploySettings              deploySettings,
            GitLabSettings              gitLabSettings,
            Secret?                     appSecret          = null,
            IEnumerable<ContainerArgs>? sidecars           = null,
            Action<ContainerArgs>?      configureContainer = null
        ) {
            var container = new ContainerArgs {
                Name            = deploySettings.ResourceName,
                Image           = deploySettings.Image,
                ImagePullPolicy = "IfNotPresent",
                Env = new[] {
                    EnvVar("ASPNETCORE_ENVIRONMENT", gitLabSettings.EnvName),
                    EnvVar("GITLAB_ENVIRONMENT_NAME", gitLabSettings.EnvName),
                    EnvVar("GITLAB_ENVIRONMENT_URL", deploySettings.Url ?? "")
                },
                Ports = new[] {
                    new ContainerPortArgs {Name = "web", ContainerPortValue = appSettings.Port}
                }
            };

            if (appSecret != null) {
                container.EnvFrom = new EnvFromSourceArgs {
                    SecretRef = new SecretEnvSourceArgs {Name = appSecret.Metadata.Apply(x => x.Name)}
                };
            }

            container.WhenNotEmptyString(
                appSettings.ReadinessProbe,
                (c, p) => c.ReadinessProbe =
                    HttpProbe(p, appSettings.Port)
            );

            container.WhenNotEmptyString(
                appSettings.LivenessProbe,
                (c, p) => c.LivenessProbe =
                    HttpProbe(p, appSettings.Port)
            );

            configureContainer?.Invoke(container);

            var containers = new List<ContainerArgs> {container};
            if (sidecars != null) containers.AddRange(sidecars);

            return containers;
        }

        public static PodTemplateSpecArgs GetPodTemplate(
            Namespace            kubens,
            List<ContainerArgs>  containers,
            Secret?              imagePullSecret,
            InputMap<string>     labels,
            InputMap<string>     annotations,
            int                  terminationGracePeriod = 60,
            Action<PodSpecArgs>? configurePod           = null
        ) {
            var podSpec = new PodSpecArgs {
                ImagePullSecrets              = ImagePullSecrets(imagePullSecret?.Metadata.Apply(x => x.Name)),
                Containers                    = containers,
                TerminationGracePeriodSeconds = terminationGracePeriod
            };
            configurePod?.Invoke(podSpec);

            return new PodTemplateSpecArgs {
                Metadata = new ObjectMetaArgs {
                    Labels      = labels,
                    Annotations = annotations,
                    Namespace   = kubens.GetName()
                },
                Spec = podSpec
            };
        }

        /// <summary>
        /// Get the resource requests or limits as input map
        /// </summary>
        /// <param name="cpu">CPU value</param>
        /// <param name="memory">Memory value</param>
        /// <returns></returns>
        public static InputMap<string> Resource(string cpu, string memory)
            => new() {
                {"cpu", cpu},
                {"memory", memory}
            };

        /// <summary>
        /// Get an HTTP probe, which can be used for readiness or liveness 
        /// </summary>
        /// <param name="path">Probe path</param>
        /// <param name="port">Probe port</param>
        /// <returns></returns>
        public static ProbeArgs HttpProbe(string path, int port) => new() {
            HttpGet = new HTTPGetActionArgs {
                Path   = path,
                Port   = port,
                Scheme = "HTTP"
            }
        };

        public static InputList<LocalObjectReferenceArgs> ImagePullSecrets(params Output<string>?[] imagePullSecrets)
            => imagePullSecrets
                .Where(x => x != null)
                .Select(x => new LocalObjectReferenceArgs {Name = x!})
                .ToArray();

        public static EnvVarArgs EnvVar(string name, string value) => new() {Name = name, Value = value};

        public static EnvVarArgs FieldFrom(string envName, string field)
            => new() {
                Name      = envName,
                ValueFrom = new EnvVarSourceArgs {FieldRef = new ObjectFieldSelectorArgs {FieldPath = field}}
            };

        /// <summary>
        /// Add a volume to the pod configuration
        /// </summary>
        /// <param name="pod">Pod spec</param>
        /// <param name="volumes">One or more volumes</param>
        /// <returns></returns>
        public static PodSpecArgs AddVolumes(this PodSpecArgs pod, params VolumeArgs[] volumes) {
            var podVolumes = pod.Volumes;

            foreach (var volume in volumes) {
                podVolumes.Add(volume);
            }

            return pod;
        }
    }
}