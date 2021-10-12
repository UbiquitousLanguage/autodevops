using System.CommandLine;
using System.CommandLine.Invocation;
using Pulumi.Automation;
using Serilog;
using Ubiquitous.AutoDevOps.Deployments;

namespace Ubiquitous.AutoDevOps.Commands; 

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
                var result = await stackDeployment.DeployStack(configuration, options);

                Log.Information("Stack {UpdateKind} {UpdateState}", result.UpdateKind, result.UpdateState);

                var artefacts = new Artefacts(result);
                await artefacts.Save(Path.Join(Directory.GetCurrentDirectory(), "pulumi.txt"));
                return result.UpdateState == UpdateState.Succeeded ? 0 : -1;
            }
            catch (Exception e) {
                Log.Error(e, "Deployment failed: {Message}", e.Message);
                return -1;
            }
        }
    }
}