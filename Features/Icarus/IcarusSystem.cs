using log4net.Repository.Hierarchy;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.Icarus
{
    public class IcarusSystem : ModPlayer
    {
        public override void Load() => IL_Player.Update += ApplyIcarusFolly;

        private void ApplyIcarusFolly(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(57)))
            {
                return;
            }

            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Stloc, 57);
        }

        public override void PreUpdateMovement()
        {
            if (Player.velocity.Y <= 0f)
            {
                Player.fallStart = (int)(Player.position.Y / 16f);
            }
        }
    }
}
