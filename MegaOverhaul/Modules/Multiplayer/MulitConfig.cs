using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOverhaul.Modules.Multiplayer
{
    public class MultiConfig : Config
    {
        public bool IsMainPlayer { get; set; }

        public void setMultiConfig(Config config)
        {
            this.EnergyModsActive = config.EnergyModsActive;
            this.SpeedBoost_Active = config.SpeedBoost_Active;

            this.EnergyLossDivisor = config.EnergyLossDivisor;
            this.RestEnergyGain = config.RestEnergyGain;
            this.SpeedBoost = config.SpeedBoost;
        }

        public void setMultiConfig(MultiConfig hostConfig) {
            this.EnergyModsActive = hostConfig.EnergyModsActive;
            this.SpeedBoost_Active = hostConfig.SpeedBoost_Active;

            this.EnergyLossDivisor = hostConfig.EnergyLossDivisor;
            this.RestEnergyGain = hostConfig.RestEnergyGain;
            this.SpeedBoost = hostConfig.SpeedBoost;
        }
    }
}
