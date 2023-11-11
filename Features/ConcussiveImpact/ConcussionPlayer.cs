using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace WellsFargosDifficulty.Features.ConcussiveImpact
{
    public class ConcussionPlayer : ModPlayer
    {
        public static Effect ConcussionEffect { get; private set; }

        public static Filter Concussion { get; private set; }

        public float ConcussionProgress
        {
            get => Internal_ConcussionProgress;
            set => Internal_ConcussionProgress = Math.Clamp(value, 0, 1);
        }

        private float Internal_ConcussionProgress = 1;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            Ref<Effect> concussionEffect = new Ref<Effect>(ModContent.Request<Effect>("WellsFargosDifficulty/Features/ConcussiveImpact/Effects/Concussion", AssetRequestMode.ImmediateLoad).Value);
            ConcussionEffect = concussionEffect.Value;
            Filters.Scene["WellsFargosDifficulty:Concussion"] = new Filter(new ScreenShaderData(concussionEffect, "Concussion"), EffectPriority.VeryHigh);
            Concussion = Filters.Scene["WellsFargosDifficulty:Concussion"];
            Concussion.Load();
        }

        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            ConcussionProgress += 0.005f;
            if (ConcussionProgress == 1)
            {
                if (Concussion.Active)
                {
                    Filters.Scene.Deactivate("WellsFargosDifficulty:Concussion");
                    ConcussionEffect.Parameters["active"].SetValue(false);
                }
                return;
            }

            if (!Concussion.Active)
            {
                Filters.Scene.Activate("WellsFargosDifficulty:Concussion");
            }

            ConcussionEffect.Parameters["active"].SetValue(true);
            ConcussionEffect.Parameters["concussiveShade"].SetValue(new Vector4(0));
            ConcussionEffect.Parameters["concussiveProgress"].SetValue(1 - (float)Math.Pow(ConcussionProgress, 2));
            ConcussionEffect.Parameters["dazeProgress"].SetValue((1 - ConcussionProgress) / 2);
            ConcussionEffect.Parameters["dazeOffset"].SetValue(new Vector2((float)Math.Sin(ConcussionProgress * 20), (float)Math.Cos(ConcussionProgress * 20)) * (1 - ConcussionProgress) / 20);
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (info.Damage >= Player.statLifeMax2 / 5)
            {
                ConcussionProgress = 0;
            }
        }
    }
}
