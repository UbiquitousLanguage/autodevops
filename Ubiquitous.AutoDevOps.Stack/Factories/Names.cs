using JetBrains.Annotations;
using Pulumi;

namespace Ubiquitous.AutoDevOps.Stack.Factories;

[PublicAPI]
public record ResourceName {
    public string Value { get; }

    public ResourceName(string value) {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value), "Resource name cannot be empty");
        Value = value;
    }

    public static implicit operator string(ResourceName resourceName) => resourceName.GetName();
        
    public static implicit operator Input<string>(ResourceName resourceName) => resourceName.GetName();
        
    public static implicit operator ResourceName(string value) => new(value);

    static string _baseName = "";
    public static void SetBaseName(string baseName) => _baseName = baseName;

    public PulumiName AsPulumiName() => new(Value);
        
    string GetName() => _baseName.IsEmpty() ? Value : $"{_baseName}-{Value}";
}
    
[PublicAPI]
public record PulumiName {
    public string Value { get; }

    public PulumiName(string value) {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value), "Pulumi resource name cannot be empty");
        Value = value;
    }

    public static implicit operator string(PulumiName resourceName)
        => _baseName.IsEmpty() ? resourceName.Value : $"{_baseName}-{resourceName.Value}";

    static string _baseName = "";
    public static void SetBaseName(string baseName) => _baseName = baseName;
}