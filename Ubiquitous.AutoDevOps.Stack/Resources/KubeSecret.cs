using System.Collections;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using static System.Environment;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    public static class KubeSecret {
        /// <summary>
        /// Create the application secret from CI variables, which are prefixed with K8S_SECRET_
        /// </summary>
        /// <param name="namespaceName">Namespace, where the secret should be created</param>
        /// <param name="settings">AutoDevOps settings</param>
        /// <param name="providerResource">Optional: customer Kubernetes provider</param>
        /// <returns></returns>
        public static Secret? CreateAppSecret(
            Output<string>     namespaceName,
            AutoDevOpsSettings settings,
            ProviderResource?  providerResource = null
        ) {
            var env = GetEnvironmentVariables();

            var vars = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in env) {
                var key = (string) entry.Key;

                if (key.StartsWith("K8S_SECRET_") && entry.Value != null)
                    vars[key.Remove(0, 11)] = (string) entry.Value;
            }

            if (settings.Env != null) {
                foreach (var (name, value) in settings.Env) {
                    vars[name] = value;
                }
            }

            if (vars.Count == 0) return null;

            var secretName = $"{settings.Application.Name}-secret";

            return new Secret(
                secretName,
                new SecretArgs {
                    Metadata   = CreateArgs.GetMeta(secretName, namespaceName),
                    Type       = "opaque",
                    StringData = vars
                },
                new CustomResourceOptions {Provider = providerResource}
            );
        }

        public static Secret CreateRegistrySecret(
            Output<string>                      @namespace,
            AutoDevOpsSettings.RegistrySettings registrySettings,
            ProviderResource?                   providerResource = null
        ) {
            const string secretName = "gitlab-registry";

            var creds = $"{registrySettings.User}:{registrySettings.Password}".Base64Encode();

            var content =
                $"{{\"auths\":{{\"{registrySettings.Server}\":{{\"email\":\"{registrySettings.Email}\", \"auth\":\"{creds}\"}}}}}}"
                    .Base64Encode();

            return new Secret(
                secretName,
                new SecretArgs {
                    Metadata = CreateArgs.GetMeta(secretName, @namespace),
                    Type     = "kubernetes.io/dockerconfigjson",
                    Data     = new InputMap<string> {{".dockerconfigjson", content}}
                },
                new CustomResourceOptions {
                    Provider = providerResource
                }
            );
        }
    }
}