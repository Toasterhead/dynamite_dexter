using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public static class ForbiddenWorld
    {
        private const int MARGIN = 3 * Game1.TILE_SIZE;
        private const int ENTRY_POINT_X = 11;
        private const int ENTRY_POINT_Y = 20;

        private static readonly string leftToRight =
            "..............." +
            "..............." +
            "..............." +
            "..............." +
            "..............." +
            "###############" +
            "...         ..." +
            "...         ..." +
            "...         ..." +
            "###############" +
            "..............." +
            "..............." +
            "..............." +
            "..............." +
            "...............";

        private static readonly string topToBottom =
            ".....#...#....." +
            ".....#...#....." +
            ".....#...#....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#   #....." +
            ".....#...#....." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string topLeftL =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "###         #.." +
            "...         #.." +
            "...         #.." +
            "...         #.." +
            "###         #.." +
            "..#         #.." +
            "..#         #.." +
            "..###########.." +
            "..............." +
            "...............";

        private static readonly string topRightL =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "..#         ###" +
            "..#         ..." +
            "..#         ..." +
            "..#         ..." +
            "..#         ###" +
            "..#         #.." +
            "..#         #.." +
            "..###########.." +
            "..............." +
            "...............";

        private static readonly string bottomLeftL =
            "..............." +
            "..............." +
            "..###########.." +
            "..#         #.." +
            "..#         #.." +
            "###         #.." +
            "...         #.." +
            "...         #.." +
            "...         #.." +
            "###         #.." +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string bottomRightL =
            "..............." +
            "..............." +
            "..###########.." +
            "..#         #.." +
            "..#         #.." +
            "..#         ###" +
            "..#         ..." +
            "..#         ..." +
            "..#         ..." +
            "..#         ###" +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string leftT =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "###         #.." +
            "...         #.." +
            "...         #.." +
            "...         #.." +
            "###         #.." +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string topT =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "###         ###" +
            "...         ..." +
            "...         ..." +
            "...         ..." +
            "###         ###" +
            "..#         #.." +
            "..#         #.." +
            "..###########.." +
            "..............." +
            "...............";

        private static readonly string rightT =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "..#         ###" +
            "..#         ..." +
            "..#         ..." +
            "..#         ..." +
            "..#         ###" +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string bottomT =
            "..............." +
            "..............." +
            "..###########.." +
            "..#         #.." +
            "..#         #.." +
            "###         ###" +
            "...         ..." +
            "...         ..." +
            "...         ..." +
            "###         ###" +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string cross =
            ".....#...#....." +
            ".....#...#....." +
            "..####...####.." +
            "..#         #.." +
            "..#         #.." +
            "###         ###" +
            "...         ..." +
            "...         ..." +
            "...         ..." +
            "###         ###" +
            "..#         #.." +
            "..#         #.." +
            "..####...####.." +
            ".....#...#....." +
            ".....#...#.....";

        private static readonly string[] leftCompatible = { leftToRight, topLeftL, bottomLeftL, leftT, topT, bottomT, cross };
        private static readonly string[] rightCompatible = { leftToRight, topRightL, bottomRightL, rightT, topT, bottomT, cross };
        private static readonly string[] topCompatible = { topToBottom, topLeftL, topRightL, topT, leftT, rightT, cross };
        private static readonly string[] bottomCompatible = { topToBottom, bottomLeftL, bottomRightL, bottomT, leftT, rightT, cross };

        public static Point previousLocation;
        public static List<IGameObject> currentSet;
        public static List<IGameObject> previousSet;

        public static List<IGameObject> GenerateSpriteSet(Player player, Point worldCursor)
        {
            int selectionIndex;
            int range = leftCompatible.Length;
            string selection = "";
            List<IGameObject> spriteSet = new List<IGameObject>();

            //Determine previous world map location and directional compatibility...

            Point previousLocationDuplicate = previousLocation;

            if (worldCursor.X == ENTRY_POINT_X && worldCursor.Y == ENTRY_POINT_Y)

                selection = cross;

            else
            { 
                bool selectionValid;

                do
                {
                    selectionIndex = Game1.rand.Next(range);

                    if (player.X < Game1.playfield.X / 2 - MARGIN)
                    {
                        previousLocation = new Point(worldCursor.X - 1, worldCursor.Y);
                        selection = leftCompatible[selectionIndex];
                    }
                    else if (player.X > Game1.playfield.X / 2 + MARGIN)
                    {
                        previousLocation = new Point(worldCursor.X + 1, worldCursor.Y);
                        selection = rightCompatible[selectionIndex];
                    }
                    else if (player.Y < Game1.playfield.Y / 2 - MARGIN)
                    {
                        previousLocation = new Point(worldCursor.X, worldCursor.Y - 1);
                        selection = topCompatible[selectionIndex];
                    }
                    else //if (player.Y > Game1.playfield.Y / 2 + MARGIN)
                    {
                        previousLocation = new Point(worldCursor.X, worldCursor.Y + 1);
                        selection = bottomCompatible[selectionIndex];
                    }

                    selectionValid = true;

                    if (worldCursor.X == -1 && worldCursor.Y >= 0 && worldCursor.Y < Game1.WORLD_SIZE_Y && rightCompatible.ToList<string>().Contains(selection))
                        selectionValid = false;
                    else if (worldCursor.X == Game1.WORLD_SIZE_X && worldCursor.Y >= 0 && worldCursor.Y < Game1.WORLD_SIZE_Y && leftCompatible.ToList<string>().Contains(selection))
                        selectionValid = false;
                    else if (worldCursor.Y == -1 && worldCursor.X >= 0 && worldCursor.X < Game1.WORLD_SIZE_X && bottomCompatible.ToList<string>().Contains(selection))
                        selectionValid = false;
                    else if (worldCursor.Y == Game1.WORLD_SIZE_Y && worldCursor.X >= 0 && worldCursor.X < Game1.WORLD_SIZE_X && topCompatible.ToList<string>().Contains(selection))
                        selectionValid = false;

                    //Bug fix.
                    if (selectionValid == false && worldCursor.X == ENTRY_POINT_X && worldCursor.Y == ENTRY_POINT_Y)
                    {
                        selectionValid = true;
                        selection = cross;
                    }

                } while (!selectionValid);
            }

            if (worldCursor.X == previousLocationDuplicate.X && worldCursor.Y == previousLocationDuplicate.Y)
            {
                List<IGameObject> previousSetCopy = new List<IGameObject>();
                foreach (IGameObject i in previousSet)
                    previousSetCopy.Add(i);
                previousSet = new List<IGameObject>();
                foreach (IGameObject i in currentSet)
                    previousSet.Add(i);
                currentSet = previousSetCopy;

                return currentSet;
            }

            previousSet = new List<IGameObject>();
            if (currentSet != null)
                foreach (IGameObject i in currentSet)
                    previousSet.Add(i);

            //Determine difficulty level and probabilities...

            int[] distances = 
            {
                worldCursor.X - Game1.WORLD_SIZE_X,
                worldCursor.X < 0 ? -worldCursor.X - 1 : 0,
                worldCursor.Y - Game1.WORLD_SIZE_Y,
                worldCursor.Y < 0 ? -worldCursor.Y - 1 : 0
            };

            int level = 0;

            foreach (int i in distances)
                if (i > level)
                    level = i;

            int enemyCap = 3 * (level + 1);
            int dynamiteCap = level;
            int treasureCap = level + 2;

            int enemyCount = 0;
            int dynamiteCount = 0;
            int treasureCount = 0;

            int probability = 40 / (level + 1);

            //Create sprite set...

            int rowIndex = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                if (i > 0 && i % Game1.GRID_SIZE_X == 0)
                    rowIndex++;

                int x = i % Game1.GRID_SIZE_X;
                int y = rowIndex;

                if (selection[i] == '#')
                    spriteSet.Add(new SkullWall(x, y, player));
                else if (selection[i] == ' ' && Game1.rand.Next(probability) == 0)
                {
                    if (Game1.rand.Next(probability / 4) == 0)
                    {
                        if (level >= 2 && Game1.rand.Next(4) == 0 && treasureCount++ < treasureCap)
                        {
                            if (Game1.rand.Next(5) == 0)
                                spriteSet.Add(new MoneyBag(x, y));
                            else spriteSet.Add(new Coins(x, y));
                        }
                        else if (dynamiteCount++ < dynamiteCap)
                        {
                            if (Game1.rand.Next(10) == 0)
                                spriteSet.Add(new DynamitePack(x, y));
                            else spriteSet.Add(new DynamiteSingleStick(x, y));
                        }
                        else if (enemyCount++ < enemyCap)
                            spriteSet.Add(GetRandomEnemy(x, y, player));
                    }
                    else if (enemyCount++ < enemyCap)
                        spriteSet.Add(GetRandomEnemy(x, y, player));
                }
            }

            currentSet = new List<IGameObject>();
            foreach (IGameObject i in spriteSet)
            { 
                currentSet.Add(i);
                if (i is INavigates)
                    (i as INavigates).AcquireSet(spriteSet);
            }

            return spriteSet;
        }

        private static IGameObject GetRandomEnemy(int x, int y, IGameObject target)
        {
            switch (Game1.rand.Next(6))
            {
                case 0: return new Beetle(x, y);
                case 1: return new Spider(x, y);
                case 2: return new Scorpion(x, y, target);
                case 3: return new Hypnoball(x, y);
                case 4: return y < Game1.GRID_SIZE_Y / 2 - 3 ? new FloatingHead(x, y) as IGameObject : new Beetle(x, y);
                case 5: return y < Game1.GRID_SIZE_Y / 2 - 3 ? new FloatingHead(x, y, true) as IGameObject : new Spider(x, y);
            }

            return new Beetle(x, y);
        }
    }
}
