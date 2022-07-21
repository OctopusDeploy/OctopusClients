ARG BASE_IMAGE=centos:7
FROM ${BASE_IMAGE}

# Install .NET SDKs
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH=/usr/share/dotnet:${PATH}

RUN yum install \
        findutils \
        libicu \
        libunwind \
    -y

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 6.0 --install-dir /usr/share/dotnet

# Install PowerShell
ENV POWERSHELL_TELEMETRY_OPTOUT=1
RUN dotnet tool install --global PowerShell --version=7.1.3

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
    org.label-schema.description="Test base image for Fedora." \
    org.label-schema.license="Apache"  \
    org.label-schema.name="test-fedora" \
    org.label-schema.schema-version="1.0" \
    org.label-schema.url="https://octopus.com/" \
    # org.label-schema.vcs-ref="${GIT_COMMIT_HASH}" \
    org.label-schema.vcs-url="https://github.com/OctopusDeploy/tool-containers/" \
    org.label-schema.vendor="Octopus Deploy" \
    # org.label-schema.version="${BUILD_NUMBER}" \
    # com.octopus.provenance.base-image="${BASE_IMAGE}" \
    # com.octopus.provenance.created-by="${CREATED_BY}" \
    # com.octopus.provenance.git-branch="${GIT_BRANCH}"