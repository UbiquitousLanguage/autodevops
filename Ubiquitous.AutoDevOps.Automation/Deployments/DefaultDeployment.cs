using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleTableExt;
using Pulumi.Automation;
using Pulumi.Automation.Events;
using Ubiquitous.AutoDevOps.GitLab;
using Ubiquitous.AutoDevOps.Stack;
using static Serilog.Log;

namespace Ubiquitous.AutoDevOps.Deployments {
    public class DefaultDeployment<T> : IStackDeployment<T> where T : IDeploymentOptions {
        public async Task<CommandResult> DeployStack(IStackConfiguration<T> configuration, T options) {
            var currentDir = Directory.GetCurrentDirectory();

            Information("Starting with {Name} {Stack} in {CurrentDir}", options.Name, options.Stack, currentDir);

            var stackArgs = configuration.GetStackArgs(options.Name, options.Stack, currentDir);

            using var workspace = await LocalWorkspace.CreateAsync(stackArgs);

            var appStack = await WorkspaceStack.CreateOrSelectAsync(options.Stack, workspace);

            Information("Configuring stack {Stack}", options.Stack);

            var appSettings = new AutoDevOpsSettings.AppSettings(
                options.Name,
                options.Tier,
                options.Track,
                options.Version
            );
            await configuration.ConfigureStack(appStack, appSettings, options);

            Information("Refreshing stack {Stack}", options.Stack);
            await appStack.RefreshAsync();

            Information("Installing plugins");
            await configuration.InstallPlugins(appStack.Workspace);

            if (options.Preview) {
                return await Preview(appStack, options);
            }

            Information("Deploying stack {Stack}", options.Stack);

            var result = await appStack.UpAsync(
                new UpOptions {
                    OnStandardOutput = Information,
                    OnStandardError  = Error
                }
            );
            Information("Deployment result: {Result}", result.Summary.Message);

            if (!Env.EnvironmentUrl.IsEmpty()) {
                Information("Environment URL: {EnvironmentUrl}", Env.EnvironmentUrl);
            }

            return new CommandResult(
                result.Summary.Kind,
                result.Summary.Result,
                result.StandardOutput,
                result.StandardError,
                result.Summary.ResourceChanges
            );
        }

        static async Task<CommandResult> Preview(WorkspaceStack appStack, T options) {
            Information("Executing preview for stack {Stack}", options.Stack);

            var events = new List<ResourcePreview>();

            var previewResult = await appStack.PreviewAsync(
                new PreviewOptions {
                    OnStandardOutput = Information,
                    OnStandardError  = Error,
                    OnEvent          = OnEvent
                }
            );
            var lines    = Regex.Split(previewResult.StandardOutput, "\r\n|\r|\n");
            var linkLine = lines.FirstOrDefault(x => x.StartsWith("View Live:"));

            var tableData = events
                .Where(x => x.Show)
                .Select(x => x.AsRow())
                .ToList();

            var builder = ConsoleTableBuilder
                .From(tableData)
                .WithColumn("", "Name", "Type", "Operation", "Diff")
                .WithCharMapDefinition()
                .WithPaddingLeft(string.Empty)
                .WithPaddingLeft(" ");
                // .WithFormat(ConsoleTableBuilderFormat.Minimal)

            var result = new StringBuilder();
            result.AppendLine("# Stack update preview");
            result.AppendLine(linkLine);
            result.AppendLine();
            result.AppendLine("```diff");
            result.Append(builder.Export());
            result.AppendLine("```");

            var gitLabClient = GitLabClient.Create();

            if (gitLabClient != null) {
                Information("GitLab API URL and credentials are defined");
                await gitLabClient.AddMergeRequestNote(result.ToString());
            }
            else {
                Information("CI_API_V4_URL or GITLAB_API_TOKEN variable is not defined");
            }

            return new CommandResult(
                UpdateKind.Preview,
                UpdateState.Succeeded,
                previewResult.StandardOutput,
                previewResult.StandardError,
                previewResult.ChangeSummary
            );

            void OnEvent(EngineEvent engineEvent) {
                if (engineEvent.SummaryEvent != null) {
                    // Add summary here
                    return;
                }

                var outputEvent = engineEvent.ResourceOutputsEvent;
                if (outputEvent == null) return;
                events.Add(ResourcePreview.FromOutputEvent(outputEvent));
            }
        }

        record ResourcePreview {
            ResourcePreview(OperationType op, string urn, string[]? diffs) {
                Op    = op;
                Diffs = diffs == null ? "" : string.Join(", ", diffs);
                var parts = urn.Split("::");
                Type = parts[2];
                Name = parts[3];
            }

            public static ResourcePreview FromOutputEvent(ResourceOutputsEvent evt) {
                return new(evt.Metadata.Op, evt.Metadata.Urn, evt.Metadata.Diffs?.ToArray());
            }

            public List<object> AsRow() => new() {OpString(Op), Name, Type, Op.ToString(), Diffs};

            static string OpString(OperationType op)
                => op switch {
                    OperationType.Create            => "+",
                    OperationType.Delete            => "-",
                    OperationType.Update            => "~",
                    OperationType.Replace           => "+-",
                    _                               => ""
                };

            OperationType Op    { get; }
            string        Name  { get; }
            string        Type  { get; }
            string        Diffs { get; }

            public bool Show => !OpString(Op).IsEmpty();
        }
    }
}