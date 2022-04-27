using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public static class ForbiddenWorld
    {
        private const int MARGIN = 3 * Game1.TILE_SIZE;
        private const int INITIAL_TREASURE_SUPPLY = 50;
        private const int DIFFICULTY_SPIKE_THRESHOLD = 20;

        private static List<Point> entryPoints = new List<Point>();
        private static int? totalTreasureSupply = INITIAL_TREASURE_SUPPLY;
        private static int traversals = 0;

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

            if (AtValidEntryPoint(worldCursor.X, worldCursor.Y))

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

                    if (AtValidEntryPoint(worldCursor.X, worldCursor.Y))
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

            int difficultySpike = traversals >= DIFFICULTY_SPIKE_THRESHOLD ? (traversals - DIFFICULTY_SPIKE_THRESHOLD) / 4 : 0;

            int enemyCap = 3 * (level + 1) + difficultySpike;
            int dynamiteCap = level;
            int treasureCap = level + 2;

            int enemyCount = 0;
            int dynamiteCount = 0;
            int treasureCount = 0;

            int probability = 40 / (level + 1 + (difficultySpike / 4));

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
                            if (totalTreasureSupply != null)
                            { 
                                if (Game1.rand.Next(5) == 0)
                                    spriteSet.Add(new MoneyBag(x, y));
                                else spriteSet.Add(new Coins(x, y));

                                if (--totalTreasureSupply <= 0) totalTreasureSupply = null;
                            }
                            else spriteSet.Add(GetRandomEnemy(x, y, player));
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

            traversals++;

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

        private static bool AtValidEntryPoint(int worldCursorX, int worldCursorY)
        {
            bool atEdge =
                (worldCursorX == -1 && worldCursorY >= 0 && worldCursorY < Game1.WORLD_SIZE_Y) ||
                (worldCursorX == Game1.WORLD_SIZE_X && worldCursorY >= 0 && worldCursorY < Game1.WORLD_SIZE_Y) ||
                (worldCursorY == -1 && worldCursorX >= 0 && worldCursorX < Game1.WORLD_SIZE_X) ||
                (worldCursorY == Game1.WORLD_SIZE_Y && worldCursorX >= 0 && worldCursorX < Game1.WORLD_SIZE_X);

            if (atEdge)
            { 
                foreach (Point entryPoint in entryPoints)
            
                    if (worldCursorX == entryPoint.X && worldCursorY == entryPoint.Y)

                        return true;
            }

            return false;
        }

        public static void LoadEntryPoints(List<string> entryPointsFileData)
        {
            char[] delimiters = { ':' };
            entryPoints = new List<Point>();

            foreach (string line in entryPointsFileData)
            {
                string[] terms = line.Split(delimiters);
                entryPoints.Add(new Point(Convert.ToInt32(terms[0]), Convert.ToInt32(terms[1])));
            }
        }

        public static void ResetCounts()
        {
            traversals = 0;
            totalTreasureSupply = INITIAL_TREASURE_SUPPLY;
        }
    }
}
