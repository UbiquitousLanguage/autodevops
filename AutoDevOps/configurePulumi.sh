#!/bin/sh

# Configure GitLab settings
pulumi config set --path gitlab.App "$CI_PROJECT_PATH_SLUG"
pulumi config set --path gitlab.Env "$CI_ENVIRONMENT_SLUG"
pulumi config set --path gitlab.EnvName "$CI_ENVIRONMENT_NAME"
pulumi config set --path gitlab.EnvURL "$CI_ENVIRONMENT_URL"
pulumi config set --path gitlab.Visibility "$CI_PROJECT_VISIBILITY"

# Configure container registry access
pulumi config set --path registry.Server "$CI_REGISTRY"
pulumi config set --path registry.User "${CI_DEPLOY_USER:-$CI_REGISTRY_USER}"
pulumi config set --secret --path registry.Password "${CI_DEPLOY_PASSWORD:-$CI_REGISTRY_PASSWORD}"
pulumi config set --path registry.Email "$GITLAB_USER_EMAIL"

# Configure app parameters
pulumi config set --path app.Name "$CI_PROJECT_NAME"
pulumi config set --path app.SecretName "$APPLICATION_SECRET_NAME"
pulumi config set --path app.SecretChecksum "$APPLICATION_SECRET_CHECKSUM"
pulumi config set --path app.Track "$track"

# Configure deployment
pulumi config set --path deploy.Image "$image_repository"
pulumi config set --path deploy.ImageTag "$image_tag"
pulumi config set --path deploy.Percentage "$percentage"
pulumi config set --path deploy.Release "$CI_ENVIRONMENT_NAME"
pulumi config set --path deploy.Namespace "$KUBE_NAMESPACE"
pulumi config set --path deploy.Url "$CI_ENVIRONMENT_URL"

echo "Configured Pulumi using CI/CD environment settings"