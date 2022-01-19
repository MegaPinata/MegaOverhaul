using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using Netcode;
using MegaOverhaul.Modules;
using MegaOverhaul.Modules.Multiplayer;
using MegaOverhaul.ConfigUI;


namespace MegaOverhaul
{
    public class ModEntry : Mod
    {
        public static Energy EnergyMod;
        public static SpeedBoost SpeedBoostMod;

        public Config Config { get; private set; } = new Config();
        public static MultiConfig MultiConfig { get; set; } = new MultiConfig();
        public static MultiConfigHandler MultiConfigHandler;

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
            this.Config = helper.ReadConfig<Config>();
            StaticMonitor = this.Monitor;
            StaticHelper = this.Helper;

            helper.Events.GameLoop.GameLaunched += (EventHandler<GameLaunchedEventArgs>)((sender, e) => this.LoadModules());
            helper.Events.GameLoop.GameLaunched += SetupConfigUI;
        }

        private void LoadModules()
        {
            EnergyMod = new Energy(this);
            SpeedBoostMod = new SpeedBoost(this);
            MultiConfigHandler = new MultiConfigHandler(this);
            if(Config.EnergyModsActive)
                EnergyMod.Activate();
            if(Config.SpeedBoost_Active)
                SpeedBoostMod.Activate();
            MultiConfigHandler.Activate();
        }

        private void UnloadModules()
        {
            EnergyMod.Deactivate();
            EnergyMod = null;
            SpeedBoostMod.Deactivate();
            SpeedBoostMod = null;
        }

        private void SetupConfigUI(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                LogDebug("ConfigMenu is null");
                return;
            }

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new Config(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Energy Mods"
                );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnergyModsActive,
                setValue: value => Config.EnergyModsActive = value,
                name: () => "Energy Mods Enabled?",
                fieldId: "configMenu.EnergyMods_Active"
                );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.EnergyLossDivisor,
                setValue: value => Config.EnergyLossDivisor = value,
                name: () => "Energy Loss Divisor",
                tooltip: () => "This is the number that your statmina usage will be divided by.\ne.g. 2 halves stamina usage, 4 quarters it, so on...",
                min: 1,
                interval: 1,
                fieldId: "configMenu.EnergyLossDivisorVal"
                );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.RestEnergyGain,
                setValue: value => Config.RestEnergyGain = value,
                name: () => "Resting Regen. Per Second",
                tooltip: () => "The amount of stamina you regenerate from 1 second of resting in a chair or in bed",
                min: 0,
                interval: 1,
                fieldId: "configMenu.RestEnergyGainVal"
                );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Speed Boost Mods"
                );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SpeedBoost_Active,
                setValue: value => Config.SpeedBoost_Active = value,
                name: () => "Speed Boost Mods Enabled?",
                fieldId: "configMenu.SpeedBoostActive"
                );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.SpeedBoost,
                setValue: value => Config.SpeedBoost = value,
                name: () => "Speed Boost",
                tooltip: () => "The amount you add to your player's speed.\nMax Value doubles your running speed",
                min: 0,
                max: 5,
                fieldId: "configMenu.SpeedBoostVal"
                );

            configMenu.OnFieldChanged(
                mod: ModManifest,
                onChange: FieldChanged
                );
        }

        private void FieldChanged(string str, object obj)
        {
            if (obj is float)
            {
                if (str.Equals("configMenu.EnergyLossDivisorVal") && MultiConfig.IsMainPlayer)
                {
                    ModEntry.EnergyMod.valChanged = true;
                }

                if (str.Equals("configMenu.RestEnergyGainVal") && MultiConfig.IsMainPlayer)
                {
                    ModEntry.EnergyMod.valChanged = true;
                }

                if (str.Equals("configMenu.SpeedBoostVal") && MultiConfig.IsMainPlayer)
                {
                    ModEntry.SpeedBoostMod.ResetSpeedBoost();
                }
            }
            if (obj is bool)
            {
                if (str.Equals("configMenu.EnergyMods_Active") && MultiConfig.IsMainPlayer)
                {
                    if ((bool)obj)
                    {
                        EnergyMod.Activate();
                        EnergyMod.valChanged = true;
                    }
                    else
                        EnergyMod.Deactivate();
                }
                if (str.Equals("configMenu.SpeedBoostActive") && MultiConfig.IsMainPlayer)
                {
                    if ((bool)obj)
                        SpeedBoostMod.Activate();
                    else
                        SpeedBoostMod.Deactivate();
                }
            }

        }
    }
}
