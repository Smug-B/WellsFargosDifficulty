using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.AdaptiveEconomy
{
    public class InflationItem : GlobalItem
    {
        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        {
            reforgePrice = (int)(reforgePrice * ModContent.GetInstance<InflationSystem>().TotalMultiplier);
            return false;
        }
    }
}