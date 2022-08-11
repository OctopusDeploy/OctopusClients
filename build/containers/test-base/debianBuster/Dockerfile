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

# Install .NET SDKs
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 3.1 --install-dir /usr/share/dotnet
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 5.0 --install-dir /usr/share/dotnet

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV PATH=/usr/share/dotnet:${PATH}

# Convention to allow users of this tool container to easily see how it was created.
COPY Dockerfile /

