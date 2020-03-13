# NPM MSBuild Targets



## Project Description

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder which will be automatically compiled into the _wwwroot_ folder on build which can be excluded from the version control system.

Note, that these operations are optimized by running them only if the corresponding files have been changed (i.e. `npm install` will only be executed if the _package.json_ file has been changed).


## How to use

Add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files in this project.

```
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process which can be utilized to run a Gulp task for example.

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this:

```
"scripts": {
  "dotnet-prebuild": "gulp build"
},
```

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**.
Feel free to send pull requests too, no matter which source repository you choose for this purpose.

This project is developed by [Lombiq Technologies Ltd](http://lombiq.com/). Commercial-grade support is available through Lombiq.