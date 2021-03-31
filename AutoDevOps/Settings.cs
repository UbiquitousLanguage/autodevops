using AutoDevOps.Stack;
using static AutoDevOps.Env;

namespace AutoDevOps {
    public static class Settings {
        public static AutoDevOpsSettings.GitLabSettings GitLabSettings()
            => new(ProjectName, EnvironmentSlug, Environment, EnvironmentUrl, ProjectVisibility);

        public static AutoDevOpsSettings.RegistrySettings RegistrySettings()
            => new(Registry, DeployRegistryUser, DeployRegistryPassword, UserEmail);

        public static AutoDevOpsSettings.DeploySettings DeploySettings(string image, string tag, int percentage)
            => new(KubeNamespace, Environment, 1, percentage, image, tag, EnvironmentUrl);
    }
}