# DJ's Audio Stats Plugin for SimHub
 A SimHub plugin that provides information about your default audio playback, recording and communications devices on your system. The plugin exposes the name and volume properties of each device, and automatically detects when a default device changes.

The plugin is based off the SimHub Plugin SDK that comes bundled with the SimHub application software. The plugin was built using Visual Studio 2019 and uses [XenoLighting's .NET AudioSwitcher library](https://github.com/xenolightning/AudioSwitcher) which is referenced in the project.


# Available Properties
The plugin allows you to retrieve the following properties for each of your default device. The list below will appear three times in SimHub, with each set corresponding to your default playback, default capture, and default communications device's as set in the Windows Audio control panel.

Property | Type | Description
------------ | ------------- | -------------
AudioDevice | string | The short name of your default device as determined by Windows (e.g. "Speakers", "Headset")
AudioDeviceName | string | The full name of your default device (e.g. "Realtek HD Audio")
AudioMuted | bool | Whether or not the device is muted. NOTE: Some devices report true if the volume is set to 0.
AudioTimeSinceLastChange | int | The number of seconds that have elapsed since a change was detected on the device (e.g. volume change)
AudioVolume | int | The current volume level of the device.


# Demo
Here is a link to a YouTube video showcasing how the plugin can be used. The video shows an Arduino controlling a custom built dashboard using TM1638 displays.

SimHub can read the plugin's properties and display their values on the TM1638 displays. From top to bottom we can see the name of the default playback device, the current volume level as a number, and the current volume level represented as a series of green and red LED's.

[![Audio plugin demo video using TM1638's](http://img.youtube.com/vi/egcoY04dHTk/0.jpg)](http://www.youtube.com/watch?v=egcoY04dHTk "Audio plugin demo video using TM1638's")


# Build
1. Clone the project into your SimHub PluginSdk folder. By default, this should be at "C:\Program Files (x86)\SimHub\PluginSdk\". It is important that the project is cloned into this folder as it references several DLL's that are only provided by the SimHub software in its installation directory.
2. Open the project in Visual Studio and hit Build. The plugin should build to the root of the SimHub install folder. VS should automatically pull any libraries for the plugin to work (if required).
3. Restart SimHub. It should detect the new plugin and prompt you to enable it.


# Installation
1. Download the latest version of the plugin from the Releases page.
2. Drop all .DLL files directly into the root of the SimHub installation directory. The default folder for SimHub is "C:\Program Files (x86)\SimHub\"
3. Start SimHub. It should detect and prompt you to enable the plugin 
