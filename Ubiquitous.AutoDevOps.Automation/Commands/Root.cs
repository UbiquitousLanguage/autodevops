using System.CommandLine;

namespace Ubiquitous.AutoDevOps.Commands {
    public class Root : RootCommand {
        public Root(params Command[] commands) {
            foreach (var command in commands) {
                AddCommand(command);
            }
            
            AddGlobalOption(new Option<string>("--stack", () => Env.Environment, "Stack name"));
            AddGlobalOption(new Option<string>("--name", () => Env.ProjectName, "Application name"));
        }
    }
}