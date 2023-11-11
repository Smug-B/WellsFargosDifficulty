using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace WellsFargosDifficulty.Features.AdaptiveEconomy
{
    public class InflationSystem : ModSystem
    {
        /// <summary>
        /// The total rise in cost since the mod was loaded.
        /// Increases at 4:30 AM each day at a rate determined by <see cref="RateGenerator"/>
        /// </summary>
        public float TotalMultiplier { get; private set; } = 1f;

        /// <summary>
        /// Used to generate <see cref="DailyRate"/> in a 'predictable' manner.
        /// </summary>
        public WeightedRandom<float> RateGenerator { get; private set; } = new WeightedRandom<float>(
           new Tuple<float, double>(0.99f, 0.1),
           new Tuple<float, double>(0.995f, 0.15),
           new Tuple<float, double>(1.00f, 0.25),
           new Tuple<float, double>(1.005f, 0.5),
           new Tuple<float, double>(1.01f, 0.66),
           new Tuple<float, double>(1.015f, 0.75),
           new Tuple<float, double>(1.02f, 1),
           new Tuple<float, double>(1.025f, 0.75),
           new Tuple<float, double>(1.03f, 0.66),
           new Tuple<float, double>(1.035f, 0.5),
           new Tuple<float, double>(1.04f, 0.25),
           new Tuple<float, double>(1.045f, 0.15),
           new Tuple<float, double>(1.05f, 0.1));

        public override void Load()
        {
            void ApplyInflationRates(On_Player.orig_GetItemExpectedPrice orig, Player self, Item item, out long calcForSelling, out long calcForBuying)
            {
                orig(self, item, out calcForSelling, out calcForBuying);
                calcForBuying = (long)(TotalMultiplier * calcForBuying);
            }

            On_Player.GetItemExpectedPrice += ApplyInflationRates;
        }

        public override void PostUpdateEverything()
        {
            if (Main.dayTime && Main.time == 0)
            {
                TotalMultiplier *= RateGenerator.Get();
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (TotalMultiplier > 0)
            {
                tag[nameof(TotalMultiplier)] = TotalMultiplier;
            }
        }

        public override void NetSend(BinaryWriter writer) => writer.Write(TotalMultiplier);

        public override void LoadWorldData(TagCompound tag)
        {
            TotalMultiplier = tag.TryGet(nameof(TotalMultiplier), out float value) ? value : 0;
        }

        public override void NetReceive(BinaryReader reader) => TotalMultiplier = reader.ReadSingle();
    }
}
