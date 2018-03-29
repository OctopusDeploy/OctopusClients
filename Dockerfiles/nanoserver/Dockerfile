FROM microsoft/dotnet:2.1-runtime-nanoserver-sac2016
MAINTAINER robert.erez devops@octopus.com
ARG OCTO_TOOLS_VERSION=4.31.1

LABEL maintainer="devops@octopus.com" \ 
	octopus.dockerfile.version="1.0" \
	octopus.tools.version=$OCTO_TOOLS_VERSION 

ENV OCTO_TOOLS_DOWNLOAD_URL https://download.octopusdeploy.com/octopus-tools/$OCTO_TOOLS_VERSION/OctopusTools.$OCTO_TOOLS_VERSION.portable.zip

RUN Invoke-WebRequest $Env:OCTO_TOOLS_DOWNLOAD_URL -OutFile OctopusTools.zip; \
	Expand-Archive OctopusTools.zip -DestinationPath octo; \
	Remove-Item -Force OctopusTools.zip; \
	mkdir src |Out-Null

WORKDIR /src
ENTRYPOINT ["dotnet", "/octo/Octo.dll"]
