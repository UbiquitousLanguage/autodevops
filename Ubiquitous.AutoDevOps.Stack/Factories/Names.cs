using static Ubiquitous.AutoDevOps.Stack.AutoDevOpsSettings;

namespace Ubiquitous.AutoDevOps.Stack.Factories {
    public static class Names {
        public static string PulumiName(this AppSettings settings, string resource) => $"{settings.Name}-{resource}";
    }
}