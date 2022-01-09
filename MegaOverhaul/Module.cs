using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MegaOverhaul
{
    public abstract class Module
    {
        public ModEntry modEntry { get; }

        public Config Config => ModEntry.Config;

        public IMonitor Monitor => this.modEntry.Monitor;

        public IModEvents Events => this.modEntry.Helper.Events;

        public Module(ModEntry modEntry) => this.modEntry = modEntry;

        public abstract void Activate();

        public abstract void Deactivate();
    }
}
