using System.Collections.Generic;
using System.CommandLine;

namespace Ubiquitous.AutoDevOps.Deployments {
    public class DefaultOptions : IDeploymentOptions {
        public DefaultOptions(
            string name, string stack, string tier, string track, string version, string image, int percentage, bool preview
        ) {
            Name       = name;
            Stack      = stack;
            Tier       = tier;
            Track      = track;
            Version    = version;
            Image      = image;
            Percentage = percentage;
            Preview    = preview;
        }

        public string Stack      { get; }
        public string Name       { get; }
        public string Tier       { get; }
        public string Track      { get; }
        public string Version    { get; }
        public string Image      { get; }
        public int    Percentage { get; }
        public bool   Preview    { get; }

        public static IEnumerable<Option> GetOptions()
            => new Option[] {
                new Option<string>("--tier", () => "web", "Application tier"),
                new Option<string>("--track", () => "stable", "Application track"),
                new Option<string>("--image", Settings.GetImageRegistry, "Docker image"),
                new Option<int>("--percentage", () => 100, "Deployment percentage"),
                new Option<string>("--version", () => Env.ApplicationVersion, "Application version"),
                new Option<bool>("--preview", () => true, "Run preview instead of update")
            };
    }
}