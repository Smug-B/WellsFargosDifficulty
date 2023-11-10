using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning
{
    public abstract class HeavyMetalEffect : ILoadable
    {
        public void Load(Mod mod) => HeavyMetalPlayer.LoadedEffects.Add(this);

        public void Unload() { }

        public abstract void Effect(Player player, int concentration);
    }
}
