using static System.Environment;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Ubiquitous.AutoDevOps {
    static class Env {
        static string EnvVar(string var) => GetEnvironmentVariable(var);

        public const string AppVersionVar = "APPLICATION_VERSION";

        public static readonly string ProjectName           = EnvVar("CI_PROJECT_PATH_SLUG");
        public static readonly string Environment           = EnvVar("CI_ENVIRONMENT_NAME");
        public static readonly string EnvironmentUrl        = EnvVar("CI_ENVIRONMENT_URL");
        public static readonly string EnvironmentSlug       = EnvVar("CI_ENVIRONMENT_SLUG");
        public static readonly string ProjectVisibility     = EnvVar("CI_PROJECT_VISIBILITY");
        public static readonly string Registry              = EnvVar("CI_REGISTRY");
        public static readonly string RegistryUser          = EnvVar("CI_REGISTRY_USER");
        public static readonly string RegistryPassword      = EnvVar("CI_REGISTRY_PASSWORD");
        public static readonly string RegistryImage         = EnvVar("CI_REGISTRY_IMAGE");
        public static readonly string DeployUser            = EnvVar("CI_DEPLOY_USER");
        public static readonly string DeployPassword        = EnvVar("CI_DEPLOY_PASSWORD");
        public static readonly string KubeNamespace         = EnvVar("KUBE_NAMESPACE");
        public static readonly string UserEmail             = EnvVar("GITLAB_USER_EMAIL");
        public static readonly string CommitTag             = EnvVar("CI_COMMIT_TAG");
        public static readonly string CommitSha             = EnvVar("CI_COMMIT_SHA");
        public static readonly string CommitRefSlug         = EnvVar("CI_COMMIT_REF_SLUG");
        public static readonly string ApplicationRepository = EnvVar("CI_APPLICATION_REPOSITORY");
        public static readonly string ApplicationTag        = EnvVar("CI_APPLICATION_TAG");
        public static readonly string ApplicationVersion    = EnvVar(AppVersionVar);

        public static readonly string DeployRegistryUser     = DeployUser ?? RegistryUser;
        public static readonly string DeployRegistryPassword = DeployPassword ?? RegistryPassword;

        public static readonly string ImageRepository = ApplicationRepository ??
            (CommitTag != null ? RegistryImage : $"{RegistryImage}/{CommitRefSlug}");

        public static string ImageTag() => ApplicationTag ?? CommitTag ?? CommitSha;
    }
}