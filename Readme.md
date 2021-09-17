# Lombiq NPM MSBuild Targets



## About

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder that will be automatically compiled into the _wwwroot_ folder on build, which then can be excluded from the version control system.

Note, that these operations are optimized by running them only if the corresponding files have been changed.

Also see our [Gulp Extensions](https://github.com/Lombiq/Gulp-Extensions) library that contains some useful Gulp helpers.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!


## How to use

Add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files in this project.

```
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

The `npm install` (or `pnpm install`, see below) command will be executed but only if the _package.json_ file exists and has been changed since the last build (i.e. you un/installed packages or up/downgraded ones). Note that if you update NPM then the _package.json_ and _package-lock.json_ files can change on `npm install`; currently, [there's no way to prevent this](https://github.com/npm/cli/issues/564) (`npm ci` is much slower).

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process which can be utilized to run a Gulp task for example. This will only happen if the files defined in `NpmDotnetPrebuildWatchedFiles` have changed (to achieve [incremental build](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-incrementally?view=vs-2019)).

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this (`gulp build` is just an example of a command you can run):

```
{
  "private": true,
  "scripts": {
    "dotnet-prebuild": "gulp build"
  }
}
```

Similarly, you can execute `npm run dotnet-postclean --if-present` via the `dotnet-postclean` script to clean up anything after an MSBuild `Clean`, for example:

```
"scripts": {
  "dotnet-postclean": "gulp clean"
}
```


## Using pnpm for package restore

[Pnpm](https://pnpm.io/) is a faster and more efficient package manager. If it's installed globally, then the module will use that instead of *npm* to restore packages.

To install *pnpm* globally run this command: `npm install pnpm -g`. Once it's complete, the module will automatically use that to restore packages.


## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
