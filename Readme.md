# Lombiq NPM MSBuild Targets

[![Lombiq.Npm.Targets NuGet](https://img.shields.io/nuget/v/Lombiq.Npm.Targets?label=Lombiq.Npm.Targets)](https://www.nuget.org/packages/Lombiq.Npm.Targets/)

## About

Provides automatic NPM package installation and a custom NPM command execution before building a .NET project. This way it is possible for example to manage assets (e.g. .scss files or images) in a folder that will be automatically compiled into the _wwwroot_ folder on build, which then can be excluded from the version control system.

Note, that these operations are optimized by running them only if the corresponding files have been changed.

Also see our [Gulp Extensions](https://github.com/Lombiq/Gulp-Extensions) library that contains some useful Gulp helpers.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## How to use

Install the [NuGet package](https://www.nuget.org/packages/Lombiq.Npm.Targets/) or if you use the project from a submodule, add the following lines to the csproj file where the _package.json_ file is. Make sure that the paths are pointing to the _Lombiq.Npm.Targets.props_ and _Lombiq.Npm.Targets.targets_ files of this project.

```xml
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.props" />
<Import Project="path\to\Lombiq.Npm.Targets\Lombiq.Npm.Targets.targets" />
```

The `npm install` (or `pnpm install`, see below) command will be executed but only if the _package.json_ file exists and has been changed since the last build (i.e. you un/installed packages or up/downgraded ones). Note that if you update NPM then the _package.json_ and _package-lock.json_ files can change on `npm install`; currently, [there's no way to prevent this](https://github.com/npm/cli/issues/564) (`npm ci` is much slower).

An `npm run dotnet-prebuild --if-present` script will be also executed during the build process which can be utilized to run a Gulp task for example. This will only happen if the files defined in `NpmDotnetPrebuildWatchedFiles` have changed (to achieve [incremental build](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-build-incrementally?view=vs-2019)). By default, these files contain _package.json_, _Assets/\*\*/\*.\*_, _Scripts/\*\*/\*.\*_, and _Styles/\*\*/\*.\*_. If you want to adjust this list to cover your custom folders and files, override the given item group in your project file as follows:

```xml
<ItemGroup>
  <NpmDotnetPrebuildWatchedFiles Include="package.json;CustomFolder/**/*.*" />
</ItemGroup>
```

If you want to utilize this then add a `dotnet-prebuild` script to the _package.json_ file like this (`gulp build` is just an example of a command you can run; you can also run the default gulp command with just `gulp`):

```json
{
  "private": true,
  "scripts": {
    "dotnet-prebuild": "gulp build"
  }
}
```

Similarly, you can execute `npm run dotnet-postclean --if-present` via the `dotnet-postclean` script to clean up anything after an MSBuild `Clean`, for example:

```json
{
  "scripts": {
    "dotnet-postclean": "gulp clean"
  }
}
```

## Using PNPM for package restore

[PNPM](https://pnpm.io/) is a faster and more efficient package manager. If it's installed globally, then the module will use `pnpm` instead of `npm` to restore packages.

### Installation and usage

To instaall PNPM globally, run this command: `npm install pnpm -g`. Once it's complete, the module will automatically use that to restore packages.

### Notes

- PNPM supports restoring packages directly to a directory so it's not necessary to move _node_modules_ to a parent directory anymore.
- It uses its own package lock file. So if you want to keep NPM compatibility, then you have to maintain both _pnpm-lock.yaml_ and _package-lock.json_ files if you want to support both.
- It installs the latest package dependencies unless it's overriden from the _package.json_ file. For example the latest _sass_ is installed along with _gulp-dart-sass_ that might [cause issues with Bootstrap 4](https://github.com/twbs/bootstrap/issues/34051). In this case it must be overriden with a lower version.

## Global NPM vs Userspace NPM via Node Version Manager on Linux

If you installed NPM from your package manager, you will likely suffer an `EACCES` error when you try to install a package globally. The [recommended solution](https://docs.npmjs.com/resolving-eacces-permissions-errors-when-installing-packages-globally/#reinstall-npm-with-a-node-version-manager) is to install NPM via the Node Version Manager (NVM). This is what you should do:

Before we start, ensure your directories are set up:

```shell
[ -z "$XDG_CONFIG_HOME" ] && echo 'export XDG_CONFIG_HOME="$HOME/.config"' >> ~/.bashrc && source ~/.bashrc
mkdir -p "$XDG_CONFIG_HOME"
mkdir -p "$HOME/.local/bin"
```

Also if you have installed NPM globally via your package manager, uninstall it to avoid future confusion.

Next we install NVM and Node.js in userspace.

1. Install NVM for your user: `curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.1/install.sh | bash`
2. Type `nvm`.
    - If you get _nvm: command not found_ error, reload your shell profile first (`source ~/.bashrc`) and try again.
3. Install the latest Node.js with `nvm install node`.
    - If you are going to use Gulp and you have problems with Node.js 16.x (see [here](https://github.com/Lombiq/Orchard-Vue.js#prerequisites)), try downgrading to 14.x and make it the default: `nvm install 14.7.0 && nvm alias default 14.7.0`.

This is good enough for launching new apps, but for example MSBuild doesn't use a login shell. If you have a desktop environment (through a display manager) see if the following works on your system. If yes, you can skip the rest of this section.

1. Open the _~/.bashrc_ in a text editor.
2. Open or create the _~/.xsessionrc_ file in a text editor. This is the autorun file for your dektop login session.
3. Copy the new lines at the bottom of _~/.bashrc_ created by the NVM installer and append them to the end of the _~/.xsessionrc_.
4. Save everything, log out and log back it.
If everything went well you can now successfully build your `Lombiq.Npm.Targets`-using project as your IDE will already be in an NVM enabled environment when you start it up.

You need to set up some proxy commands for `node` and `npm`:

```shell
function proxy-nvm-command() {
    for command in "$@"; do
    
    cat > ~/.local/bin/$command << DONE
#!/bin/bash

export NVM_DIR="$HOME/.config/nvm"
source "\$NVM_DIR/nvm.sh"
exec $command "\$@"
DONE

    chmod +x ~/.local/bin/$command
    
    done
}

proxy-nvm-command node npm
```

If you wish to use PNPM too, type this:

```shell
npm install pnpm -g
proxy-nvm-command pnpm
```

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
