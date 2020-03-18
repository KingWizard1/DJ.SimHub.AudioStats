# DJ's Audio Stats Plugin for SimHub
 A SimHub plugin that provides information about your default audio playback device.

The plugin is based off the SimHub Plugin SDK that comes bundled with the SimHub application software. The plugin was built using Visual Studio 2019 and uses [XenoLighting's .NET AudioSwitcher library](https://github.com/xenolightning/AudioSwitcher) which is referenced in the project.


# Build
1. Clone the project into your SimHub PluginSdk folder. By default, this should be at "C:\Program Files (x86)\SimHub\PluginSdk\". It is important that the project is cloned into this folder as it references several DLL's that are only provided by the SimHub software in its installation directory.
2. Open the project in Visual Studio and hit Build. The plugin should build to the root of the SimHub install folder. VS should automatically pull any libraries for the plugin to work (if required).
3. Restart SimHub. It should detect the new plugin and prompt you to enable it.


# Installation
1. Download the latest version of the plugin from the Releases page.
2. Drop all .DLL files directly into the root of the SimHub installation directory. The default folder for SimHub is "C:\Program Files (x86)\SimHub\"
 * Start SimHub. It should detect and prompt you to enable the plugin 


# Available Properties
Property | Type | Description
------------ | ------------- | -------------
AudioPlaybackDevice | string | The short name of your default device as determined by Windows (e.g. "Speakers", "Headset")
AudioPlaybackDeviceName | string | The full name of your default device (e.g. "Realtek HD Audio")
AudioPlaybackMuted | bool | Whether or not the device is muted. NOTE: Some devices report true if the volume is set to 0.
AudioPlaybackTimeSinceLastChange | int | The number of seconds that have elapsed since a change was detected on the device (e.g. volume change)
AudioPlaybackVolume | int | The current volume level of the device.
