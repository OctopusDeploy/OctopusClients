ARG CODE_VERSION=latest
ARG BASE_IMAGE=docker.packages.octopushq.com/octopusdeploy/tool-containers/octopusclients-test
FROM $BASE_IMAGE:$CODE_VERSION AS build
WORKDIR /source

# COPY source/NuGet.Config .
# COPY source/Octopus.Client/*.csproj Octopus.Client/
# COPY source/Octopus.Client.Tests/*.csproj Octopus.Client.Tests/
# COPY source/Octopus.Server.Client/*.csproj Octopus.Server.Client/
# RUN dotnet restore Octopus.Client.Tests/.

# copy and build app and libraries
COPY . .
RUN dotnet test ./source


# target entrypoint with: docker build --target test
FROM build AS test
WORKDIR /source/Octopus.Client.Tests
ENTRYPOINT ["dotnet", "test", "--logger:trx"]
