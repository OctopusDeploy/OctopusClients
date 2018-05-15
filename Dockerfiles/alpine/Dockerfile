FROM microsoft/dotnet:2.1-runtime-alpine
MAINTAINER robert.erez devops@octopus.com

#alpine doesnt have curl installed
RUN apk add --no-cache curl

# Alpine base image does not have ICU libraries available
# https://github.com/dotnet/corefx/blob/master/Documentation/architecture/globalization-invariant-mode.md
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8

ARG OCTO_TOOLS_VERSION=4.31.1

LABEL maintainer="devops@octopus.com" \
        octopus.dockerfile.version="1.0" \
        octopus.tools.version=$OCTO_TOOLS_VERSION


ENV OCTO_TOOLS_DOWNLOAD_URL https://download.octopusdeploy.com/octopus-tools/$OCTO_TOOLS_VERSION/OctopusTools.$OCTO_TOOLS_VERSION.portable.tar.gz


WORKDIR /octo
RUN curl ${OCTO_TOOLS_DOWNLOAD_URL}  --output OctopusTools.tar.gz && tar -zxvf OctopusTools.tar.gz

RUN apk del curl

WORKDIR /src
ENTRYPOINT ["dotnet", "/octo/Octo.dll"]
