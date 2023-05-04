This repository contains the .NET Client Library (`Octopus.Client`) for [Octopus Deploy](https://octopus.com), an automated deployment server for professional .NET developers. You can use it to create and deploy releases, create and push packages, and manage environments.

`Octopus.Client` is [available on nuget.org](https://www.nuget.org/packages/Octopus.Client)

## Documentation
- [Octopus.Client](https://octopus.com/docs/api-and-integration/octopus.client)

## Issues
Please see [Contributing](CONTRIBUTING.md)

## Development
You need:
- VSCode, Visual Studio 2017 or JetBrains Rider to compile the solution
- dotnet core 2.2 SDK

Run `Build.cmd` to build, test and package the project. Do this before pushing as it will run the public surface area tests as well,
which require approval on every change that modifies the public API.

## Releasing

_Note:_ releases can only be performed by Octopus staff.
To release to Nuget, tag `master` with the next major, minor or patch number, TeamCity will do the rest. 
Kick off the `Create OctopusClients Release` build again if any of the dependencies fail.

This will push the release to our Octopus server, and trigger the `Octopus.Client` project in the integration space. 
A deployment will automatically happen to the `Extensions - Internal` environment, and publish the package to [Feedz.io](https://f.feedz.io/octopus-deploy/dependencies/nuget).
Once ready to be fully released, promote the release to the `Extensions - External` environment.

## Compatibility
See the [Compatibility](https://octopus.com/docs/api-and-integration/compatibility) page in our docs

## Versioning
We use [Semantic Versioning](http://semver.org/) for our open source libraries and tools. This breaks with our older practice of keeping version numbers in sync with Octopus Server. 

Within a major version of `Octopus.Client` we will maintain backwards compatibility to a set version of Octopus Server, allowing worry free minor and patch upgrades.

Conversely we also maintain backwards compatibility in our Server API as much as possible while still being able to add new features. This means that an older version of `Octopus.Client` will work with newer versions of Octopus Server.

Refer to our [Compatibility](https://octopus.com/docs/api-and-integration/compatibility) page to get an overview of which versions work with a particular version of Octopus Server.