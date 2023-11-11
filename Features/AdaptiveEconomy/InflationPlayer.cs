using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.AdaptiveEconomy
{
    public class InflationPlayer : ModPlayer
    {
        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price) 
            => price = (int)(price * ModContent.GetInstance<InflationSystem>().TotalMultiplier);
    }
}