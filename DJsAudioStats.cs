using AudioSwitcher.AudioApi.CoreAudio;
using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace DJ.SimHub.AudioStats
{
    [PluginDescription("Provides information about your default audio playback device.")]
    [PluginAuthor("djphilos412")]
    [PluginName("DJ's Audio Stats")]
    public class DJsAudioStats : IPlugin, IDataPlugin, IWPFSettings
    {
        
        public AudioStatsPluginSettings Settings;


        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {

            LogHelper.Log($"Starting {nameof(DJsAudioStats)}");
            

            // Load settings
            Settings = this.ReadCommonSettings<AudioStatsPluginSettings>("GeneralSettings", () => new AudioStatsPluginSettings());


            // Declare a property available in the property list
            //pluginManager.AddProperty("CurrentDateTime", this.GetType(), DateTime.Now);


            AudioStats.Init();
            pluginManager.AddProperty(nameof(AudioStats.AudioPlaybackMuted), this.GetType(), typeof(bool));
            pluginManager.AddProperty(nameof(AudioStats.AudioPlaybackVolume), this.GetType(), typeof(int));
            pluginManager.AddProperty(nameof(AudioStats.AudioPlaybackDevice), this.GetType(), typeof(string));
            pluginManager.AddProperty(nameof(AudioStats.AudioPlaybackDeviceName), this.GetType(), typeof(string));
            pluginManager.AddProperty(nameof(AudioStats.AudioPlaybackTimeSinceLastChange), this.GetType(), typeof(int));

            //SimHub.Logging.Current.Info(AudioStats.GetVolume());

            LogHelper.Log($"{nameof(DJsAudioStats)} initialised OK");

            return;

            // Declare an event 
            pluginManager.AddEvent("SpeedWarning", this.GetType());

            // Declare an action which can be called
            pluginManager.AddAction("IncrementSpeedWarning", this.GetType(), (a, b) =>
            {
                Settings.SpeedWarningLevel++;
                global::SimHub.Logging.Current.Info("Speed warning changed");
            });

            // Declare an action which can be called
            pluginManager.AddAction("DecrementSpeedWarning", this.GetType(), (a, b) =>
            {
                Settings.SpeedWarningLevel--;
            });
        }
        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here ! 
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            //return new SettingsControlDemo(this);
            return null;
        }

        /// <summary>
        /// Called one time per game data update, contains all normalized game data, 
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        /// 
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        /// 
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Define the value of our property (declared in init)
            //pluginManager.SetPropertyValue("CurrentDateTime", this.GetType(), DateTime.Now.Second);

            /* Audio Stats */
            pluginManager.SetPropertyValue(nameof(AudioStats.AudioPlaybackMuted), this.GetType(), AudioStats.AudioPlaybackMuted);
            pluginManager.SetPropertyValue(nameof(AudioStats.AudioPlaybackVolume), this.GetType(), AudioStats.AudioPlaybackVolume);
            pluginManager.SetPropertyValue(nameof(AudioStats.AudioPlaybackDevice), this.GetType(), AudioStats.AudioPlaybackDevice);
            pluginManager.SetPropertyValue(nameof(AudioStats.AudioPlaybackDeviceName), this.GetType(), AudioStats.AudioPlaybackDeviceName);
            pluginManager.SetPropertyValue(nameof(AudioStats.AudioPlaybackTimeSinceLastChange), this.GetType(), AudioStats.AudioPlaybackTimeSinceLastChange);


            //if (data.GameRunning)
            //{
            //    if (data.OldData != null && data.NewData != null)
            //    {
            //        if (data.OldData.SpeedKmh < Settings.SpeedWarningLevel && data.OldData.SpeedKmh >= Settings.SpeedWarningLevel)
            //        {
            //            // Trigger an event
            //            pluginManager.TriggerEvent("SpeedWarning", this.GetType());
            //        }
            //    }
            //}
        }

    }
}