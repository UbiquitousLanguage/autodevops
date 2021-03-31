using System.CommandLine;
using AutoDevOps.Commands;
using Serilog;

var command = new Root();

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

return await command.InvokeAsync(args);
