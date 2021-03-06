// *** WARNING: this file was generated by crd2pulumi. ***
// *** Do not edit by hand unless you're certain you know what you are doing! ***

using Pulumi;

namespace Ubiquitous.AutoDevOps.Crds.CertManager.Acme.V1Beta1.Inputs
{

    /// <summary>
    /// A node selector term, associated with the corresponding weight.
    /// </summary>
    public class ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceArgs : Pulumi.ResourceArgs
    {
        [Input("matchExpressions")]
        private InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchExpressionsArgs>? _matchExpressions;

        /// <summary>
        /// A list of node selector requirements by node's labels.
        /// </summary>
        public InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchExpressionsArgs> MatchExpressions
        {
            get => _matchExpressions ?? (_matchExpressions = new InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchExpressionsArgs>());
            set => _matchExpressions = value;
        }

        [Input("matchFields")]
        private InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchFieldsArgs>? _matchFields;

        /// <summary>
        /// A list of node selector requirements by node's fields.
        /// </summary>
        public InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchFieldsArgs> MatchFields
        {
            get => _matchFields ?? (_matchFields = new InputList<ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceMatchFieldsArgs>());
            set => _matchFields = value;
        }

        public ChallengeSpecSolverHttp01IngressPodTemplateSpecAffinityNodeAffinityPreferredDuringSchedulingIgnoredDuringExecutionPreferenceArgs()
        {
        }
    }
}
