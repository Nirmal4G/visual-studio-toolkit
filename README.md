# Community toolkit for Visual Studio extensions

A community driven effort for a better Visual Studio experience developing extensions.

[![Build status](https://ci.appveyor.com/api/projects/status/0p4wvtwuj55qixhr?svg=true)](https://ci.appveyor.com/project/madskristensen/community-visualstudio-toolkit-1dwx1)
[![NuGet](https://img.shields.io/nuget/v/Community.VisualStudio.Toolkit)](https://nuget.org/packages/Community.VisualStudio.Toolkit/)

The NuGet package [Community.VisualStudio.Toolkit](https://www.nuget.org/packages/Community.VisualStudio.Toolkit/) acts as a companion to the regular Visual Studio SDK packages with helper methods, classes and extension methods that makes writing extensions a lot easier. 

* [Community.VisualStudio.Toolkit](https://www.nuget.org/packages/Community.VisualStudio.Toolkit/) official NuGet package
* [CI NuGet feed](https://ci.appveyor.com/nuget/community-visualstudio-toolkit) for nightly builds
* [API Documentation](https://vsixcommunity.github.io/Community.VisualStudio.Toolkit/v1/api/)

## Examples
Here are some examples of typical scenarios used in extensions.

### Commands
You can now write commands much simpler than you're used to. Here's what a command that opens a tool window would look like:

```c#
[Command(PackageIds.RunnerWindow)]
internal sealed class RunnerWindowCommand : BaseCommand<RunnerWindowCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) =>
        await RunnerWindow.ShowAsync();
}
```

### Writing to the statusbar

``` C#
await VS.Notifiations.SetStatusbarTextAsync("My statusbar text");
```

### Showing a message box

``` C#
VS.Notifiations.ShowMessage("Title", "Message");
```

### Log error to output window
Synchronous version:

``` C#
catch (Exception ex)
{
    ex.Log();
}
```

Async version:

``` C#
catch (Exception ex)
{
    await ex.LogAsync();
}
```

## Purpose
This package attempts to solve multiple issues with the current extensibility model.

### Too much boilerplate is needed to do simple things
Base classes, helper methods, and extension methods encapsulate the complexity so you don't have to. 

### It's difficult to find what services and components to use
Now the most commmon services are all easy to get to from the main `VS` object. For instance, to write to the Statusbar, you can now write the following:


### Best practices change with each version of VS. I can't keep up
The underlying implementation of the project uses the best practices for each version of VS it supports. This ensures that your extension is much more likely to handle threading correctly, and avoid hangs and crashes.


### The API is dated and has lots of ugly COM legacy noise
The most common APIs of the old complex COM nature are wrapped to expose a modern async API. This makes it much easier to code against the API and you can avoid the `EnvDTE` object for most scenarios.

### The API isn't async and getting threading right is too hard
All the base classes and helper methods are async by default. There are cases where they are not, but that is because it wouldn't be beneficial for them to be. 

### Only Microsoft can update the API and that doesn't scale
This is a living project where the whole community can contribute helpers on top of the official VS SDK. There is no need to wait for Microsoft to make an update, since this projet gives the ability to continue the work in a separate work stream.

### Breaking changes in the API between VS version are painful
This project works around those changes in the implementation of its public contracts and interfaces. This means that what was a breaking change to the VS SDK, becomes an implementation detail of this project and no user will be affected.


## Templates
For both project- and item templates that utilizes this NuGet packages, download the [Extensibility Template Pack](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityItemTemplates).