using System.CommandLine;
using System.CommandLine.Invocation;
using Ubiquitous.AutoDevOps.Stack;
using Pulumi.Automation;
using static Serilog.Log;

namespace Ubiquitous.AutoDevOps.Commands;

public class Destroy : Command {
    public Destroy() : base("destroy", "Destroy the stack") {
        Handler = CommandHandler.Create<string>(DestroyStack);
    }

    static async Task<int> DestroyStack(string stack) {
        var projectName = Env.ProjectName;
        var currentDir  = Directory.GetCurrentDirectory();

        using var workspace = await LocalWorkspace.CreateAsync(
            new LocalWorkspaceOptions {
                Program         = PulumiFn.Create<DefaultStack>(),
                ProjectSettings = new ProjectSettings(projectName, ProjectRuntimeName.Dotnet),
                WorkDir         = currentDir
            }
        );
        var appStack = await WorkspaceStack.SelectAsync(stack, workspace);

        Information("Destroying {Stack}", stack);

        var result = await appStack.DestroyAsync(
            new DestroyOptions {
                OnStandardOutput = Information,
                OnStandardError  = Error
            }
        );

        return result.Summary.Result == UpdateState.Succeeded ? 0 : -1;
    }
}