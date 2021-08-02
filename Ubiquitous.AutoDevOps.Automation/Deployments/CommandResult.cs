using System.Collections.Immutable;
using JetBrains.Annotations;
using Pulumi.Automation;

namespace Ubiquitous.AutoDevOps.Deployments {
    [PublicAPI]
    public class CommandResult {
        public CommandResult(
            UpdateKind                                updateKind,
            UpdateState                               updateState,
            string                                    standardOutput,
            string                                    standardError,
            IImmutableDictionary<OperationType, int>? changeSummary
        ) {
            UpdateState    = updateState;
            UpdateKind     = updateKind;
            StandardOutput = standardOutput;
            StandardError  = standardError;
            ChangeSummary  = changeSummary;
        }

        public UpdateKind  UpdateKind     { get; }
        public UpdateState UpdateState    { get; }
        public string      StandardOutput { get; }
        public string      StandardError  { get; }

        public IImmutableDictionary<OperationType, int>? ChangeSummary { get; }
    }
}