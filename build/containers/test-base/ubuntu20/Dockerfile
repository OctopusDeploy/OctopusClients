ARG BASE_IMAGE=ubuntu:focal
FROM ${BASE_IMAGE}

# Install base pre-reqs
RUN apt-get update && \
    apt-get install -y \
        apt-transport-https \
        curl \
        unzip \
        gnupg \
        tzdata \
        sudo \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install Mono
RUN apt-key update && \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" > /etc/apt/sources.list.d/mono-official-stable.list && \
    apt-get update && \
    apt-get install -y --allow-unauthenticated \
        mono-complete \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install .NET SDKs
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH=/usr/share/dotnet:${PATH}
# RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 3.1 --install-dir /usr/share/dotnet
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 6.0 --install-dir /usr/share/dotnet

# Install PowerShell
ENV POWERSHELL_TELEMETRY_OPTOUT=1
# need to add the global tool directory to the PATH - workloads running this container need to just call `pwsh` and not provide the full path
# the global tool install does *not* put the installed tool location on the path - https://github.com/dotnet/dotnet-docker/issues/520 for a similar discussion
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global PowerShell --version=7.1.3

# Install Git and prerequisites
RUN apt-get update && \
    apt-get install -y \
        software-properties-common \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

RUN add-apt-repository ppa:git-core/ppa && \
    apt-get update && \
    apt-get clean && \
    apt-get install -y \
        build-essential \
        git \
        openjdk-11-jre \
        libcurl4-openssl-dev  \
        libreadline-dev \
        libsqlite3-dev \
        libssl-dev \
        libxml2-dev \
        libxslt1-dev \
        libyaml-dev \
        sqlite3 \
        zlib1g-dev \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# aws cli
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" && \
        unzip awscliv2.zip && \
        ./aws/install && \
        rm -rf ./aws && rm awscliv2.zip

# helm

RUN curl https://baltocdn.com/helm/signing.asc | sudo apt-key add - && \
    sudo apt-get install apt-transport-https --yes && \
    echo "deb https://baltocdn.com/helm/stable/debian/ all main" | sudo tee /etc/apt/sources.list.d/helm-stable-debian.list && \
    sudo apt-get update && \
    sudo apt-get install helm

# kubectl
RUN sudo apt-get install -y apt-transport-https ca-certificates curl && \
    sudo curl -fsSLo /usr/share/keyrings/kubernetes-archive-keyring.gpg https://packages.cloud.google.com/apt/doc/apt-key.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/kubernetes-archive-keyring.gpg] https://apt.kubernetes.io/ kubernetes-xenial main" | sudo tee /etc/apt/sources.list.d/kubernetes.list && \
    sudo apt-get update && \
    sudo apt-get install -y kubectl

# Confirm these are actually installed
RUN curl --version && \
    dotnet --version && \
    pwsh --version && \
    sudo --version && \
    mono --version && \
    java --version && \
    aws --version && \
    helm version && \
    kubectl version --client

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
    org.label-schema.description="Test base image for Ubuntu 20." \
    org.label-schema.license="Apache"  \
    org.label-schema.name="test-ubuntu20" \
    org.label-schema.schema-version="1.0" \
    org.label-schema.url="https://octopus.com/" \
    # org.label-schema.vcs-ref="${GIT_COMMIT_HASH}" \
    org.label-schema.vcs-url="https://github.com/OctopusDeploy/tool-containers/" \
    org.label-schema.vendor="Octopus Deploy" \
    # org.label-schema.version="${BUILD_NUMBER}" \
    # com.octopus.provenance.base-image="${BASE_IMAGE}" \
    # com.octopus.provenance.created-by="${CREATED_BY}" \
    # com.octopus.provenance.git-branch="${GIT_BRANCH}"