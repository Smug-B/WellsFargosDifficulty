using Terraria;
using Terraria.DataStructures;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning.Effects
{
    public class OrganDamage : HeavyMetalEffect
    {
        public override void BadLifeRegenEffects(Player player, int concentration)
        {
            if (concentration <= 3600)
            {
                return;
            }

            player.lifeRegenTime = 0;

            if (player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }

            int reductionThreshold = player.GetModPlayer<HeavyMetalPlayer>().ReductionThreshold;
            player.lifeRegen -= (concentration - reductionThreshold) / (reductionThreshold / 10);
        }

        public override void PreKillEffects(Player player, int concentration, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (concentration <= 3600)
            {
                return;
            }

            if (damageSource.SourceProjectileLocalIndex == -1 && damageSource.SourceNPCIndex == -1 && damageSource.SourcePlayerIndex == -1 && damageSource.SourceCustomReason == null)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s organs failed.");
            }
        }
    }
}
