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
        /// <summary>The core audio controller.</summary>
        private static CoreAudioController _audioController = null;
        
        /// <summary>Monitors for changes across all devices.</summary>
        private static AudioDeviceChangeObserver _deviceChangeObserver = null;

        /// <summary>Monitors for changes for each default device type.</summary>
        private static AudioDeviceObserver _playbackDeviceObserver = null;
        private static AudioDeviceObserver _captureDeviceObserver = null;
        private static AudioDeviceObserver _commsDeviceObserver = null;

        // ------------------------------------------------- //

        public static void Init()
        {
            // Create our controller interface.
            _audioController = new CoreAudioController();

            // Create an observer to monitor for device changes.
            _deviceChangeObserver = new AudioDeviceChangeObserver();
            _audioController.AudioDeviceChanged.Subscribe(_deviceChangeObserver);

            // Get all currently active audio devices
            var activeDevices = _audioController.GetDevices(DeviceType.All, DeviceState.Active);
            
            /* Create observers for each default audio device type */
            // Default playback device
            var defaultPlaybackDevice = activeDevices.FirstOrDefault(d => d.DeviceType == DeviceType.Playback && d.IsDefaultDevice);
            if (defaultPlaybackDevice != null)
                _playbackDeviceObserver = _CreateObserver(defaultPlaybackDevice);
            else
                LogHelper.Error($"{nameof(DJsAudioStats)} couldn't find an active default playback device on the system.");

            // Default capture device
            var defaultCaptureDevice = activeDevices.FirstOrDefault(d => d.DeviceType == DeviceType.Capture && d.IsDefaultDevice);
            if (defaultCaptureDevice != null)
                _captureDeviceObserver = _CreateObserver(defaultCaptureDevice);
            else
                LogHelper.Error($"{nameof(DJsAudioStats)} couldn't find an active default capture device on the system.");

            // Default communications device
            var defaultCommsDevice = activeDevices.FirstOrDefault(d => d.DeviceType == DeviceType.Capture && d.IsDefaultCommunicationsDevice);
            if (defaultCommsDevice != null)
                _commsDeviceObserver = _CreateObserver(defaultCommsDevice);
            else
                LogHelper.Error($"{nameof(DJsAudioStats)} couldn't find an active default communications device on the system.");


        }

        // ------------------------------------------------- //

        private static AudioDeviceObserver _CreateObserver(IDevice device)
        {
            // Log the device type and name
            var typeString = device.DeviceType.ToString().ToLower();
            if (device.IsDefaultCommunicationsDevice)
                typeString = "communications";
            LogHelper.Log($"{nameof(DJsAudioStats)} default {typeString} device is {device.FullName}");

            // Return a new observer for the device
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

                        // Handle the new default device
                        if (value.Device.IsPlaybackDevice && value.Device.IsDefaultDevice)
                            _playbackDeviceObserver = _CreateObserver(value.Device);
                        else if (value.Device.IsCaptureDevice)
                        {
                            if (value.Device.IsDefaultDevice)
                                _captureDeviceObserver = _CreateObserver(value.Device);
                            if (value.Device.IsDefaultCommunicationsDevice)
                                _commsDeviceObserver = _CreateObserver(value.Device);
                        }
                        

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

        public static double AudioCaptureVolume => _captureDeviceObserver != null ? _captureDeviceObserver.volume : 0;
        public static bool AudioCaptureMuted => _captureDeviceObserver != null ? _captureDeviceObserver.isMuted : false;
        public static double AudioCaptureTimeSinceLastChange => _captureDeviceObserver != null ? _captureDeviceObserver.timeSinceLastChange : 0;
        public static string AudioCaptureDevice => _captureDeviceObserver != null ? _captureDeviceObserver.deviceName : string.Empty;
        public static string AudioCaptureDeviceName => _captureDeviceObserver != null ? _captureDeviceObserver.deviceFullName : string.Empty;

        public static double AudioCommunicationVolume => _commsDeviceObserver != null ? _commsDeviceObserver.volume : 0;
        public static bool AudioCommunicationMuted => _commsDeviceObserver != null ? _commsDeviceObserver.isMuted : false;
        public static double AudioCommunicationTimeSinceLastChange => _commsDeviceObserver != null ? _commsDeviceObserver.timeSinceLastChange : 0;
        public static string AudioCommunicationDevice => _commsDeviceObserver != null ? _commsDeviceObserver.deviceName : string.Empty;
        public static string AudioCommunicationDeviceName => _commsDeviceObserver != null ? _commsDeviceObserver.deviceFullName : string.Empty;

    }
}
