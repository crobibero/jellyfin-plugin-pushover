<h1 align="center">Jellyfin Pushover Plugin</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org/">Jellyfin Project</a></h3>

## About

This plugin enables the sending of messages / notifications to a range of devices via <a href="https://pushover.net/">Pushover.</a>

## Build & Installation Process

1. Clone this repository
2. Ensure you have .NET Core SDK set up and installed
3. Build the plugin with your favorite IDE or the `dotnet` command.

```
dotnet publish --configuration Release --output bin
```

4. Place the resulting `Jellyfin.Plugin.Pushover.dll` file in a folder called `plugins/` inside your Jellyfin data directory.

### Screenshot

<img src=screenshot.png>
