using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using AutoDevOps.Stack;
using Pulumi.Automation;

namespace AutoDevOps.Commands {
    class Destroy : Command {
        public Destroy() : base("destroy", "Destroy the stack") {
            Handler = CommandHandler.Create<string>(DestroyStack);
        }

        static async Task DestroyStack(string stack) {
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
            
            Console.WriteLine($"Destroying {stack}");
            
            await appStack.DestroyAsync(new DestroyOptions { OnStandardOutput = Console.WriteLine }); 
        }
    }
}