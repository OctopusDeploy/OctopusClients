ARG BASE_IMAGE=ubuntu:xenial
FROM ${BASE_IMAGE}

# Install base pre-reqs
RUN apt-get update && \
    apt-get install -y \
        apt-transport-https \
        curl \
        tzdata \
        libicu55 \
        && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install .NET SDKs
ENV PATH=/usr/share/dotnet:${PATH}
RUN curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh && chmod +x dotnet-install.sh && ./dotnet-install.sh --channel 6.0 --install-dir /usr/share/dotnet

# Convention to allow users of this tool container to easily see how it was created.
COPY Dockerfile /


