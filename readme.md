# Octopus Deploy Command Line Tools

[Octopus Deploy][1] is an automated release management server for professional .NET developers.  *Octo.exe* is a command line tool for managing your Octpus installation and triggering releases. 

The tool can be [downloaded from the Octopus downloads page][2].

## General usage

All commands take the form of:

    octo command [<options>]

You can get help using:

    octo help [command]

All commands require you to pass the URL of the Octopus Server's API endpoint, and an API key. 

    octo ... --server=http://your-octopus-server/api --apiKey=ABCDEF123456

You'll find your API key in the Octopus web portal, by clicking on your profile:

![First, click on My Profile under your username](http://res.cloudinary.com/octopusdeploy/image/upload/v1366768866/2013_04_24_11_59_11_Dashboard_Octopus_ps9dhi.png)

And scrolling to the bottom:

![Then, scroll to the bottom to see your username](http://res.cloudinary.com/octopusdeploy/image/upload/v1366768867/2013_04_24_11_59_34_Configuration_Octopus_famfmz.png)

If your IIS server is set up to require Windows Authentication, the tool will automatically pass the currently logged in user's credentials. If this doesn't work, or you want to use a different user, simply add these two arguments:

    octo ... --user=fred --pass=secret

## Creating a release

This command allows you to create a release, and optionally deploy it to one or more environments. 

Usage: 

      octo create-release [<options>]

Where `[<options>]` is any of:

    --project=VALUE        Name of the project
    --deployto=VALUE       [Optional] Environment to automatically deploy
                             to, e.g., Production
    --releaseNumber, --version=VALUE
                           Release number to use for the new release.
    --defaultpackageversion, --packageversion=VALUE
                           Default version number of all packages to use
                             for this release.
    --package, --packageversionoverride=PackageId
                           [Optional] Version number to use for a package
                             in the release. Format: --
                             package=PackageId:Version
    --packagesFolder=VALUE [Optional] A folder containing NuGet packages
                             from which we should get versions.
    --forceversion         Ignored (obsolete).
    --force                Whether to force redeployment of already
                             installed packages (flag, default false).
    --releasenotes=VALUE   Release Notes for the new release.
    --releasenotesfile=VALUE
                           Path to a file that contains Release Notes for
                             the new release.
    --waitfordeployment    Whether to wait synchronously for deployment to
                             finish.
    --deploymenttimeout=VALUE
                           [Optional] Specifies maximum time (timespan
                             format) that deployment can take (default
                             00:10:00)
    --deploymentchecksleepcycle=VALUE
                           [Optional] Specifies how much time (timespan
                             format) should elapse between deployment status
                             checks (default 00:00:10)
#### Basic examples:

This will create a new release of the *HelloWorld* project using the latest available NuGet packages for each step in the project. The version number of the release will be the highest NuGet package version. You can override this using: 

    octo create-release --project=HelloWorld --server=http://octopus/api --apiKey=ABCDEF123456
    
This will create a release with a specified release number (note that this is not the NuGet package version number):

    octo create-release --project=HelloWorld --version=1.0.3 --server=http://octopus/api --apiKey=ABCDEF123456

#### Specifying the package version:

This will create a release (*1.0.3*) with a specified NuGet package version (*1.0.3*):

    octo create-release --project=HelloWorld --version=1.0.3 --packageversion=1.0.1 --server=http://octopus/api --apiKey=ABCDEF123456

This will create a release for a project with multiple packages, each with a different version:

    octo create-release --project=HelloWorld --version=1.0.3 --package=Hello.Web:1.0.1 --package=Hello.Server:1.0.2 --server=http://octopus/api --apiKey=ABCDEF123456

This will create a release for a project with multiple packages, by taking the version for each package from a folder containing the packages (this approach works well if your build server has just built the packages):

    octo create-release --project=HelloWorld --version=1.0.3 --packagesFolder=packages --server=http://octopus/api --apiKey=ABCDEF123456

#### Creating and deploying a release:

To create a release *and* deploy it to an environment named *Production*:

    octo create-release --project=HelloWorld --deployto=Production --server=http://octopus/api --apiKey=ABCDEF123456

Note that packages that have already been deployed to the selected machines will not be re-deployed. You can force them to be re-deployed using the `--force` argument:

    octo create-release --project=HelloWorld --deployto=Production --force --server=http://octopus/api --apiKey=ABCDEF123456

## Deploying a release

Usage: 

    octo deploy-release [<options>]

Where [<options>] is any of:

    --project=VALUE        Name of the project
    --deployto=VALUE       Environment to deploy to, e.g., Production
    --releaseNumber, --version=VALUE
                           Version number of the release to deploy.
    --force                Whether to force redeployment of already
                             installed packages (flag, default false).
    --waitfordeployment    Whether to wait synchronously for deployment to
                             finish.
    --deploymenttimeout=VALUE
                           [Optional] Specifies maximum time (timespan
                             format) that deployment can take (default
                             00:10:00)
    --deploymentchecksleepcycle=VALUE
                           [Optional] Specifies how much time (timespan
                             format) should elapse between deployment status
                             checks (default 00:00:10)

#### Basic examples:

This will deploy release 1.0.0 of the *HelloWorld* project to the *Production* environment:

    octo deploy-release --project=HelloWorld --releaseNumber=1.0.0 --deployto=Production --server=http://octopus/api --apiKey=ABCDEF123456

## Other commands

*Octo.exe* provides other commands, including:

 * `octo delete-releases`  
   Deletes a range of releases. See [this post](http://octopusdeploy.com/blog/deleting-releases-via-command-line) for details. 
 * `list-environments`  
   Lists all environments in the Octopus server


[1]: http://octopusdeploy.com 
[2]: http://octopusdeploy.com/downloads
