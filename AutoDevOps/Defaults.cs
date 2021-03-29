using System;
using AutoDevOps.Stack;
using static System.Environment;

namespace AutoDevOps {
    static class Defaults {
        static string VarName(string var) => $"CI_{var}";

        public static string EnvVar(string var)
            => GetEnvironmentVariable(var) ??
                throw new ArgumentNullException($"Environment variable {var} must be set");

        public static string GitLabVar(string var) => EnvVar(VarName(var));

        public static string GitLabVar(string var, string alternative)
            => GetEnvironmentVariable($"CI_{var}") ??
                GetEnvironmentVariable($"CI_{alternative}") ??
                throw new ArgumentNullException($"Either {var} or {alternative} environment variable must be set");

        public static readonly string ProjectName        = GitLabVar("PROJECT_PATH");
        public static readonly string ProjectDescription = GitLabVar("PROJECT_TITLE");
        public static readonly string ProjectWebsite     = GitLabVar("PROJECT_URL");

        public static readonly string Environment = GitLabVar("ENVIRONMENT_NAME");

        public static readonly AutoDevOpsSettings.GitLabSettings GitLabSettings = new(
            GitLabVar("PROJECT_PATH_SLUG"),
            GitLabVar("ENVIRONMENT_SLUG"),
            GitLabVar("ENVIRONMENT_NAME"),
            GitLabVar("ENVIRONMENT_URL"),
            GitLabVar("PROJECT_VISIBILITY")
        );

        public static readonly AutoDevOpsSettings.RegistrySettings RegistrySettings = new(
            GitLabVar("REGISTRY"),
            GitLabVar("DEPLOY_USER", "REGISTRY_USER"),
            GitLabVar("DEPLOY_PASSWORD", "REGISTRY_PASSWORD"),
            GitLabVar("USER_EMAIL")
        );
    }
}