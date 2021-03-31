using System.CommandLine;
using AutoDevOps.Commands;

var command = new Root();
return await command.InvokeAsync(args);
