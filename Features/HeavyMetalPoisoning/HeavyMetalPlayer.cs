using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WellsFargosDifficulty.Features.HeavyMetalPoisoning
{
    public class HeavyMetalPlayer : ModPlayer
    {
        /// <summary>
        /// All loaded <see cref="HeavyMetalEffect"/>.
        /// This will be used to apply effects based on <see cref="HeavyMetalConcentration"/>.
        /// </summary>
        public static IList<HeavyMetalEffect> LoadedEffects { get; private set; } = new List<HeavyMetalEffect>();

        /// <summary>
        /// How much heavy metal is 'in' a player.
        /// This is used to dynamically apply effects.
        /// For reference, a concentration value of 60 means that the player has been near one tile for 60 ticks, or two tiles for 30 ticks, or three tiles for 20 ticks, etc.
        /// </summary>
        public int HeavyMetalConcentration { get; private set; }

        /// <summary>
        /// The amount of time since the player has last been near a heavy metal tile.
        /// This is used to dynamically reduce <see cref="HeavyMetalConcentration"/>.
        /// </summary>
        public int HeavyMetalInteractionTimer { get; private set; }

        /// <summary>
        /// The value that <see cref="HeavyMetalInteractionTimer"/> needs to meet before <see cref="HeavyMetalConcentration"/> starts decreasing.
        /// </summary>
        public int HeavyMetalReductionThreshold { get; set; }

        /// <summary>
        /// An internally used timer to help dynamically reduce <see cref="HeavyMetalConcentration"/>
        /// </summary>
        public int HeavyMetalReductionTimer { get; private set; }

        public override void ResetEffects() => HeavyMetalInteractionTimer++;

        public override void PreUpdateMovement()
        {
            foreach (Point tilePosition in Player.TouchedTiles)
            {
                if (Main.IsTileSpelunkable(tilePosition.X, tilePosition.Y))
                {
                    HeavyMetalConcentration++;
                    HeavyMetalInteractionTimer = 0;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (HeavyMetalInteractionTimer >= HeavyMetalReductionThreshold)
            {
                HeavyMetalReductionTimer++;
                // Hard-coded to take an hour before 
                int threshold = Math.Clamp(60 - (HeavyMetalInteractionTimer / HeavyMetalReductionThreshold), 1, 60);
                if (HeavyMetalReductionTimer % threshold == 0)
                {
                    HeavyMetalConcentration--;
                }
            }

            foreach (HeavyMetalEffect heavyMetalEffect in LoadedEffects)
            {
                heavyMetalEffect.Effect(HeavyMetalConcentration);
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void SaveData(TagCompound tag)
        {
            if (HeavyMetalConcentration > 0)
            {
                tag[nameof(HeavyMetalConcentration)] = HeavyMetalConcentration;
            }

            if (HeavyMetalInteractionTimer > 0)
            {
                tag[nameof(HeavyMetalInteractionTimer)] = HeavyMetalInteractionTimer;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            HeavyMetalConcentration = tag.TryGet(nameof(HeavyMetalConcentration), out int value) ? value : 0;
            HeavyMetalInteractionTimer = tag.TryGet(nameof(HeavyMetalInteractionTimer), out value) ? value : 0;
        }
    }
}
