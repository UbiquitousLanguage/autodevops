using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using static System.Text.Encoding;
using Convert = System.Convert;

namespace AutoDevOps.Stack {
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

        public static InputMap<string> BaseLabels(this AutoDevOpsSettings settings)
            => new() {
                {"app", settings.Application.Name},
                {"release", settings.Deploy.Release}
            };

        public static InputMap<string> GitLabAnnotations(this AutoDevOpsSettings settings)
            => new() {
                {"app.gitlab.com/app", settings.GitLab.App},
                {"app.gitlab.com/env", settings.GitLab.Env}
            };

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
    }
}
