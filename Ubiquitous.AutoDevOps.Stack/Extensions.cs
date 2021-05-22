using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using static System.Text.Encoding;
using Convert = System.Convert;

namespace Ubiquitous.AutoDevOps.Stack {
    public static class Extensions {
        public static InputMap<string> AsInputMap(this Dictionary<string, string>? dict) => dict ?? new Dictionary<string, string>();

        public static InputMap<string> AddPair(this InputMap<string> inputMap, string key, string? value) {
            inputMap.Add(key, value ?? "");
            return inputMap;
        }

        public static InputMap<string> AddPairIf(this InputMap<string> inputMap, bool condition, string key, string? value) {
            if (condition) inputMap.Add(key, value ?? "");
            return inputMap;
        }

        public static void WhenNotEmptyString<T>(this T self, string argument, Action<T, string> action) {
            if (!string.IsNullOrEmpty(argument)) action(self, argument);
        }

        /// <summary>
        /// Gets the default labels, which identify the app and the release
        /// </summary>
        /// <param name="settings">Settings for AutoDevOps</param>
        /// <returns>An input map with default annotations</returns>
        public static InputMap<string> BaseLabels(this AutoDevOpsSettings settings)
            => new() {
                {"app", settings.Application.Name},
                {"release", settings.Deploy.Release}
            };

        public static InputMap<string> AppLabels(this AutoDevOpsSettings settings)
            => new() {
                {"app", settings.Application.Name},
                {"release", settings.Deploy.Release},
                {"track", settings.Application.Track},
                {"tier", settings.Application.Tier},
                {"version", settings.Application.Version ?? ""}
            };

        /// <summary>
        /// Gets the default GitLab annotations for the deployment, which are needed to
        /// show the environment in the GitLab UI
        /// </summary>
        /// <param name="settings">Settings for AutoDevOps</param>
        /// <returns>An input map with GitLab annotations</returns>
        public static InputMap<string> GitLabAnnotations(this AutoDevOpsSettings settings)
            => new() {
                {"app.gitlab.com/app", settings.GitLab.App},
                {"app.gitlab.com/env", settings.GitLab.Env}
            };

        /// <summary>
        /// Add a volume to the pod configuration
        /// </summary>
        /// <param name="pod">Pod spec</param>
        /// <param name="volumes">One or more volumes</param>
        /// <returns></returns>
        public static PodSpecArgs AddVolumes(this PodSpecArgs pod, params VolumeArgs[] volumes) {
            var podVolumes = pod.Volumes;

            foreach (var volume in volumes) {
                podVolumes.Add(volume);
            }

            return pod;
        }

        public static string Or(this string? val, string alt) => string.IsNullOrEmpty(val) ? alt : val;

        public static int IntOr(this string? val, int alt) => !string.IsNullOrWhiteSpace(val) && int.TryParse(val, out var intVal) ? intVal : alt;
        
        public static string Base64Encode(this string plainText) => Convert.ToBase64String(UTF8.GetBytes(plainText));

        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);

        public static Input<T>? AsInput<T>(this T? value) {
            if (value == null) return null;

            return value;
        }
    }
}
