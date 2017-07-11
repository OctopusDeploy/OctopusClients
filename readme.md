This repository contains the command line tool (`Octo.exe`) and .NET Client Library (`Octopus.Client`) for [Octopus Deploy][1], an automated deployment server for professional .NET developers. You can use them to create and deploy releases, create and push packages, and manage environments.

`Octo.exe` can be [downloaded from the Octopus downloads page][2], while `Octopus.Client` is [available on nuget.org][3]

## Documentation
- [Octo.exe][4]
- [Octopus.Client][5]

## Issues
Please see [Contributing](CONTRIBUTING.md)

## Development
You need:
- VSCode or Visual Studio 15.3 to compile the solution
- dotnet core 2.0-preview1 SDK

Run `Build.cmd` to build, test and package the project. Do this before pushing as it will run the surface area tests as well,
which require approval on almost every change.

To release to Nuget, tag `master` with the next major, minor or patch number, [TeamCity](https://build.octopushq.com/project.html?projectId=OctopusDeploy_OctopusClients&tab=projectOverview) will do the rest. Kick off the `Release: OctopusClients to Octopus3` build again if any of the dependencies fail.

Every successful TeamCity build for all branches will be pushed to MyGet.

## Compatibility
See the [Compatibility][7] page in our docs

## Async, Versioning and Compatibility
See the [Octopus.Client goes Open Source][6] blog post

[1]: https://octopus.com
[2]: https://octopus.com/downloads
[3]: https://www.nuget.org/packages/Octopus.Client
[4]: https://octopus.com/docs/api-and-integration/octo.exe-command-line
[5]: https://octopus.com/docs/api-and-integration/octopus.client
[6]: https://octopus.com/blog/octopus-client-goes-open-source
[7]: https://octopus.com/docs/api-and-integration/compatibility
