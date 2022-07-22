ARG CODE_VERSION=latest
ARG BASE_IMAGE=docker.packages.octopushq.com/octopusdeploy/tool-containers/octopusclients-test
FROM $BASE_IMAGE:$CODE_VERSION AS build
WORKDIR /source

# copy the full repo and restore
COPY . .
RUN dotnet restore ./source

# # target entrypoint with: docker build --target test
FROM build AS test

ARG CODE_VERSION
ENV CODE_VERSION=${CODE_VERSION}

WORKDIR /source/source/Octopus.Client.Tests

ENTRYPOINT dotnet test --configuration:Release --logger:"trx;LogFilePrefix=$CODE_VERSION" --no-restore

