using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Forms;
using System.Windows.Controls;

namespace User.PluginSdkDemo
{
    
    [PluginName("Demo plugin")]
    public class DataPluginDemo : IPlugin, IDataPlugin, IWPFSettings
    {
        private int SpeedWarningLevel = 100;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// called one time per game data update
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            pluginManager.SetPropertyValue("CurrentDateTime", this.GetType(), DateTime.Now);

            if (data.GameRunning)
            {
                if (data.OldData != null && data.NewData != null)
                {
                    if (data.OldData.SpeedKmh < SpeedWarningLevel && data.OldData.SpeedKmh >= SpeedWarningLevel)
                    {
                        pluginManager.TriggerEvent("SpeedWarning", this.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/displose anything needed here !
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
        }

        /// <summary>
        /// Return you winform settings control here, return null if no settings control
        /// 
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Forms.Control GetSettingsControl(PluginManager pluginManager)
        {
            return null;
        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControlDemo();
        }

        /// <summary>
        /// Called after plugins startup
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            pluginManager.AddProperty("CurrentDateTime", this.GetType(), DateTime.Now);

            pluginManager.AddEvent("SpeedWarning", this.GetType());

            pluginManager.AddAction("IncrementSpeedWarning", this.GetType(), (a, b) =>
            {
                this.SpeedWarningLevel++;
            });

            pluginManager.AddAction("DecrementSpeedWarning", this.GetType(), (a, b) =>
            {
                this.SpeedWarningLevel--;
            });
        }
    }
}