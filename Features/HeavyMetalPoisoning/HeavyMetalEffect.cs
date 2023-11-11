using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning
{
    public abstract class HeavyMetalEffect : ILoadable
    {
        public void Load(Mod mod) => HeavyMetalPlayer.LoadedEffects.Add(this);

        public void Unload() { }

        public virtual void GeneralEffects(Player player, int concentration) { }

        public virtual void BadLifeRegenEffects(Player player, int concentration) { }

        public virtual void PreKillEffects(Player player, int concentration, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) { }
    }
}
