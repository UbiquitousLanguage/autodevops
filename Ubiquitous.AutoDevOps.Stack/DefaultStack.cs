using System;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Stack {
    public class DefaultStack : Pulumi.Stack {
        protected readonly AutoDevOps AutoDevOps;

        public DefaultStack() : this(settings => new AutoDevOps(settings)) { }

        public DefaultStack(Func<AutoDevOpsSettings, AutoDevOps> cfg) {
            var config     = new Config();
            var settings   = new AutoDevOpsSettings(config);
            AutoDevOps = cfg(settings);
        }
    }

    public class DefaultStack<T> : DefaultStack {
        public DefaultStack(Func<AutoDevOps, Output<T>> getOutput) => AppStackOutput = getOutput(AutoDevOps);

        public DefaultStack(Func<AutoDevOpsSettings, AutoDevOps> cfg, Func<AutoDevOps, Output<T>> getOutput) : base(cfg)
            => AppStackOutput = getOutput(AutoDevOps);

        [Output]
        public Output<T> AppStackOutput { get; }
    }
}