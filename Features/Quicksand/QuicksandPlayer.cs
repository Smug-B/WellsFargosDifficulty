using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace WellsFargosDifficulty.Features.Quicksand
{
    public class QuicksandPlayer : ModPlayer
    {
        /// <summary>
        /// Determines many layers of sand are required for the surface to be 'quicksand'.
        /// </summary>
        public const int QuickSandThreshold = 10;

        /// <summary>
        /// Whether or not the player is standing on 'quicksand'.
        /// </summary>
        public bool Quicksand { get; private set; }

        public int LastMoved { get; private set; }

        public int LastMovedThreshold { get; private set; } = 180;

        public override void ResetEffects()
        {
            Quicksand = false;
            LastMoved = Math.Clamp(LastMoved + 1, 0, LastMovedThreshold * 2);
        }

        public override void PostUpdateMiscEffects()
        {
            if (LastMoved < LastMovedThreshold)
            {
                return;
            }

            Point tilePositon = Player.Center.ToTileCoordinates();
            int startX = tilePositon.X - 1;
            int endX = tilePositon.X + 1 + (Player.Center.X % 16 < 8 ? -1 : 0);
            int j = tilePositon.Y + 2;

            for (int i = startX; i <= endX; i++)
            {
                if (!WorldGen.InWorld(i, j))
                {
                    continue;
                }

                Tile tile = Framing.GetTileSafely(i, j);
                if (tile.HasTile && !Main.tileSand[tile.TileType])
                {
                    LastMoved = 0;
                    return;
                }

                for (int searchY = 1; searchY < QuickSandThreshold; searchY++)
                {
                    if (!WorldGen.InWorld(i, j + searchY))
                    {
                        continue;
                    }

                    Tile searchTile = Framing.GetTileSafely(i, j + searchY);
                    if (!searchTile.HasTile || !Main.tileSand[searchTile.TileType])
                    {
                        LastMoved = 0;
                        return;
                    }
                }
            }

            Quicksand = true;
            Player.position.Y++;
            Player.controlDown = true;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Player.velocity.X != 0 || Player.velocity.Y < 0)
            {
                LastMoved -= LastMovedThreshold / 10;
                return;
            }

            if (Quicksand)
            {
                Player.maxRunSpeed /= 10;
                Player.runAcceleration /= 25;
            }
        }
    }
}
