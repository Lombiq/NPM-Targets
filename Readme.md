# Lombiq NPM MSBuild Targets

[![Lombiq.Npm.Targets NuGet](https://img.shields.io/nuget/v/Lombiq.Npm.Targets?label=Lombiq.Npm.Targets)](https://www.nuget.org/packages/Lombiq.Npm.Targets/)

## About

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder that will be automatically compiled into the _wwwroot_ folder on build, which then can be excluded from the version control system. These operations are optimized by running them only if the corresponding files have been changed.

We at [Lombiq](https://lombiq.com/) also used this utility for the following projects:

- The new [City of Santa Monica website](https://santamonica.gov/) when migrating it from Orchard 1 to Orchard Core ([see case study](https://lombiq.com/blog/helping-the-city-of-santa-monica-with-orchard-core-consulting)).
- The new [Smithsonian Folkways Recordings website](https://folkways.si.edu/) when migrating it from Orchard 1 to Orchard Core ([see case study](https://lombiq.com/blog/smithsonian-folkways-recordings-now-upgraded-to-orchard-core)).
- The new [Lombiq website](https://lombiq.com/) when migrating it from Orchard 1 to Orchard Core ([see case study](https://lombiq.com/blog/how-we-renewed-and-migrated-lombiq-com-from-orchard-1-to-orchard-core)).
- The new client portal for [WTW](https://www.wtwco.com/) ([see case study](https://lombiq.com/blog/lombiq-s-journey-with-wtw-s-client-portal)).

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

Also see our [Node.js Extensions](https://github.com/Lombiq/NodeJs-Extensions) project, which contains complete asset pipelines built on top of this project.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## How to use

Install the [NuGet package](https://www.nuget.org/packages/Lombiq.Npm.Targets/), or if you use the project from a submodule, add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files of this project.

```xml
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

The `npm install` (or `pnpm install`, see below) command will be executed but only if the _package.json_ file exists and has been changed since the last build (i.e. you un/installed or up/downgraded packages). Note that if you update NPM then the _package.json_ and _package-lock.json_ files can change on `npm install`; currently, [there's no way to prevent this](https://github.com/npm/cli/issues/564) (`npm ci` is much slower).

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process, which can be utilized to run a custom task. This will only happen if the files defined in `NpmDotnetPrebuildWatchedFiles` have changed (to achieve [incremental build](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-incrementally?view=vs-2019)). By default, these files contain _package.json_, _Assets/\*\*/\*.\*_, _Scripts/\*\*/\*.\*_, and _Styles/\*\*/\*.\*_. If you want to adjust this list to cover your custom folders and files, override the given MSBuild item in your project file as follows:

```xml
<ItemGroup>
  <NpmDotnetPrebuildWatchedFiles Include="package.json;CustomFolder/**/*.*" />
</ItemGroup>
```

If you want to utilize this, then add a `dotnet-prebuild` script to the _package.json_ file as follows (`npm run build` is just an example command):

```json
{
  "private": true,
  "scripts": {
    "dotnet-prebuild": "npm run build"
  }
}
```

Similarly, you can execute `npm run dotnet-postclean --if-present` via the `dotnet-postclean` script to clean up anything after an MSBuild `Clean`, for example:

```json
{
  "scripts": {
    "dotnet-postclean": "npm run clean"
  }
}
```

## Using PNPM

[PNPM](https://pnpm.io/) is a faster and more efficient package manager. If it's available in the current shell, NPM Targets will use `pnpm` instead of `npm` to restore packages.

For details on setting up PNPM, refer to [these prerequisites](https://github.com/Lombiq/NodeJs-Extensions#prerequisites).

If you want to keep compatibility with NPM, then you have to maintain both _pnpm-lock.yaml_ and _package-lock.json_ files, since PNPM uses its own package lock file.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
