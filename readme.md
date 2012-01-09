# Octopus Command Line Tools

[Octopus][1] is an automated release management server for professional .NET developers.  *Octo.exe* is a command line tool for managing your Octpus installation and triggering releases. 

## General usage

All commands take the form of:

    octo command [<options>]

You can get help using:

    octo help [command]

## Example: creating a release

Assuming you have a project named *HelloWorld*:

    octo create-release --server=http://your-octopus/api --project=HelloWorld
    
This will create a new release of the *HelloWorld* project using the latest available NuGet packages for each step in the project. The version number of the release will be the highest NuGet package version. You can override this using:

    octo create-release --server=http://your-octopus/api --project=HelloWorld --version=1.0.3

To create a release *and* deploy it to an environment named *Production*:

    octo create-release --server=http://your-octopus/api --project=HelloWorld --deployto=Production

Note that packages that have already been deployed to the selected machines will not be re-deployed. You can force them to be re-deployed using the `--force` argument:

    octo create-release --server=http://your-octopus/api --project=HelloWorld --deployto=Production --force

## API Keys

All commands take an API key. This is for future usage, and isn't supported by Octopus just yet. For now, this tool only works when your Octopus installation is available anonymously. 

[1]: http://octopusdeploy.com 