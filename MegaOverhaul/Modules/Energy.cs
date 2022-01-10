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

        private int _curTimeInterval = 0;
        private int _lastTimeInterval = 0;

        private float _curEnergyTime = 0;
        private float _lasEnergyTime = 0;

        public Energy(ModEntry modEntry) : base(modEntry)
        {
        }

        public override void Activate()
        {
            Events.GameLoop.DayStarted += NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked += HalfEnergyUse;

            Events.GameLoop.OneSecondUpdateTicked += AddRestEnergy;

            Events.GameLoop.TimeChanged += LogEnergyDif;
        }

        public override void Deactivate()
        {
            Events.GameLoop.DayStarted -= NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked -= HalfEnergyUse;

            Events.GameLoop.OneSecondUpdateTicked -= AddRestEnergy;
            Events.GameLoop.TimeChanged -= LogEnergyDif;
        }


        /// <summary>
        /// Resets cur and las values for energy at the start of a new day
        /// </summary>
        public void NewDayEnergyCheck(object sender, DayStartedEventArgs e)
        {
            _curEnergy = Game1.player.stamina;
            _lasEnergy = _curEnergy;

            _curEnergyTime = Game1.player.stamina;
            _lasEnergyTime = _curEnergyTime;
        }

        /// <summary>
        /// This is a Debug Function not intended for release
        /// </summary>
        public void LogEnergyDif(object sender, TimeChangedEventArgs e)
        {
            _lasEnergyTime = _curEnergyTime;
            _curEnergyTime = _curEnergy;

            ModEntry.LogDebug($"Energy Dif : {_curEnergyTime - _lasEnergyTime}");
        }

        /// <summary>
        /// On a GameLoop.UpdateTicked, this will add half of the energy used back to the player
        /// </summary>
        public void HalfEnergyUse(object sender, UpdateTickedEventArgs e)
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
            }
            else if (_curEnergy > _lasEnergy)
            {
                float diff = _curEnergy - _lasEnergy;
            }
        }

        public void AddRestEnergy(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            int multiplayerOffset = Config.RestEnergyGain - 2;
            if(multiplayerOffset < 0)
            {
                multiplayerOffset = 0;
            }

            if (Game1.IsMultiplayer)
            {
                if (Game1.player.isInBed.Value && Game1.shouldTimePass())
                {
                    if (Game1.player.stamina + multiplayerOffset >= Game1.player.maxStamina.Value)
                    {
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    }
                    else
                    {
                        Game1.player.stamina += multiplayerOffset;
                    }
                }
                if (Game1.player.isSitting.Value && Game1.shouldTimePass())
                {
                    if (Game1.player.stamina + Config.RestEnergyGain >= Game1.player.maxStamina.Value)
                    {
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    }
                    else
                    {
                        Game1.player.stamina += Config.RestEnergyGain;
                    }
                }
            }
            else
            {
                if ((Game1.player.isInBed.Value || Game1.player.isSitting.Value) && Game1.shouldTimePass(true))
                {
                    if (Game1.player.stamina + Config.RestEnergyGain >= Game1.player.maxStamina.Value)
                        Game1.player.stamina = Game1.player.maxStamina.Value;
                    else
                        Game1.player.stamina += Config.RestEnergyGain;
                }
            }


        }
    }
}
