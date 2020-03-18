using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace DJ.SimHub.AudioStats
{
    public static class AudioStats
    {

        private static CoreAudioController _audioController = null;

        private static AudioDeviceChangeObserver _deviceChangeObserver = null;
        private static AudioDeviceObserver _playbackDeviceObserver = null;

        // ------------------------------------------------- //

        public static void Init()
        {
            // Create our controller interface.
            _audioController = new CoreAudioController();

            // Create an observer to monitor for device changes.
            _deviceChangeObserver = new AudioDeviceChangeObserver();
            _audioController.AudioDeviceChanged.Subscribe(_deviceChangeObserver);

            // Create an observer for the current default playback device.
            var activePlaybackDevices = _audioController.GetPlaybackDevices(DeviceState.Active);
            var defaultPlaybackDevice = activePlaybackDevices.FirstOrDefault(d => d.IsDefaultDevice);
            if (defaultPlaybackDevice != null)
                _playbackDeviceObserver = CreateNewPlaybackDeviceObserver(defaultPlaybackDevice);
            else
                LogHelper.Error($"{nameof(DJsAudioStats)} couldn't find a default playback device on the system.");

            // K, go
            LogHelper.Log($"{nameof(DJsAudioStats)} initialised");

        }

        // ------------------------------------------------- //

        private static AudioDeviceObserver CreateNewPlaybackDeviceObserver(IDevice device)
        {
            LogHelper.Log($"{nameof(DJsAudioStats)} default playback device is {device.FullName}");
            return new AudioDeviceObserver(device);
        }

        // ------------------------------------------------- //

        private class AudioDeviceChangeObserver : IObserver<DeviceChangedArgs>
        {
            /* Not used */
            public void OnCompleted() { }
            public void OnError(Exception error) { }

            // ------------------------------------------------- //

            public void OnNext(DeviceChangedArgs value)
            {
                //LogHelper.Log($"{nameof(AudioDeviceChangeObserver)}.OnNext(): {value.ChangedType}");

                switch (value.ChangedType)
                {
                    case DeviceChangedType.DefaultChanged:

                        // For default device changes, OnNext() will be called TWICE.
                        // The first call will be for the device being UNSET as the default device.
                        // The second call will be for the device being SET as the default device.

                        // Ignore the device that isn't the default
                        if (!value.Device.IsDefaultDevice)
                            return;

                        // Handle the new default device
                        _playbackDeviceObserver = CreateNewPlaybackDeviceObserver(value.Device);

                        break;

                    default:
                    case DeviceChangedType.DeviceAdded:
                    case DeviceChangedType.DeviceRemoved:
                    case DeviceChangedType.PropertyChanged:
                    case DeviceChangedType.StateChanged:
                    case DeviceChangedType.PeakValueChanged:
                    case DeviceChangedType.MuteChanged:
                    case DeviceChangedType.VolumeChanged:
                        break;
                }

            }
        }

        // ------------------------------------------------- //

        private class AudioDeviceObserver : IObserver<DeviceMuteChangedArgs>, IObserver<DeviceVolumeChangedArgs>
        {
            public int volume { get; private set; }
            public bool isMuted { get; private set; }
            public string deviceName { get; private set; }
            public string deviceFullName { get; private set; }

            private DateTime _lastChangeAt;
            public int timeSinceLastChange
            {
                get
                {
                    // Cap the total seconds so it doesn't go on forever and overflow
                    var secondsSinceLastChange = (DateTime.Now - _lastChangeAt).TotalSeconds;
                    secondsSinceLastChange = Math.Min(secondsSinceLastChange, 9999);
                    return Convert.ToInt32(secondsSinceLastChange); // automatically rounds to the nearest
                }
            }

            // ------------------------------------------------- //

            public AudioDeviceObserver(IDevice device)
            {
                // Subscribe
                device.VolumeChanged.Subscribe(this);
                device.MuteChanged.Subscribe(this);

                // Initial values
                volume = Convert.ToInt32(device.Volume);
                isMuted = device.IsMuted;
                deviceName = device.Name;
                deviceFullName = device.FullName;
                _lastChangeAt = DateTime.Now;
            }

            // ------------------------------------------------- //

            /* Not used */
            public void OnCompleted() { }
            public void OnError(Exception error) { }

            // ------------------------------------------------- //

            public void OnNext(DeviceVolumeChangedArgs value)
            {
                volume = Convert.ToInt32(value.Volume); // automatically rounds to the nearest
                _lastChangeAt = DateTime.Now;
            }

            public void OnNext(DeviceMuteChangedArgs value)
            {
                isMuted = value.IsMuted;
                _lastChangeAt = DateTime.Now;
            }

        }

        // ------------------------------------------------- //

        //public static double audioPlaybackVolume => 0;
        //public static bool audioPlaybackMuted => false;
        //public static double audioPlaybackTimeSinceLastChange => 0;

        public static double AudioPlaybackVolume => _playbackDeviceObserver != null ? _playbackDeviceObserver.volume : 0;
        public static bool AudioPlaybackMuted => _playbackDeviceObserver != null ? _playbackDeviceObserver.isMuted : false;
        public static double AudioPlaybackTimeSinceLastChange => _playbackDeviceObserver != null ? _playbackDeviceObserver.timeSinceLastChange : 0;
        public static string AudioPlaybackDevice => _playbackDeviceObserver != null ? _playbackDeviceObserver.deviceName : string.Empty;
        public static string AudioPlaybackDeviceName => _playbackDeviceObserver != null ? _playbackDeviceObserver.deviceFullName : string.Empty;

    }
}
