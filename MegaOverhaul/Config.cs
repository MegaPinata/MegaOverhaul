using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOverhaul
{
    public class Config
    {
        public bool EnergyModsActive { get; set; } = true;

        public float EnergyLossDivisor { get; set; } = 2;

        public float RestEnergyGain { get; set; } = 3;

        public bool SpeedBoost_Active { get; set; } = true;

        public int SpeedBoost { get; set; } = 2;
    }
}
