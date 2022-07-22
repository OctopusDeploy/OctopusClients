ARG CODE_VERSION=latest
ARG BASE_IMAGE=docker.packages.octopushq.com/octopusdeploy/tool-containers/octopusclients-test
FROM $BASE_IMAGE:$CODE_VERSION AS build
WORKDIR /source

# copy the full repo and restore
COPY . .
RUN dotnet restore ./source

# target entrypoint with: docker build --target test
FROM build AS test
WORKDIR /source/source/Octopus.Client.Tests
ENTRYPOINT ["dotnet", "test", "--configuration:Release", "--logger:trx", "--no-restore"]
