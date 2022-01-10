using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MegaOverhaul
{
    public abstract class Module
    {
        public ModEntry ModEntry { get; }

        public Config Config => ModEntry.Config;

        public IMonitor Monitor => this.ModEntry.Monitor;

        public IModEvents Events => this.ModEntry.Helper.Events;

        public Module(ModEntry modEntry) => this.ModEntry = modEntry;

        public bool isActive { get; protected set; } = false;

        public abstract void Activate();

        public abstract void Deactivate();
    }
}
