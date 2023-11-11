using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning
{
    public class HeavyMetalPlayer : ModPlayer
    {
        /// <summary>
        /// All loaded <see cref="HeavyMetalEffect"/>.
        /// This will be used to apply effects based on <see cref="Concentration"/>.
        /// </summary>
        public static IList<HeavyMetalEffect> LoadedEffects { get; private set; } = new List<HeavyMetalEffect>();

        /// <summary>
        /// How much heavy metal is 'in' a player.
        /// This is used to dynamically apply effects.
        /// For reference, a concentration value of 60 means that the player has been near one tile for 60 ticks, or two tiles for 30 ticks, or three tiles for 20 ticks, etc.
        /// </summary>
        public int Concentration { get; private set; }

        /// <summary>
        /// The amount of time since the player has last been near a heavy metal tile.
        /// This is used to dynamically reduce <see cref="Concentration"/>.
        /// </summary>
        public int InteractionTimer { get; private set; }

        /// <summary>
        /// The value that <see cref="InteractionTimer"/> needs to meet before <see cref="Concentration"/> starts decreasing.
        /// </summary>
        public int ReductionThreshold { get; set; } = 3600;

        /// <summary>
        /// The time, in minutes, for maximum <see cref="Concentration"/> reduction to occur.
        /// </summary>
        public int MaximumReductionTime { get; set; } = 10;

        /// <summary>
        /// An internally used timer to help dynamically reduce <see cref="Concentration"/>
        /// </summary>
        public int ReductionTimer { get; private set; }

        public override void ResetEffects() => InteractionTimer = Math.Clamp(InteractionTimer + 1, 0, ReductionThreshold * MaximumReductionTime);

        public override void PreUpdateMovement()
        {
            Point tilePositon = Player.Center.ToTileCoordinates();
            for (int i = tilePositon.X - 1; i <= tilePositon.X + 2; i++)
            {
                for (int j = tilePositon.Y - 2; j <= tilePositon.Y + 3; j++)
                {
                    if (!WorldGen.InWorld(i, j))
                    {
                        continue;
                    }

                    Tile tile = Framing.GetTileSafely(i, j);
                    if (Main.tileSolid[tile.TileType] && Main.IsTileSpelunkable(i, j))
                    {
                        Concentration++;
                        InteractionTimer = 0;
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            foreach (HeavyMetalEffect heavyMetalEffect in LoadedEffects)
            {
                heavyMetalEffect.BadLifeRegenEffects(Player, Concentration);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (InteractionTimer >= ReductionThreshold)
            {
                ReductionTimer++;
                int threshold = Math.Clamp(MaximumReductionTime - (InteractionTimer / ReductionThreshold), 1, MaximumReductionTime);
                if (ReductionTimer % threshold == 0)
                {
                    Concentration--;
                }
            }

            foreach (HeavyMetalEffect heavyMetalEffect in LoadedEffects)
            {
                heavyMetalEffect.GeneralEffects(Player, Concentration);
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            foreach (HeavyMetalEffect heavyMetalEffect in LoadedEffects)
            {
                heavyMetalEffect.PreKillEffects(Player, Concentration, damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
            }
            return true;
        }

        public override void UpdateDead()
        {
            Concentration = 0;
            InteractionTimer = 0;
        }
 
        public override void Unload() => LoadedEffects.Clear();

        public override void SaveData(TagCompound tag)
        {
            if (Concentration > 0)
            {
                tag[nameof(Concentration)] = Concentration;
            }

            if (InteractionTimer > 0)
            {
                tag[nameof(InteractionTimer)] = InteractionTimer;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            Concentration = tag.TryGet(nameof(Concentration), out int value) ? value : 0;
            InteractionTimer = tag.TryGet(nameof(InteractionTimer), out value) ? value : 0;
        }
    }
}
