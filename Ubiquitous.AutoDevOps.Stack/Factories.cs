using System;
using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1;

namespace Ubiquitous.AutoDevOps.Stack {
    public static class CreateArgs {
        public static InputList<IngressRuleArgs> IngressRule(
            string hostName, string serviceName, int servicePort, string path = "/"
        )
            => new[] {
                new IngressRuleArgs {
                    Host = hostName,
                    Http = new HTTPIngressRuleValueArgs {
                        Paths = new[] {HttpIngressPath(serviceName, servicePort, path)}
                    }
                }
            };

        public static HTTPIngressPathArgs HttpIngressPath(string serviceName, int servicePort, string path)
            => new() {
                PathType = "Prefix",
                Path     = path,
                Backend = new IngressBackendArgs {
                    Service = new IngressServiceBackendArgs {
                        Name = serviceName,
                        Port = new ServiceBackendPortArgs {Number = servicePort}
                    }
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

        public static ObjectMetaArgs GetMeta(
            string            name, Output<string> @namespace, InputMap<string>? annotations = null,
            InputMap<string>? labels = null
        )
            => new() {
                Name        = name,
                Namespace   = @namespace,
                Annotations = annotations ?? new InputMap<string>(),
                Labels      = labels ?? new InputMap<string>()
            };

        public static List<ContainerArgs> GetAppContainers(
            AutoDevOpsSettings          settings,
            Secret?                     appSecret          = null,
            IEnumerable<ContainerArgs>? sidecars           = null,
            Action<ContainerArgs>?      configureContainer = null
        ) {
            var container = new ContainerArgs {
                Name            = settings.Application.Name,
                Image           = $"{settings.Deploy.Image}:{settings.Deploy.ImageTag}",
                ImagePullPolicy = "IfNotPresent",
                Env = new[] {
                    EnvVar("ASPNETCORE_ENVIRONMENT", settings.GitLab.EnvName),
                    EnvVar("GITLAB_ENVIRONMENT_NAME", settings.GitLab.EnvName),
                    EnvVar("GITLAB_ENVIRONMENT_URL", settings.Deploy.Url)
                },
                Ports = new[] {
                    new ContainerPortArgs {Name = "web", ContainerPortValue = settings.Application.Port}
                }
            };

            if (appSecret != null) {
                container.EnvFrom = new EnvFromSourceArgs {
                    SecretRef = new SecretEnvSourceArgs {Name = appSecret.Metadata.Apply(x => x.Name)}
                };
            }

            container.WhenNotEmptyString(
                settings.Application.ReadinessProbe,
                (c, p) => c.ReadinessProbe =
                    HttpProbe(p, settings.Application.Port)
            );

            container.WhenNotEmptyString(
                settings.Application.LivenessProbe,
                (c, p) => c.LivenessProbe =
                    HttpProbe(p, settings.Application.Port)
            );

            configureContainer?.Invoke(container);

            var containers = new List<ContainerArgs> {container};
            if (sidecars != null) containers.AddRange(sidecars);

            return containers;
        }
    }
}