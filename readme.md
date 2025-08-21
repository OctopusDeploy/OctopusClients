This repository contains the .NET Client Library (`Octopus.Client`) for [Octopus Deploy](https://octopus.com).  
You can use it to create and deploy releases, push packages, manage environments, and perform many other actions.

`Octopus.Client` is [available on nuget.org](https://www.nuget.org/packages/Octopus.Client)

## Documentation

Please see the [Octopus.Client](https://octopus.com/docs/api-and-integration/octopus.client) documentation on the Octopus Documentation site.

## Issues

If you believe you have found a problem, or have a suggested enhancement, please raise it with the Octopus [Support Team](https://octopus.com/support)

While this library is licensed under the Apache 2.0 license and you are welcome to fork the it, this repository does not accept GitHub issues or Pull Requests. For more information please see the Octopus Deploy Blog: [Changes to the Octopus C# client library open source repository](https://octopus.com/blog/changes-to-octopus-csharp-client-repository)

## Versioning
We use [Semantic Versioning](http://semver.org/) for our open source libraries and tools. This breaks with our older practice of keeping version numbers in sync with Octopus Server. 

Within a major version of `Octopus.Client` we will maintain backwards compatibility to a set version of Octopus Server, allowing worry free minor and patch upgrades.

Conversely we also maintain backwards compatibility in our Server API as much as possible while still being able to add new features. This means that an older version of `Octopus.Client` will work with newer versions of Octopus Server.

## Compatibility

Refer to our [Compatibility](https://octopus.com/docs/api-and-integration/compatibility) page to get an overview of which versions work with a particular version of Octopus Server.

## Development
You need:
- VSCode, Visual Studio or JetBrains Rider to compile the solution
- dotnet 8.0 SDK

Run `Build.cmd` to build, test and package the project. Do this before pushing as it will run the public surface area tests as well,
which require approval on every change that modifies the public API.
