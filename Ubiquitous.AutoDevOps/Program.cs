using System.CommandLine;
using Ubiquitous.AutoDevOps.Commands;
using Serilog;
using Ubiquitous.AutoDevOps.Deployments;

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