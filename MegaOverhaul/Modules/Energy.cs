using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MegaOverhaul.Modules
{
    public class Energy : Module
    {
        private float _curEnergy = 0;
        private float _lasEnergy = 0;

        private float _multiplayerOffset;
        private float _updateInterval;
        private float _multiplayerUpdate;
        private float _singleplayerUpdate;


        public Energy(ModEntry modEntry) : base(modEntry)
        {
        }

        public override void Activate()
        {
            Events.GameLoop.DayStarted += NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked += AdjustEnergyUse;

            Events.GameLoop.UpdateTicked += AddRestEnergy;
            Events.GameLoop.DayStarted += OnDayStart;
        }

        public override void Deactivate()
        {
            Events.GameLoop.DayStarted -= NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked -= AdjustEnergyUse;

            Events.GameLoop.UpdateTicked -= AddRestEnergy;
            Events.GameLoop.DayStarted -= OnDayStart;
        }


        public void OnDayStart(object sender, DayStartedEventArgs e)
        {
            InitValues();
        }

        public void InitValues()
        {
            _multiplayerOffset = Config.RestEnergyGain - 2;
            if (_multiplayerOffset < 0)
                _multiplayerOffset = 0;


            _updateInterval = (float)Math.Floor(60 / Config.RestEnergyGain);
            _multiplayerUpdate = _multiplayerOffset / (60 / _updateInterval);
            _singleplayerUpdate = Config.RestEnergyGain / (60 / _updateInterval);

            ModEntry.LogDebug($"RestEnergyGain: {Config.RestEnergyGain}");
            ModEntry.LogDebug($"_ui: {_updateInterval}");
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

            _lasEnergy = _curEnergy;
            _curEnergy = Game1.player.stamina;

            if (_curEnergy < _lasEnergy)
            {
                float diff = _curEnergy - _lasEnergy;
                Game1.player.stamina += Math.Abs(diff / Config.EnergyLossDivisor);
                ModEntry.LogDebug($"ELD: {Config.EnergyLossDivisor}");
            }
        }

        public void AddRestEnergy(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if ((uint)_updateInterval == 0)
            {
                Monitor.LogOnce("Divide 0", LogLevel.Error);
                return;
            }

            if (!e.IsMultipleOf((uint)_updateInterval))
                return;

            if (Game1.IsMultiplayer)
            {
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
