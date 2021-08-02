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
                .Select(x => x.AsRow())
                .Where(x => x != null)
                .ToList();

            var sb = ConsoleTableBuilder
                .From(tableData)
                .WithColumn("", "Name", "Type", "Operation", "Diff")
                .WithFormat(ConsoleTableBuilderFormat.Minimal)
                .Export();

            var result = new StringBuilder();
            result.AppendLine(linkLine);
            result.AppendLine();
            result.Append(sb);

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

        record ResourcePreview(OperationType Op, string? Name, string Type, string[]? Diffs) {
            public static ResourcePreview FromOutputEvent(ResourceOutputsEvent evt)
                => new(evt.Metadata.Op, evt.Metadata.Old?.Id ?? evt.Metadata.New?.Id, evt.Metadata.Type, evt.Metadata
                    .Diffs?.ToArray());

            public List<object>? AsRow() {
                return Name == null
                    ? null
                    : new List<object> {OpString(Op), Name, Type, Op.ToString(), Diffs == null ? "" : string.Join(", ", Diffs)};

                string OpString(OperationType op)
                    => op switch {
                        OperationType.Create            => "+",
                        OperationType.Delete            => "-",
                        OperationType.Update            => "~",
                        OperationType.Replace           => "+-",
                        OperationType.CreateReplacement => "++",
                        OperationType.DeleteReplaced    => "--",
                        _                               => ""
                    };
            }
        }
    }
}