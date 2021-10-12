using System.Text.Json;
using Pulumi.Automation;

namespace Ubiquitous.AutoDevOps; 

public static class Extensions {
    public static Task SetJsonConfig(this WorkspaceStack stack, string key, object config, bool secure = false)
        => stack.SetConfigAsync(key, new ConfigValue(JsonSerializer.Serialize(config), secure));

    public static Task SetGitLabSettings(this WorkspaceStack stack)
        => stack.SetJsonConfig("gitlab", Settings.GitLabSettings());

    public static Task SetRegistrySettings(this WorkspaceStack stack)
        => stack.SetJsonConfig("registry", Settings.RegistrySettings(), true);
}