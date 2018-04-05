FROM microsoft/dotnet:2.1-runtime-alpine
MAINTAINER robert.erez devops@octopus.com

#alpine doesnt have curl installed
RUN apk add --no-cache curl

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
