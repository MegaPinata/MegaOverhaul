using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaOverhaul.Modules.Multiplayer;

namespace MegaOverhaul
{
    public class Config
    {
        public Config() { }
        public Config(MultiConfig multiConfig)
        {
            this.EnergyModsActive = multiConfig.EnergyModsActive;
            this.EnergyLossDivisor = multiConfig.EnergyLossDivisor;
            this.RestEnergyGain = multiConfig.RestEnergyGain;
            this.SpeedBoost_Active = multiConfig.SpeedBoost_Active;
            this.SpeedBoost = multiConfig.SpeedBoost;
        }

        public bool EnergyModsActive { get; set; } = true;

        public float EnergyLossDivisor { get; set; } = 2;

        public float RestEnergyGain { get; set; } = 3;

        public bool SpeedBoost_Active { get; set; } = true;

        public int SpeedBoost { get; set; } = 2;
    }
}
