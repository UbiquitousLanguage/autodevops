using System.Collections;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using static System.Environment;

namespace AutoDevOps.Resources {
    static class KubeSecret {
        internal static Secret? CreateAppSecret(Output<string> namespaceName, AutoDevOpsSettings settings) {
            var env = GetEnvironmentVariables();

            var vars = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in env) {
                var key = (string) entry.Key;

                if (key.StartsWith("K8S_SECRET_") && entry.Value != null) vars[key.Remove(0, 11)] = (string) entry.Value;
            }

            if (settings.Env != null) {
                foreach (var envVar in settings.Env) {
                    vars[envVar.Name] = envVar.Value;
                }
            }

            if (vars.Count == 0) return null;

            var secretName = $"{settings.Application.Name}-secret";

            return new Secret(
                secretName,
                new SecretArgs {
                    Metadata = new ObjectMetaArgs {
                        Name      = secretName,
                        Namespace = namespaceName
                    },
                    Type       = "opaque",
                    StringData = vars
                }
            );
        }

        internal static Secret CreateRegistrySecret(Output<string> @namespace, AutoDevOpsSettings.RegistrySettings registrySettings) {
            const string secretName = "gitlab-registry";

            var creds   = $"{registrySettings.User}:{registrySettings.Password}".Base64Encode();
            var content = $"{{\"auths\":{{\"{registrySettings.Server}\":{{\"email\":\"{registrySettings.Email}\", \"auth\":\"{creds}\"}}}}}}".Base64Encode();

            return new Secret(
                secretName,
                new SecretArgs {
                    Metadata = new ObjectMetaArgs {
                        Name      = secretName,
                        Namespace = @namespace
                    },
                    Type = "kubernetes.io/dockerconfigjson",
                    Data = new InputMap<string> {{".dockerconfigjson", content}}
                }
            );
        }
    }
}
