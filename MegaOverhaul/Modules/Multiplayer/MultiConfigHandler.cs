using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Netcode;
using MegaOverhaul.Modules.Multiplayer;

namespace MegaOverhaul.Modules.Multiplayer
{
    public class MultiConfigHandler : Module
    {
        public MultiConfigHandler(ModEntry modEntry) : base(modEntry)
        {
            isActive = true;
        }

        public override void Activate()
        {
            Events.GameLoop.SaveLoaded += DefineMainPlayer;
            Events.GameLoop.SaveLoaded += GenMultiConfig;
            Events.Multiplayer.PeerConnected += SendClientConfig;
            Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        private void DefineMainPlayer(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.player.IsMainPlayer)
                ModEntry.MultiConfig.IsMainPlayer = true;
            else
                ModEntry.MultiConfig.IsMainPlayer = false;
        }

        private void GenMultiConfig(object sender, SaveLoadedEventArgs e)
        {
            if (ModEntry.MultiConfig.IsMainPlayer)
            {
                ModEntry.MultiConfig.set(ModEntry.Config);

                ModEntry.LogDebug($"EnergyModsActive: {ModEntry.MultiConfig.EnergyModsActive}");
                ModEntry.LogDebug($"EnergyLossDivisor: {ModEntry.MultiConfig.EnergyLossDivisor}");
                ModEntry.LogDebug($"RestEnergyGain: {ModEntry.MultiConfig.RestEnergyGain}");
                ModEntry.LogDebug($"SpeedBoostActive: {ModEntry.MultiConfig.SpeedBoost_Active}");
                ModEntry.LogDebug($"SpeedBoost: {ModEntry.MultiConfig.SpeedBoost}");

            }
        }

        private void SendClientConfig(object sender, PeerConnectedEventArgs e)
        {
            foreach (var mod in e.Peer.Mods)
            {
                if (mod.ID == ModEntry.ModManifest.UniqueID)
                {
                    SendClientConfig();
                }
            }

        }

        public void SendClientConfig()
        {
            ModEntry.MultiConfig.set(ModEntry.Config);

            ModEntry.StaticHelper.Multiplayer.SendMessage<MultiConfig>(
                        message: ModEntry.MultiConfig,
                        messageType: "SendClientConfig",
                        modIDs: new[]{
                            ModEntry.ModManifest.UniqueID
                        }
                        );
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModEntry.ModManifest.UniqueID)
            {
                if(e.Type == "SendClientConfig")
                {
                    if (!ModEntry.MultiConfig.IsMainPlayer)
                    {
                        ModEntry.MultiConfig.set(e.ReadAs<MultiConfig>());
                        ModEntry.LogDebug($"EnergyModsActive: {ModEntry.MultiConfig.EnergyModsActive}");
                        ModEntry.LogDebug($"EnergyLossDivisor: {ModEntry.MultiConfig.EnergyLossDivisor}");
                        ModEntry.LogDebug($"RestEnergyGain: {ModEntry.MultiConfig.RestEnergyGain}");
                        ModEntry.LogDebug($"SpeedBoostActive: {ModEntry.MultiConfig.SpeedBoost_Active}");
                        ModEntry.LogDebug($"SpeedBoost: {ModEntry.MultiConfig.SpeedBoost}");

                        if (ModEntry.EnergyMod != null)
                            ModEntry.EnergyMod.InitValues();
                    }
                }
            }
        }

    }
}
