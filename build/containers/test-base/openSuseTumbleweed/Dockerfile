ARG BASE_IMAGE=opensuse/tumbleweed:latest
FROM ${BASE_IMAGE}

# Install .NET SDKs
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH=/usr/share/dotnet:${PATH}

RUN zypper --non-interactive install \
        findutils \
        tar \
        gzip \
        icu

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 6.0 --install-dir /usr/share/dotnet

# Convention to allow users of this tool container to easily see how it was created.
COPY Dockerfile /
