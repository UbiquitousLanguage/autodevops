using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serilog;
using Ubiquitous.AutoDevOps.Deployments;

namespace Ubiquitous.AutoDevOps.Commands {
    public class Deploy<T, TConfig, TOptions> : Command
        where T : IStackDeployment<TOptions>
        where TConfig : class, IStackConfiguration<TOptions>
        where TOptions : IDeploymentOptions {
        public Deploy(T stackDeployment, TConfig configuration, IEnumerable<Option> commandOptions)
            : base("deploy", "Deploy (update) the stack") {
            foreach (var option in commandOptions) {
                AddOption(option);
            }

            Handler = CommandHandler.Create<TOptions>(DeployStack);

            async Task<int> DeployStack(TOptions options) {
                try {
                    return await stackDeployment.DeployStack(configuration, options);
                }
                catch (Exception e) {
                    Log.Error("Deployment failed: {Message}", e.Message);
                    return -1;
                }
            }
        }
    }
}