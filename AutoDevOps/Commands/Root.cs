using System.CommandLine;

namespace AutoDevOps.Commands {
    public class Root : RootCommand {
        public Root() {
            AddCommand(new Deploy());
            AddCommand(new Destroy());
            AddGlobalOption(new Option<string>("--stack", () => Defaults.GitLabVar("CI_ENVIRONMENT_NAME"), "Stack name"));
        }
    }
}