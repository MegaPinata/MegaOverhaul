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
    public class SpeedBoost : Module
    {
        public SpeedBoost(ModEntry modEntry) : base(modEntry)
        {
        }

        public override void Activate()
        {
            if (!isActive)
            {
                isActive = true;

                Events.GameLoop.UpdateTicking += AddSpeedBoost;
            }
        }

        public override void Deactivate()
        {
            if (isActive)
            {
                this.isActive = false;

                Events.GameLoop.UpdateTicking -= AddSpeedBoost;
            }
        }

        public void ResetSpeedBoost()
        {
            Game1.player.addedSpeed = 0;
        }

        private void AddSpeedBoost(object sender, UpdateTickingEventArgs e)
        {
            if (Game1.player.isMoving() && Game1.player.addedSpeed == 0)
                Game1.player.addedSpeed = Config.SpeedBoost;
        }
    }
}
