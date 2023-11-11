using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning
{
    public abstract class HeavyMetalEffect : ILoadable
    {
        public void Load(Mod mod) => HeavyMetalPlayer.LoadedEffects.Add(this);

        public void Unload() { }

        public virtual void GeneralEffects(Player player, int concentration) { }

        public virtual void BadLifeRegenEffects(Player player, int concentration) { }
    }
}
