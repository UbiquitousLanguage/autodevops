using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

namespace Ubiquitous.AutoDevOps.Stack.Resources;

public static class KubeNamespace {
    /// <summary>
    /// Create the namespace
    /// </summary>
    /// <param name="name">Namespace name</param>
    /// <param name="annotations">Optional namespaces annotations</param>
    /// <param name="providerResource">Optional custom Kubernetes provider</param>
    /// <returns></returns>
    public static Namespace Create(
        string name, Dictionary<string, string>? annotations = null, ProviderResource? providerResource = null
    )
        => new(
            name,
            new NamespaceBuilder(name).WithAnnotations(annotations).Build(),
            providerResource.AsResourceOptions()
        );
}

public class NamespaceBuilder {
    readonly ObjectMetaArgs _meta = new();

    public NamespaceBuilder(string name) => _meta.Name = name;

    public NamespaceBuilder WithAnnotations(Dictionary<string, string>? annotations) {
        if (annotations != null) _meta.Annotations = annotations;
        return this;
    }

    public NamespaceBuilder WithLabels(Dictionary<string, string>? labels) {
        if (labels != null) _meta.Labels = labels;
        return this;
    }

    public NamespaceBuilder ConfigureMeta(Action<ObjectMetaArgs> configure) {
        configure(_meta);
        return this;
    }

    public NamespaceArgs Build() => new() { Metadata = _meta };
}

public static class ProviderExtensions {
    public static CustomResourceOptions? AsResourceOptions(this ProviderResource? providerResource)
        => providerResource == null ? null : new CustomResourceOptions();
}