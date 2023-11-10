﻿using Microsoft.Xna.Framework;
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
        public int ReductionThreshold { get; set; }

        /// <summary>
        /// An internally used timer to help dynamically reduce <see cref="Concentration"/>
        /// </summary>
        public int ReductionTimer { get; private set; }

        public override void ResetEffects() => InteractionTimer++;

        public override void PreUpdateMovement()
        {
            foreach (Point tilePosition in Player.TouchedTiles)
            {
                if (Main.IsTileSpelunkable(tilePosition.X, tilePosition.Y))
                {
                    Concentration++;
                    InteractionTimer = 0;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (InteractionTimer >= ReductionThreshold)
            {
                ReductionTimer++;
                // Hard-coded to take an hour before 
                int threshold = Math.Clamp(60 - (InteractionTimer / ReductionThreshold), 1, 60);
                if (ReductionTimer % threshold == 0)
                {
                    Concentration--;
                }
            }

            foreach (HeavyMetalEffect heavyMetalEffect in LoadedEffects)
            {
                heavyMetalEffect.Effect(Concentration);
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

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