using System;
using JetBrains.Annotations;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Stack {
    /// <summary>
    /// The default stack, which deploys the <see cref="AutoDevOps"/> resources.
    /// </summary>
    [PublicAPI]
    public class DefaultStack : Pulumi.Stack {
        protected readonly AutoDevOps AutoDevOps;

        /// <summary>
        /// Default constructor, which uses the default way to configure the deployment.
        /// </summary>
        public DefaultStack() : this(settings => new AutoDevOps(settings)) { }

        /// <summary>
        /// Use this constructor to configure the deployment differently, using the configuration provided.
        /// </summary>
        /// <param name="cfg">A function, which creates the <see cref="AutoDevOps"/> instance given the settings.</param>
        public DefaultStack(Func<AutoDevOpsSettings, AutoDevOps> cfg) {
            var config     = new Config();
            var settings   = new AutoDevOpsSettings(config);
            AutoDevOps = cfg(settings);
        }
    }

    /// <summary>
    /// A variation of the default stack, which returns a custom result output.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public class DefaultStack<T> : DefaultStack {
        /// <summary>
        /// Default constructor, which uses the default <see cref="AutoDevOps"/> configuration.
        /// </summary>
        /// <param name="getOutput">A function, which creates the result from the AutoDevOps instance.</param>
        public DefaultStack(Func<AutoDevOps, Output<T>> getOutput) => AppStackOutput = getOutput(AutoDevOps);

        /// <summary>
        /// Use this constructor to configure the deployment differently, using the configuration provided.
        /// </summary>
        /// <param name="cfg">A function, which creates the <see cref="AutoDevOps"/> instance given the settings.</param>
        /// <param name="getOutput">A function, which creates the result from the AutoDevOps instance.</param>
        public DefaultStack(Func<AutoDevOpsSettings, AutoDevOps> cfg, Func<AutoDevOps, Output<T>> getOutput) : base(cfg)
            => AppStackOutput = getOutput(AutoDevOps);

        [Output]
        public Output<T> AppStackOutput { get; }
    }
}