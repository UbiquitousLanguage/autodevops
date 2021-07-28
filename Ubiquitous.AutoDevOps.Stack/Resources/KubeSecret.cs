using System.Collections;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static System.Environment;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
    public static class KubeSecret {
        /// <summary>
        /// Create the application secret from CI variables, which are prefixed with K8S_SECRET_
        /// </summary>
        /// <param name="namespace">Namespace, where the secret should be created</param>
        /// <param name="settings">AutoDevOps settings</param>
        /// <param name="providerResource">Optional: customer Kubernetes provider</param>
        /// <returns></returns>
        public static Secret? CreateAppSecret(
            Namespace          @namespace,
            AutoDevOpsSettings settings,
            ProviderResource?  providerResource = null
        ) {
            var env = GetEnvironmentVariables();

            var vars = new Dictionary<string, string>();

#pragma warning disable 8605
            foreach (DictionaryEntry entry in env) {
#pragma warning restore 8605
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
                    Metadata   = Meta.GetMeta(secretName, @namespace.GetName()),
                    Type       = "opaque",
                    StringData = vars
                },
                new CustomResourceOptions {Provider = providerResource}
            );
        }

        /// <summary>
        /// Creates a secret for pulling images from the CI image registry
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="registrySettings"></param>
        /// <param name="providerResource"></param>
        /// <returns></returns>
        public static Secret CreateRegistrySecret(
            Namespace                           @namespace,
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
                    Metadata = Meta.GetMeta(secretName, @namespace.GetName()),
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