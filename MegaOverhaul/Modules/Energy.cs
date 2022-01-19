using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MegaOverhaul.Modules.Multiplayer;

namespace MegaOverhaul.Modules
{
    public class Energy : Module
    {
        public bool valChanged = false;

        private float _curEnergy = 0;
        private float _lasEnergy = 0;

        private float _multiplayerOffset;
        private float _updateInterval;
        private float _multiUpdateInterval;
        private float _singleplayerUpdate;
        private float _multiOffsetUpdate;
        private float _multiplayerUpdate;


        public Energy(ModEntry modEntry) : base(modEntry)
        {
        }

        public override void Activate()
        {
            Events.GameLoop.DayStarted += NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked += AdjustEnergyUse;

            Events.GameLoop.UpdateTicked += AddRestEnergy;

            Events.GameLoop.OneSecondUpdateTicked += (sender, e) => CheckConfigUpdated();

            Events.GameLoop.SaveLoaded += (sender, e) => InitValues();
        }

        public override void Deactivate()
        {
            Events.GameLoop.DayStarted -= NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked -= AdjustEnergyUse;

            Events.GameLoop.UpdateTicked -= AddRestEnergy;


            Events.GameLoop.OneSecondUpdateTicked -= (sender, e) => CheckConfigUpdated();
            Events.GameLoop.SaveLoaded -= (sender, e) => InitValues();
        }

        public void CheckConfigUpdated()
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null || !MultiConfig.IsMainPlayer)
                return;

            Monitor.LogOnce("Checking for Config Updates", LogLevel.Info);
            if (valChanged)
            {
                Monitor.Log("Changing Config Values", LogLevel.Info);
                InitValues();
                valChanged = false;
                ModEntry.MultiConfigHandler.SendClientConfig();
            }
        }

        public void InitValues()
        {
            ModEntry.LogDebug("Energy Values Initalized.");
            _multiplayerOffset = MultiConfig.RestEnergyGain - 2;
            if (_multiplayerOffset < 0)
                _multiplayerOffset = 0;


            _updateInterval = (float)Math.Floor(60 / Config.RestEnergyGain);
            _multiUpdateInterval = (float)Math.Floor(60 / MultiConfig.RestEnergyGain);
            _multiOffsetUpdate = _multiplayerOffset / (60 / _multiUpdateInterval);
            _multiplayerUpdate = MultiConfig.RestEnergyGain / (60 / _multiUpdateInterval);
            _singleplayerUpdate = Config.RestEnergyGain / (60 / _updateInterval);

            ModEntry.LogDebug($"EnergyLossDivisor: {Config.EnergyLossDivisor}");
            ModEntry.LogDebug($"RestEnergyGain: {Config.RestEnergyGain}");
            ModEntry.LogDebug($"_ui: {_updateInterval}");
            ModEntry.LogDebug($"_mou: {_multiOffsetUpdate}");
            ModEntry.LogDebug($"_mu: {_multiplayerUpdate}");
            ModEntry.LogDebug($"_su: {_singleplayerUpdate}");
        }

        /// <summary>
        /// Resets cur and las values for energy at the start of a new day
        /// </summary>
        public void NewDayEnergyCheck(object sender, DayStartedEventArgs e)
        {
            _curEnergy = Game1.player.stamina;
            _lasEnergy = _curEnergy;
        }

        /// <summary>
        /// On a GameLoop.UpdateTicked, this will add half of the energy used back to the player
        /// </summary>
        public void AdjustEnergyUse(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (MultiConfig.EnergyLossDivisor == 0)
            {
                Monitor.LogOnce("AEU - MC.ELD == 0", LogLevel.Error);
                return;
            }
            if (Config.EnergyLossDivisor == 0)
            {
                Monitor.LogOnce("AEU - C.ELD == 0", LogLevel.Error);
                return;
            }

            _lasEnergy = _curEnergy;
            _curEnergy = Game1.player.stamina;

            if (_curEnergy < _lasEnergy)
            {
                float diff = _curEnergy - _lasEnergy;

                if (Game1.IsMultiplayer)
                {
                    Game1.player.stamina += Math.Abs(diff / MultiConfig.EnergyLossDivisor);
                    ModEntry.LogDebug($"MultiELD: {MultiConfig.EnergyLossDivisor}");
                }
                else
                {
                    Game1.player.stamina += Math.Abs(diff / Config.EnergyLossDivisor);
                    ModEntry.LogDebug($"ELD: {Config.EnergyLossDivisor}");
                }
                
            }
        }

        public void AddRestEnergy(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if ((uint)_updateInterval == 0)
            {
                Monitor.LogOnce("ARE - _ui == 0", LogLevel.Error);
                return;
            }
            if ((uint)_multiUpdateInterval == 0)
            {
                Monitor.LogOnce("ARE - _mui == 0", LogLevel.Error);
            }

            if (Game1.IsMultiplayer)
            {
                if (!e.IsMultipleOf((uint)_multiUpdateInterval))
                    return;

                if (Game1.player.isInBed.Value && Game1.shouldTimePass())
                {
                    if (Game1.player.stamina + _multiplayerUpdate >= Game1.player.maxStamina.Value)
                    {
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    }
                    else
                    {
                        Game1.player.stamina += _multiplayerUpdate;
                    }
                }
                if (Game1.player.isSitting.Value && Game1.shouldTimePass())
                {
                    if (Game1.player.stamina + _singleplayerUpdate >= Game1.player.maxStamina.Value)
                    {
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    }
                    else
                    {
                        Game1.player.stamina += _singleplayerUpdate;
                    }
                }
            }
            else
            {
                if (!e.IsMultipleOf((uint)_updateInterval))
                    return;

                if ((Game1.player.isInBed.Value || Game1.player.isSitting.Value) && Game1.shouldTimePass(true))
                {
                    if (Game1.player.stamina + _singleplayerUpdate >= Game1.player.maxStamina.Value)
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    else
                        Game1.player.stamina += _singleplayerUpdate;
                }
            }


        }
    }
}
