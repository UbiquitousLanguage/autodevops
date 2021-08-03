using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleTableExt;
using Pulumi.Automation;
using Pulumi.Automation.Events;
using Ubiquitous.AutoDevOps.Stack;

namespace Ubiquitous.AutoDevOps.GitLab {
    public class NoteCollector {
        SummaryEvent?         summary;
        string?               linkLine;
        List<ResourcePreview> events = new();

        public void OnEvent(EngineEvent engineEvent) {
            if (engineEvent.SummaryEvent != null) {
                summary = engineEvent.SummaryEvent;
                return;
            }

            var outputEvent = engineEvent.ResourceOutputsEvent;
            if (outputEvent == null) return;
            events.Add(ResourcePreview.FromOutputEvent(outputEvent));
        }

        public void AddLink(string output) {
            var lines = Regex.Split(output, "\r\n|\r|\n");
            linkLine = lines.FirstOrDefault(x => x.StartsWith("View Live:"));
        }

        public string ParseAsNote() {
            var tableData = events
                .Where(x => x.Show)
                .Select(x => x.AsRow())
                .ToList();

            var builder = ConsoleTableBuilder
                .From(tableData)
                .WithColumn("", "Name", "Type", "Operation", "Diff")
                .WithCharMapDefinition()
                .WithPaddingLeft(string.Empty)
                .WithPaddingRight(" ");

            var result = new StringBuilder()
                .AppendLine("# Stack update preview")
                .AppendLine(linkLine)
                .AppendLine()
                .AppendLine("## Changes")
                .AppendLine("```diff")
                .Append(builder.Export())
                .AppendLine("```");

            if (summary != null) {
                result
                    .AppendLine("## Summary")
                    .AppendLine("```diff")
                    .Append(AsSummary())
                    .AppendLine("```");
            }

            return result.ToString();
        }

        StringBuilder AsSummary() {
            var sb = new StringBuilder();
            AddSummaryForOp(sb, OperationType.Create);
            AddSummaryForOp(sb, OperationType.Update);
            AddSummaryForOp(sb, OperationType.Delete);
            AddSummaryForOp(sb, OperationType.Replace);
            AddSummaryForOp(sb, OperationType.Refresh);
            AddSummaryForOp(sb, OperationType.Same);
            return sb;
        }

        void AddSummaryForOp(StringBuilder sb, OperationType operationType) {
            if (summary!.ResourceChanges.TryGetValue(operationType, out var changes))
                sb.AppendLine($"{OpString(operationType)} {operationType.ToString()}: {changes}");
        }

        record ResourcePreview {
            ResourcePreview(OperationType op, string urn, string[]? diffs) {
                Op    = op;
                Diffs = diffs == null ? "" : string.Join(", ", diffs);
                var parts = urn.Split("::");
                Type = parts[2];
                Name = parts[3];
            }

            public static ResourcePreview FromOutputEvent(ResourceOutputsEvent evt)
                => new(evt.Metadata.Op, evt.Metadata.Urn, evt.Metadata.Diffs?.ToArray());

            public List<object> AsRow() => new() {OpString(Op), Name, Type, Op.ToString(), Diffs};

            OperationType Op    { get; }
            string        Name  { get; }
            string        Type  { get; }
            string        Diffs { get; }

            public bool Show => !OpString(Op).IsEmpty();
        }

        static string OpString(OperationType op)
            => op switch {
                OperationType.Create  => "+",
                OperationType.Delete  => "-",
                OperationType.Update  => "~",
                OperationType.Replace => "+-",
                _                     => ""
            };
    }
}