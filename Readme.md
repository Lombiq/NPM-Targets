# NPM MSBuild Targets



## About

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder that will be automatically compiled into the _wwwroot_ folder on build, which then can be excluded from the version control system.

Note, that these operations are optimized by running them only if the corresponding files have been changed.

Also see our [Gulp Extensions](https://github.com/Lombiq/Gulp-Extensions) library that contains some useful Gulp helpers.


## How to use

Add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files in this project.

```
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

The `npm install` command will be executed but only if the _package.json_ file exists and has been changed since the last build (i.e. you un/installed packages or up/downgraded ones).

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process which can be utilized to run a Gulp task for example. This will only happen if the files defined in `NpmDotnetPrebuildWatchedFiles` have changed (to achieve [incremental build](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-incrementally?view=vs-2019)).

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this:

```
"scripts": {
  "dotnet-prebuild": "gulp build"
},
```

Similarly, you can execute `npm run dotnet-postclean --if-present` via the `dotnet-postclean` script to clean up anything after an MSBuild `Clean`, for example:

```
"scripts": {
  "dotnet-postclean": "gulp clean"
}
```


## Contribution and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
