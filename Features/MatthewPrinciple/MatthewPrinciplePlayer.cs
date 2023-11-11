using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using WellsFargosDifficulty.Features.AdaptiveEconomy;

namespace WellsFargosDifficulty.Features.MatthewPrinciple
{
    public class MatthewPrinciplePlayer : ModPlayer
    {
        public const int PlatinumCoin = 1000000;

        /// <summary>
        /// A multiplier on how many coins an enemy drops build on the Matthew Principle that 'the rich get richer'
        /// In our case, coins will only begin dropping once a player has half a platinum* in their inventory.
        /// * = Subject to inflation, see <see cref="InflationSystem"/>.
        /// </summary>
        public float WealthAccruementMultiplier
        {
            get
            {
                int wealth = 0;
                foreach (Item item in Player.inventory)
                {
                    if (!item.IsACoin)
                    {
                        continue;
                    }
                    wealth += item.value * item.stack / 5;
                }
                int threshold = (int)(PlatinumCoin / 2 * ModContent.GetInstance<InflationSystem>().TotalMultiplier);
                return Math.Clamp((wealth / threshold) - 1, 0, 1);
            }
        }

        public override void Load()
        {
            IL_NPC.NPCLoot_DropMoney += ApplyMatthewPrinciple;
            IL_CoinsRule.TryDroppingItem += ApplyMatthewPrincipleForBags;
        }

        private void ApplyMatthewPrinciple(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld(out _), i => i.MatchConvR4(), i => i.MatchAdd(), i => i.MatchStloc0()))
            {
                return;
            }

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate<Func<Player, float, float>>((player, coins) => coins *= player.GetModPlayer<MatthewPrinciplePlayer>().WealthAccruementMultiplier);
            cursor.Emit(OpCodes.Stloc_0);
        }

        private void ApplyMatthewPrincipleForBags(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld(out _), i => i.MatchConvR8(), i => i.MatchLdloc0(), i => i.MatchMul()))
            {
                return;
            }

            cursor.EmitDelegate(() => (double)Main.LocalPlayer.GetModPlayer<MatthewPrinciplePlayer>().WealthAccruementMultiplier);
            cursor.Emit(OpCodes.Mul);
        }
    }
}
