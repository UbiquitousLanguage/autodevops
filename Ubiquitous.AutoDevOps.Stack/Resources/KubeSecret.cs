using System.Collections;
using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Ubiquitous.AutoDevOps.Stack.Factories;
using static System.Environment;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

[PublicAPI]
public static class KubeSecret {
    /// <summary>
    /// Create the application secret from CI variables, which are prefixed with K8S_SECRET_
    /// </summary>
    /// <param name="namespace">Namespace, where the secret should be created</param>
    /// <param name="resourceName">Resource name</param>
    /// <param name="settings">AutoDevOps settings</param>
    /// <param name="extraData">Additional variables to add to the secret</param>
    /// <param name="providerResource">Optional: customer Kubernetes provider</param>
    /// <returns></returns>
    public static Secret? CreateAppSecret(
        Namespace                   @namespace,
        ResourceName                resourceName,
        AutoDevOpsSettings          settings,
        Dictionary<string, string>? extraData        = null,
        ProviderResource?           providerResource = null
    ) {
        var env = GetEnvironmentVariables();

        var vars = new Dictionary<string, string>();

#pragma warning disable 8605
        foreach (DictionaryEntry entry in env) {
#pragma warning restore 8605
            var key = (string)entry.Key;

            if (key.StartsWith("K8S_SECRET_") && entry.Value != null)
                vars[key.Remove(0, 11)] = (string)entry.Value;
        }

        if (settings.Env != null) {
            foreach (var (name, value) in settings.Env) {
                vars[name] = value;
            }
        }

        if (extraData != null) {
            foreach (var (key, value) in extraData) {
                vars.TryAdd(key, value);
            }
        }

        if (vars.Count == 0) return null;

        return new Secret(
            resourceName.AsPulumiName(),
            new SecretArgs {
                Metadata   = Meta.GetMeta(resourceName, @namespace.GetName()),
                Type       = "opaque",
                StringData = vars
            },
            providerResource.AsResourceOptions()
        );
    }

    /// <summary>
    /// Create a custom app secret using a string map
    /// </summary>
    /// <param name="namespace">Deployment namespace</param>
    /// <param name="resourceName">Resource name for Secret</param>
    /// <param name="variables">Environment vars</param>
    /// <param name="providerResource"></param>
    /// <returns>Kubernetes Secret resource</returns>
    public static Secret CreateAppSecret(
        Namespace         @namespace,
        ResourceName      resourceName,
        InputMap<string>  variables,
        ProviderResource? providerResource = null
    )
        => new(
            resourceName.AsPulumiName(),
            new SecretArgs {
                Metadata   = Meta.GetMeta(resourceName, @namespace.GetName()),
                Type       = "opaque",
                StringData = variables
            },
            providerResource.AsResourceOptions()
        );

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
                Data     = new InputMap<string> { { ".dockerconfigjson", content } }
            },
            providerResource.AsResourceOptions()
        );
    }
}