ARG PULUMI_VERSION=latest
ARG PULUMI_IMAGE=pulumi/pulumi-base
FROM ${PULUMI_IMAGE}:${PULUMI_VERSION} as pulumi

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . .
RUN dotnet publish ./Ubiquitous.AutoDevOps -c Release -r linux-x64 --self-contained -clp:NoSummary -o /app/publish

# The runtime container
FROM debian:buster-slim
WORKDIR /pulumi/projects

# Uses the workdir, copies from pulumi interim container
COPY --from=pulumi /pulumi/bin/pulumi /pulumi/bin/pulumi
COPY --from=pulumi /pulumi/bin/*-dotnet* /pulumi/bin/
ENV PATH "/pulumi/bin:${PATH}"

WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
CMD ["./Ubiquitous.AutoDevOps"]