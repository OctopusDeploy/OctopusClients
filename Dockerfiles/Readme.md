# octopusdeploy/octo
A docker wrapped version of the popular [octo.exe](https://octopus.com/docs/api-and-integration/octo.exe-command-line) command line tool

## Platforms
Images are currently available for
- [linux/amd64](alpine/Dockerfile) - based on alpine
- [windows/amd64](alpine/Dockerfile) - based on nanoserver


## Command Options
Arguments passed to the container will be passed directly to the `octo.exe` tool internally. For the full list of commands and parameters [read our docs](https://octopus.com/docs/api-and-integration/octo.exe-command-line).

### Example Usage
#### Help
```
>$ docker run --rm octopusdeploy/octo help

Usage: octo <command> [<options>]

Where <command> is one of:

  clean-environment
   Cleans all Offline Machines from an Environment
...
...
```
#### Print Version
```
>$ docker run --rm octopusdeploy/octo version

4.31.1
```

### Pack
Packing and pushing requires providing a volume mount so that the container process is able to access the required files. Internally, the working directory is set to `/src` so the simplest approach is to mount this point to the current working directory, and then pass the command arguments relative to this location.

```
>$ docker run --rm -v $(pwd):/src octopusdeploy/octo pack --id=AcmeWeb --version=3.1.4 --basePath=WebApp --overwrite

Packing AcmeWeb version "3.1.4"...
Saving "AcmeWeb.3.1.4.nupkg" to "/src"...
Done.
```
_(When running on windows, replace the file mount `$(pwd):/src` with `"$(Convert-Path .):C:\src"`)_


### Push
```
>$ docker run --rm -v $(pwd):/src octopusdeploy/octo push --package AcmeWeb.3.1.4.nupkg --replace-existing --server https://myoctopus.acme.com  --apiKey $env:apikey

Octopus Deploy Command Line Tool, version 4.31.1
Handshaking with Octopus server: https://myoctopus.acme.com
Handshake successful. Octopus version: 2018.4.0; API version: 3.0.0
Authenticated as: steve
Pushing package: /src/AcmeWeb.3.1.4.nupkg...
Push successful
```

### Simplified command for Linux
if using Linux you can add the following function to `~/.bashrc` to interact with the tool like a normal command. All parameters that refer to the filesystem (`pack`/`push`) can then just be referenced by relative path from the cwd.
```
echo 'function octo(){ sudo docker run --rm -v $(pwd):/src octopusdeploy/octo "$@" ;}' >> ~/.bashrc
source ~/.bashrc
octo pack --id=Acme --version=3.1.4 --basePath=OctopusDocker
```

## Licence
Copyright (c) Octopus Deploy and contributors. All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use
these files except in compliance with the License. You may obtain a copy of the
License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.