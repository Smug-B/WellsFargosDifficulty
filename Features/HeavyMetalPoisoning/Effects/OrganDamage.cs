using Terraria;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning.Effects
{
    public class OrganDamage : HeavyMetalEffect
    {
        public override void BadLifeRegenEffects(Player player, int concentration)
        {
            player.lifeRegenTime = 0;

            if (player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }

            int reductionThreshold = player.GetModPlayer<HeavyMetalPlayer>().ReductionThreshold;
            player.lifeRegen -= (concentration - reductionThreshold) / (reductionThreshold / 10);
        }
    }
}
