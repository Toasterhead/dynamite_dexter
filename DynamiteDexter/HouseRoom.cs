using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public static class HouseRoom
    {
        public enum DenizenTypes { None = 0, Miner, Woman, Boy, Girl, Fish, Penguin, Robot, Clown, Baron, Enthusiast }

        private static DenizenTypes[,] denizenLocation = new DenizenTypes[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];

        public static void MarkLocation(DenizenTypes[,] denizen)
        {
            for (int i = 0; i < Game1.WORLD_SIZE_X; i++)
                for (int j = 0; j < Game1.WORLD_SIZE_Y; j++)
                    MarkLocation(i, j, denizen[i, j]);
        }

        public static void MarkLocation(int x, int y, DenizenTypes denizen) { denizenLocation[x, y] = denizen; }

        public static Texture2D DetermineDenizenImage(Point worldCursor)
        {
            DenizenTypes denizen = denizenLocation[worldCursor.X, worldCursor.Y];

            switch (denizen)
            {
                case DenizenTypes.None:
                case DenizenTypes.Miner:
                    return Images.DENIZEN_MINER;
                case DenizenTypes.Woman:
                    return Images.DENIZEN_WOMAN;
                case DenizenTypes.Boy:
                    return Images.DENIZEN_BOY;
                case DenizenTypes.Girl:
                    return Images.DENIZEN_GIRL;
                case DenizenTypes.Fish:
                    return Images.DENIZEN_FISH;
                case DenizenTypes.Penguin:
                    return Images.DENIZEN_PENGUIN;
                case DenizenTypes.Robot:
                    return Images.DENIZEN_ROBOT;
                case DenizenTypes.Clown:
                    return Images.DENIZEN_CLOWN;
                case DenizenTypes.Baron:
                    return Images.DENIZEN_BARON;
                case DenizenTypes.Enthusiast:
                    return Images.DENIZEN_ENTHUSIAST;
                default: throw new Exception("Error - Unable to recognize denizen type.");
            }
        }
    }
}
