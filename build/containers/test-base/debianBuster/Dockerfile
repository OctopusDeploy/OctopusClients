# syntax=docker/dockerfile:1.3-labs

ARG BASE_IMAGE=debian:buster
FROM ${BASE_IMAGE}

# Base prerequisites
RUN apt-get update && apt-get install \
    curl \
    unzip \
    apt-transport-https \
    dirmngr \
    gnupg \
    ca-certificates \
    software-properties-common \
    lsb-release \
    --yes

# Base setup for test usage
RUN unlink /etc/localtime && ln -s /usr/share/zoneinfo/Australia/Brisbane /etc/localtime && \
    groupadd octopus && usermod --append --groups octopus $(whoami) && \
    mkdir /etc/octopus && chgrp --recursive octopus /etc/octopus && \
    chmod -R g+rwx /etc/octopus

# Install mono 5.14 (see https://octopusdeploy.slack.com/archives/C033W4273/p1555048849003400) - https://www.mono-project.com/download/stable/#download-lin-debian
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb https://download.mono-project.com/repo/debian stable-stretch/snapshots/5.14 main" | tee /etc/apt/sources.list.d/mono-official-stable.list && \
    apt update && apt-get install mono-complete --yes

# Install NUnit Console
RUN curl -L -o /tmp/NUnit-2.6.3.zip https://launchpad.net/nunitv2/trunk/2.6.3/+download/NUnit-2.6.3.zip && \
    mkdir -p /opt/NUnit && cd /opt/NUnit && unzip /tmp/NUnit-2.6.3.zip

# inject shim for nunit console
COPY <<EOF /usr/bin/nunit-console
#!/bin/sh
exec /usr/bin/mono --debug $MONO_OPTIONS /opt/NUnit/NUnit-2.6.3/bin/nunit-console.exe "$@"
EOF

# The rest of NUnit Console
RUN chmod 755 /usr/bin/nunit-console && curl -L -o /tmp/NUnit-3.4.1.zip https://github.com/nunit/nunit/releases/download/3.4.1/NUnit-3.4.1.zip && \
    mkdir -p /opt/NUnit/NUnit-3.4.1 && cd /opt/NUnit/NUnit-3.4.1 && unzip /tmp/NUnit-3.4.1.zip

# Install .NET SDKs
# RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 3.1 --install-dir /usr/share/dotnet
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 6.0 --install-dir /usr/share/dotnet

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV PATH=/usr/share/dotnet:${PATH}

# Install git
RUN apt-get install git curl zlib1g-dev build-essential libssl-dev \
    libreadline-dev libyaml-dev libsqlite3-dev sqlite3 libxml2-dev \
    libxslt1-dev libcurl4-openssl-dev libffi-dev --yes && \
    git config --system init.defaultBranch main

# Install kubectl
RUN curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add - && \
    touch /etc/apt/sources.list.d/kubernetes.list && echo "deb http://apt.kubernetes.io/ kubernetes-xenial main" | tee -a /etc/apt/sources.list.d/kubernetes.list && \
    apt-get update && apt-get install kubectl --yes

# Install helm
RUN curl https://baltocdn.com/helm/signing.asc | apt-key add - && \
    echo "deb https://baltocdn.com/helm/stable/debian/ all main" | tee /etc/apt/sources.list.d/helm-stable-debian.list && \
    apt-get update && apt-get install helm --yes

# Install pip
RUN apt-get install python3-pip --yes

# Install Powershell
RUN curl https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb --output packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && apt-get update && \
    apt-get install powershell --yes

ENV POWERSHELL_TELEMETRY_OPTOUT=1

# Install Azure CLI
RUN curl -sL https://packages.microsoft.com/keys/microsoft.asc | \
    gpg --dearmor | tee /etc/apt/trusted.gpg.d/microsoft.gpg > /dev/null && \
    AZ_REPO=$(lsb_release -cs) && \
    echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | \
    tee /etc/apt/sources.list.d/azure-cli.list && \
    apt-get update && apt-get install azure-cli

# Install ruby - we need to update ruby-build so that it's aware of 2.7.1
RUN apt install rbenv --yes && \
    git clone https://github.com/rbenv/ruby-build.git && \
    PREFIX=/usr/local ./ruby-build/install.sh && \
    rbenv install -v 2.7.1 && rbenv global 2.7.1

# Install jq
RUN apt-get install jq --yes

# Security updates including explicit sudo update for CVE-2017-1000367
RUN apt-get install unattended-upgrades --assume-yes && \
    unattended-upgrades --verbose && \
    apt-get install sudo --assume-yes

# Sanity check
RUN mono --version && \
    ## nunit-console && \ 
    dotnet --version && \
    git --version && \
    kubectl version --client && \
    helm version && \
    pip3 --version && \
    pwsh --version && \
    az --version && \
    ruby --version && \
    jq --version

# Convention to allow users of this tool container to easily see how it was created.
COPY Dockerfile /

# note: ARG instructions go out of scope at the end of each build stage; we need to repeat them
ARG BASE_IMAGE
# ideally, these would be declared (and tested for) at the top of the file, and therefore
# at the start of the build, but that causes cache misses on subsequent RUN statements
ARG CREATED_BY
ARG GIT_BRANCH
ARG GIT_COMMIT_HASH
ARG BUILD_NUMBER
ARG BUILD_DATE

# Check for mandatory build arguments
RUN test -n "$BASE_IMAGE" || (echo "Build argument BASE_IMAGE needs to be set and non-empty" && false)
# RUN test -n "$CREATED_BY" || (echo "Build argument CREATED_BY needs to be set and non-empty" && false)
# RUN test -n "$GIT_BRANCH" || (echo "Build argument GIT_BRANCH needs to be set and non-empty" && false)
# RUN test -n "$GIT_COMMIT_HASH" || (echo "Build argument GIT_COMMIT_HASH needs to be set and non-empty" && false)
# RUN test -n "$BUILD_NUMBER" || (echo "Build argument BUILD_NUMBER needs to be set and non-empty" && false)
# RUN test -n "$BUILD_DATE" || (echo "Build argument BUILD_DATE needs to be set and non-empty" && false)

LABEL \
    org.label-schema.build-date="${BUILD_DATE}" \
    org.label-schema.description="Test base image for Debian." \
    org.label-schema.license="Apache"  \
    org.label-schema.name="test-debian-buster" \
    org.label-schema.schema-version="1.0" \
    org.label-schema.url="https://octopus.com/" \
    # org.label-schema.vcs-ref="${GIT_COMMIT_HASH}" \
    org.label-schema.vcs-url="https://github.com/OctopusDeploy/tool-containers/" \
    org.label-schema.vendor="Octopus Deploy" \
    # org.label-schema.version="${BUILD_NUMBER}" \
    # com.octopus.provenance.base-image="${BASE_IMAGE}" \
    # com.octopus.provenance.created-by="${CREATED_BY}" \
    # com.octopus.provenance.git-branch="${GIT_BRANCH}"