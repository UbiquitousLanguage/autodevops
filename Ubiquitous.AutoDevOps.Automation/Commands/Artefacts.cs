using Ubiquitous.AutoDevOps.Deployments;

namespace Ubiquitous.AutoDevOps.Commands; 

public class Artefacts {
    readonly CommandResult _result;

    public Artefacts(CommandResult result) {
        _result = result;
    }

    public async Task Save(string fileName) {
        var content = _result.StandardOutput + "\r\n" + _result.StandardError;
        await File.WriteAllTextAsync(fileName, content);
    }
}