# Lombiq NPM MSBuild Targets



[![Lombiq.Npm.Targets NuGet](https://img.shields.io/nuget/v/Lombiq.Npm.Targets?label=Lombiq.Npm.Targets)](https://www.nuget.org/packages/Lombiq.Npm.Targets/)


## About

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder that will be automatically compiled into the _wwwroot_ folder on build, which then can be excluded from the version control system.

Note, that these operations are optimized by running them only if the corresponding files have been changed.

Also see our [Gulp Extensions](https://github.com/Lombiq/Gulp-Extensions) library that contains some useful Gulp helpers.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!


## How to use

Install the [NuGet package](https://www.nuget.org/packages/Lombiq.Npm.Targets/) or if you use the project from a submodule, add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files of this project.

```
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

The `npm install` (or `pnpm install`, see below) command will be executed but only if the _package.json_ file exists and has been changed since the last build (i.e. you un/installed packages or up/downgraded ones). Note that if you update NPM then the _package.json_ and _package-lock.json_ files can change on `npm install`; currently, [there's no way to prevent this](https://github.com/npm/cli/issues/564) (`npm ci` is much slower).

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process which can be utilized to run a Gulp task for example. This will only happen if the files defined in `NpmDotnetPrebuildWatchedFiles` have changed (to achieve [incremental build](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-incrementally?view=vs-2019)).

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this (`gulp build` is just an example of a command you can run; you can also run the default gulp command with just `gulp`):

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


## Global vs Userpace NPM on Linux

If you installed NPM from your package manager, you will likely suffer an `EACCES` error when you try to install a package globally. The [recommended solution](https://docs.npmjs.com/resolving-eacces-permissions-errors-when-installing-packages-globally#reinstall-npm-with-a-node-version-manager) is to install NPM via the Node Version Manager (NVM). This is what you should do:

Before we start, ensure your directories are set up:
```shell
[ -z "$XDG_CONFIG_HOME" ] && echo 'export XDG_CONFIG_HOME="$HOME/.config"' >> ~/.bashrc && source ~/.bashrc
mkdir -p "$XDG_CONFIG_HOME"
mkdir -p "$HOME/.local/bin"
```

Next we install NVM and Node.js in userspace.
1. Install NVM for your user: `curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.1/install.sh | bash`
2. Type `nvm`. 
    - If you get _nvm: command not found_ error, reload your shell profile first (`source ~/.bashrc`) and try again.
3. Install the latest Node.js with `nvm install node`.
    - If you are going to use Gulp, you need to use 14.x (see [here](https://github.com/Lombiq/Orchard-Vue.js#prerequisites)) and make it the default: `nvm install 14.7.0 && nvm alias default 14.7.0`.

This is good enough for development, but for example MSBuild doesn't use a login shell. So you need to set up some proxy commands for `node` and `npm`.

```shell
for command in node npm; do

cat > ~/.local/bin/$command << DONE
#!/bin/bash

export NVM_DIR="$HOME/.config/nvm"
source "\$NVM_DIR/nvm.sh"
exec $command "\$@"
DONE

chmod +x ~/.local/bin/$command

done
```


## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
