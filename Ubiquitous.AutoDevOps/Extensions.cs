using System.Text.Json;
using System.Threading.Tasks;
using Pulumi.Automation;

namespace Ubiquitous.AutoDevOps {
    public static class Extensions {
        public static Task SetJsonConfig(this WorkspaceStack stack, string key, object config, bool secure = false)
            => stack.SetConfigValueAsync(key, new ConfigValue(JsonSerializer.Serialize(config), secure));
    }
}