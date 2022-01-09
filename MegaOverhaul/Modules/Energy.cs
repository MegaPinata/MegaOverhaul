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

        public Energy(ModEntry modEntry) : base(modEntry)
        {
        }

        public override void Activate()
        {
            Events.GameLoop.DayStarted += NewDayEnergyCheck;
            Events.GameLoop.UpdateTicked += HalfEnergyUse;

            Events.GameLoop.OneSecondUpdateTicked += AddRestEnergy;
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
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

                // print character energy level if different
                //ModEntry.LogDebug($"{Game1.player.Name} : {Game1.player.stamina} / {Game1.player.maxStamina.Value} energy. diff:{diff}");
            }
            else if (_curEnergy > _lasEnergy)
            {
                float diff = _curEnergy - _lasEnergy;
                //ModEntry.LogDebug($"{Game1.player.Name} : {Game1.player.stamina} / {Game1.player.maxStamina.Value} energy. diff:{diff}");
            }
            //else
                //ModEntry.LogDebug($"{Game1.player.Name} : {Game1.player.stamina} / {Game1.player.maxStamina.Value} energy. diff:0");
        }

        public void AddRestEnergy(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            _lastTimeInterval = _curTimeInterval;
            _curTimeInterval = Game1.gameTimeInterval;

            //ModEntry.LogDebug($"_lastTimeInterval: {_lastTimeInterval}\t_curTimeInterval: {_curTimeInterval}");

            if ((Game1.player.isInBed.Value || Game1.player.isSitting.Value) && Game1.player.stamina < Game1.player.MaxStamina && _lastTimeInterval != _curTimeInterval)
                Game1.player.stamina += Config.RestEnergyGain;
        }

       
    }
}
