ARG BASE_IMAGE=ubuntu:xenial
FROM ${BASE_IMAGE}

# Install base pre-reqs
RUN apt-get update && \
    apt-get install -y \
        apt-transport-https \
        curl \
        tzdata \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install Mono
RUN apt-key update && \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb http://download.mono-project.com/repo/debian wheezy/snapshots 4.6.0.245/main" | tee /etc/apt/sources.list.d/mono-xamarin.list && \
    apt-get update && \
    apt-get install -y --allow-unauthenticated \
        mono-complete \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install .NET SDKs
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN curl --silent --fail --show-error --location --output packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm -f packages-microsoft-prod.deb && \
    apt-get update -y && \
    apt-get install -y \
        dotnet-sdk-6.0 \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install PowerShell
ENV POWERSHELL_TELEMETRY_OPTOUT=1
RUN dotnet tool install --global PowerShell --version=7.1.3

# Install Git and prerequisites
RUN apt-get update && \
    apt-get install -y \
        python-software-properties \
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
RUN test -n "$CREATED_BY" || (echo "Build argument CREATED_BY needs to be set and non-empty" && false)
RUN test -n "$GIT_BRANCH" || (echo "Build argument GIT_BRANCH needs to be set and non-empty" && false)
RUN test -n "$GIT_COMMIT_HASH" || (echo "Build argument GIT_COMMIT_HASH needs to be set and non-empty" && false)
RUN test -n "$BUILD_NUMBER" || (echo "Build argument BUILD_NUMBER needs to be set and non-empty" && false)
RUN test -n "$BUILD_DATE" || (echo "Build argument BUILD_DATE needs to be set and non-empty" && false)

LABEL \
    org.label-schema.build-date="${BUILD_DATE}" \
    org.label-schema.description="Test base image for Ubuntu 16." \
    org.label-schema.license="Apache"  \
    org.label-schema.name="octopusclients-test-ubuntu16" \
    org.label-schema.schema-version="1.0" \
    org.label-schema.url="https://octopus.com/" \
    org.label-schema.vcs-ref="${GIT_COMMIT_HASH}" \
    org.label-schema.vcs-url="https://github.com/OctopusDeploy/OctopusClients/" \
    org.label-schema.vendor="Octopus Deploy" \
    org.label-schema.version="${BUILD_NUMBER}" \
    com.octopus.provenance.base-image="${BASE_IMAGE}" \
    com.octopus.provenance.created-by="${CREATED_BY}" \
    com.octopus.provenance.git-branch="${GIT_BRANCH}"