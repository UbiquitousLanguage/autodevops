using System.Collections.Generic;
using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

namespace Ubiquitous.AutoDevOps.Stack.Resources {
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
        ) {
            var namespaceAnnotations = (annotations ?? new Dictionary<string, string>())
                .AsInputMap();

            return new Namespace(
                name,
                new NamespaceArgs {Metadata = new ObjectMetaArgs {Name = name, Annotations = namespaceAnnotations}},
                new CustomResourceOptions {Provider = providerResource}
            );
        }
    }
}