using JetBrains.Annotations;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Factories;

[PublicAPI]
public static class Meta {
    public static ObjectMetaArgs GetMeta(
        ResourceName      name,
        Input<string>     @namespace,
        InputMap<string>? annotations = null,
        InputMap<string>? labels      = null
    )
        => new() {
            Name        = name,
            Namespace   = @namespace,
            Annotations = annotations ?? new InputMap<string>(),
            Labels      = labels ?? new InputMap<string>()
        };

    /// <summary>
    /// Gets the default labels, which identify the app and the release
    /// </summary>
    /// <param name="appSettings">Application settings</param>
    /// <param name="resourceName">Kubernetes resource name</param>
    /// <param name="release">Deployment release</param>
    /// <returns>An input map with default annotations</returns>
    public static InputMap<string> BaseLabels(AppSettings appSettings, ResourceName resourceName, string release)
        => new() {
            {"app", appSettings.Name},
            {"component", resourceName},
            {"release", release}
        };

    /// <summary>
    /// Add default labels following GitLab expectations
    /// </summary>
    /// <param name="appSettings">Application settings, used for Track, Tier and Version</param>
    /// <param name="resourceName">Kubernetes resource name</param>
    /// <param name="release">Deployment release</param>
    /// <returns></returns>
    public static InputMap<string> AppLabels(AppSettings appSettings, string resourceName, string release)
        => new() {
            {"app", appSettings.Name},
            {"component", resourceName},
            {"release", release},
            {"track", appSettings.Track},
            {"tier", appSettings.Tier},
            {"version", appSettings.Version ?? ""}
        };

    /// <summary>
    /// Gets the default GitLab annotations for the deployment, which are needed to
    /// show the environment in the GitLab UI
    /// </summary>
    /// <param name="settings">Settings for AutoDevOps</param>
    /// <returns>An input map with GitLab annotations</returns>
    public static InputMap<string> GitLabAnnotations(this GitLabSettings settings)
        => new() {
            {"app.gitlab.com/app", settings.App},
            {"app.gitlab.com/env", settings.Env}
        };

    public static Input<string> GetName(this Namespace ns) => ns.Metadata.Apply(m => m.Name);
}