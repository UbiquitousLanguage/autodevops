using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace AutoDevOps.Commands {
    class Destroy : Command {
        public Destroy() : base("destroy", "Destroy the stack") {
            Handler = CommandHandler.Create<string>(DestroyStack);
        }

        async Task DestroyStack(string stack) {
            Console.WriteLine($"Destroying {stack}");
        }
    }
}