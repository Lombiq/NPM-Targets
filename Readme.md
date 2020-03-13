# NPM MSBuild Targets



## Project Description

Provides automatic NPM package installation and a custom NPM command execution before building the .NET project. This way it is possible to manage assets (e.g. .scss files or images) in a folder which will be automatically compiled into the _wwwroot_ folder which is exluded from version control system.

Note, that this operations are optimized by running them only if the corresponding files are changed (i.e. `npm install` will only be executed if the _package.json_ file has been changed).


## How to use

Add the following lines to the csproj file where the _package.json_ file is.

```
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

Make sure that the paths are correct.

It will also execute a `npm run dotnet-prebuild --if-present` script which can be utilized to run a Gulp task for example.

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this:

```
"scripts": {
  "dotnet-prebuild": "gulp build"
},
```

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**.
Feel free to send pull requests too, no matter which source repository you choose for this purpose.

This project is developed by [Lombiq Technologies Ltd](http://lombiq.com/). Commercial-grade support is available through Lombiq.