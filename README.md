# Ubiquitous AutoDevOps

GitLab AutoDevOps implementation with [Pulumi](https://pulumi.com) instead of Helm.

Supports three modes:
- Stack mode, where you can use Pulumi CLI from the GitLab CI file, check the `example` directory.
- Automation API mode, where the stack is configured by the deployment app, so you don't need to configure it in the CI file.
- Custom automation mode, where you use the base code from the default deployment automation and customise it according to your needs.

Documentation is WIP.