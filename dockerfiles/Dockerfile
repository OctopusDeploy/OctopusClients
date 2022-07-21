ARG CODE_VERSION=latest
ARG BASE_IMAGE=docker.packages.octopushq.com/octopusdeploy/tool-containers/octopusclients-test
FROM $BASE_IMAGE:$CODE_VERSION AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY source/Octopus.Client/*.csproj source/Octopus.Client/*.csproj
COPY source/Octopus.Client.Tests/*.csproj source/Octopus.Client.Tests/*.csproj*.csproj
RUN dotnet restore source/Octopus.Client.Tests/.

# copy and build app and libraries
COPY source/Octopus.Client/ source/Octopus.Client/
COPY source/Octopus.Client.Tests/ source/Octopus.Client.Tests/
WORKDIR /source/Octopus.Client.Tests
RUN dotnet build -c release --no-restore

# target entrypoint with: docker build --target test
FROM build AS test
WORKDIR /source/tests
COPY tests/ .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]
