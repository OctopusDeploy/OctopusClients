This repository contains the command line tool (`Octo.exe`) and .NET Client Library (`Octopus.Client`) for [Octopus Deploy][1], an automated deployment server for professional .NET developers. You can use them to create and deploy releases, create and push packages, and manage environments.

`Octo.exe` can be [downloaded from the Octopus downloads page][2], while `Octopus.Client` is [available on nuget.org][3]

## Documentation
- [Octo.exe][4]
- [Octopus.Client][5]

## Issues
Please see [Contributing](CONTRIBUTING.md)

## Compatibility
For:
- Octopus Server 3.0-3.2 use Octopus Client 3.0-3.2 (newer is better)
- Octopus Server 3.3 use Octopus Client 3.3 or newer release notes
- Octopus Server 3.4 or newer use Octopus Client 3.4 or newer release notes

## Versioning
As of `3.5.0` we started using semantic versioning][6] for the client library, breaking away from the practice of syncing the version to Octopus Server.

The [semantic versioning][6] applies to the command line interface of `Octo.exe` and the public api exposed by `Octopus.Client`. The `minor` version will be increased on feature additions, and `major` on breaking changes.

## vNext
As of Version 4, network operations in `Octopus.Client` will be `async`. We will continue to support version 3 for some time once version 4 is released, but may not add all new features.

[1]: https://octopus.com
[2]: https://octopus.com/downloads
[3]: https://www.nuget.org/packages/Octopus.Client
[4]: http://docs.octopusdeploy.com/display/OD/Octo.exe+Command+Line
[5]: http://docs.octopusdeploy.com/display/OD/Octopus.Client
[6]: http://semver.org/
