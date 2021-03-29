using System.CommandLine;
using AutoDevOps.Commands;

var command = new Root();
await command.InvokeAsync(args);
