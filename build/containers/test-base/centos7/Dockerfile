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

RUN curl -sSL http://dot.net/v1/dotnet-install.sh > dotnet-install.sh && chmod +x dotnet-install.sh && ./dotnet-install.sh --channel 6.0 --install-dir /usr/share/dotnet

# Convention to allow users of this tool container to easily see how it was created.
COPY Dockerfile /
