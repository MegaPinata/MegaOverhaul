using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Netcode;


namespace MegaOverhaul.Modules
{
    class FenceLife
    {

        private readonly IList<GameLocation> searchedLocations = (IList<GameLocation>)new List<GameLocation>();

        internal IEnumerable<Fence> GetFences()
        {
            this.searchedLocations.Clear();
            if (Context.IsWorldReady)
            {
                foreach (Fence f in this.GetFences((IEnumerable<GameLocation>)Game1.locations))
                    yield return f;
            }
        }

        private IEnumerable<Fence> GetFences(IEnumerable<GameLocation> locations)
        {
            foreach (Fence f in locations.SelectMany<GameLocation, Fence>(new Func<GameLocation, IEnumerable<Fence>>(this.GetFences)))
                yield return f;
        }

        private IEnumerable<Fence> GetFences(GameLocation l)
        {
            if (l != null && !this.searchedLocations.Contains(l))
            {
                this.searchedLocations.Add(l);
                foreach (Fence f in l.Objects.Values)
                    yield return f;
                if (l is BuildableGameLocation bLoc)
                {
                    foreach (Fence f in this.GetFences(((IEnumerable<Building>)bLoc.buildings).Where<Building>((Func<Building, bool>)(item => item != null)).Select<Building, GameLocation>((Func<Building, GameLocation>)(item => ((NetFieldBase<GameLocation, NetRef<GameLocation>>)item.indoors).Value)).Where<GameLocation>((Func<GameLocation, bool>)(item => item != null))))
                        yield return f;
                }
            }
        }

        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (Fence fence in GetFences())
            {
                fence.repair();
                NetFloat health1 = fence.health;
                health1.Value = health1.Value * 2f;
                fence.maxHealth.Value = fence.health.Value;
                if (fence.isGate.Value)
                {
                    NetFloat health2 = fence.health;
                    health2.Value = health2.Value * 2f;
                }
            }
        }
    }
}

