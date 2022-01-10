using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using Netcode;
using MegaOverhaul.Modules;


namespace MegaOverhaul
{
    public class ModEntry : Mod
    {
        public static Energy EnergyMod;

        public static Config Config { get; private set; }
        public static IModHelper StaticHelper { get; private set; }
        public static IMonitor StaticMonitor { get; private set; }

        public static void LogDebug(string s, LogLevel l = LogLevel.Debug) => ModEntry.StaticMonitor.Log(s, l);

        

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.Config = helper.ReadConfig<Config>();
            ModEntry.StaticMonitor = this.Monitor;
            ModEntry.StaticHelper = this.Helper;

            helper.Events.GameLoop.SaveLoaded += (EventHandler<SaveLoadedEventArgs>)((sender, e) => this.LoadModules());
            helper.Events.GameLoop.ReturnedToTitle += (EventHandler<ReturnedToTitleEventArgs>)((sender, e) => this.UnloadModules());

            //helper.Events.GameLoop.ReturnedToTitle += SaveModOptions;
        }

        private void LoadModules()
        {
            ModEntry.EnergyMod = new Energy(this);
            EnergyMod.Activate();
        }

        private void UnloadModules()
        {
            EnergyMod.Deactivate();
            ModEntry.EnergyMod = null;
        }

        //private void SaveModOptions(object sender, ReturnedToTitleEventArgs e)
        //{
        //    Helper.WriteConfig(Config);
        //    LogDebug("Config File Written");
        //}
    }
}
