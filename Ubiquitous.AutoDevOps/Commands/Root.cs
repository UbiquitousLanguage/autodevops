using System.CommandLine;

namespace Ubiquitous.AutoDevOps.Commands {
    public class Root : RootCommand {
        public Root() {
            AddCommand(new Deploy());
            AddCommand(new Destroy());
            AddGlobalOption(new Option<string>("--stack", () => Env.Environment, "Stack name"));
            AddGlobalOption(new Option<string>("--name", () => Env.ProjectName, "Application name"));
        }
    }
}