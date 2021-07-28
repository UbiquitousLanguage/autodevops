using System.CommandLine;
using System.Threading.Tasks;
using Serilog;
using Ubiquitous.AutoDevOps.Commands;
using Ubiquitous.AutoDevOps.Deployments;

namespace Ubiquitous.AutoDevOps {
    static class Program {
        static async Task<int> Main(string[] args) {
            var command = new Root(
                new Deploy<DefaultDeployment<DefaultOptions>, DefaultConfiguration, DefaultOptions>(
                    new DefaultDeployment<DefaultOptions>(),
                    new DefaultConfiguration(),
                    DefaultOptions.GetOptions()
                ),
                new Destroy()
            );

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            return await command.InvokeAsync(args);
        }
    }
}