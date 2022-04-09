using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public static class BossRoom
    {
        public enum BossTypes { Defeated = 0, Bull, Snake, Vampire, Cyclops, SuperComputer, Dragon }

        private const uint TRIGGER_DURATION = 90;
        private const uint WIN_GAME_REQUIREMENT_A = 2;
        private const uint WIN_GAME_REQUIREMENT_B = 5;
        private const int SYMPATHY_STICK_X = 3;
        private const int SYMPATHY_STICK_Y = 3;

        private static readonly Rectangle tripZone = new Rectangle(
            (int)(1.5 * Game1.TILE_SIZE),
            (int)(1.5 * Game1.TILE_SIZE), 
            Game1.playfield.X - (3 * Game1.TILE_SIZE), 
            Game1.playfield.Y - (3 * Game1.TILE_SIZE));

        private static bool sympathyStick = false;
        private static uint numDefeated = 0;
        private static uint? trigger = null;
        private static IBoss boss = null;
        private static List<Terrain> blockade = new List<Terrain>();
        private static BossTypes[,] location = new BossTypes[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];

        public static bool AllDefeated { get { return numDefeated > WIN_GAME_REQUIREMENT_B; } }
        public static bool InPreBattle { get { return trigger == null || trigger > 0; } }
        public static bool BossAppears { get { return trigger == TRIGGER_DURATION; } }
        public static bool BattleBegins { get { return trigger == 1; } }
        public static IBoss TheBoss { get { return boss; } }
        public static List<Terrain> Blockade { get { return blockade; } }
        
        public static bool Defeated(Point worldCursor)
        {
            return location[worldCursor.X, worldCursor.Y] == BossTypes.Defeated;
        }

        public static bool InBattle(Point worldCursor)
        {
            return !Defeated(worldCursor) && trigger == 0;
        }

        public static bool CheckDynamiteSupply(Player player)
        {
            if (!sympathyStick && player.DynamiteSticks <= 0)

                return true;

            else if (player.DynamiteSticks > 0)
                sympathyStick = false;

            return false;
        }

        public static List<IGameObject> GetSympathyStick(Point worldCursor)
        {
            sympathyStick = true;

            int horizontalDropOff;
            int verticalPosition;

            switch (location[worldCursor.X, worldCursor.Y])
            {
                default:
                    horizontalDropOff = 6 * Game1.TILE_SIZE;
                    verticalPosition = 2 * Game1.TILE_SIZE;
                    break;
            }

            Angel angel = new Angel(horizontalDropOff, verticalPosition);

            List<IGameObject> angelList = new List<IGameObject>();
            angelList.Add(angel);
            angelList.AddRange(angel.Attachments);

            return angelList;
        }

        public static void Reset()
        {
            trigger = null;
            boss = null;
            sympathyStick = false;
            blockade.Clear();
        }

        public static void IncreaseNumDefeated() { numDefeated++; }

        public static void ResetNumDefeated() { numDefeated = 0; }

        public static void MarkLocation(BossTypes[,] boss)
        {
            for (int i = 0; i < Game1.WORLD_SIZE_X; i++)
                for (int j = 0; j < Game1.WORLD_SIZE_Y; j++)
                    MarkLocation(i, j, boss[i, j]);
        }

        public static void MarkLocation(int x, int y, BossTypes boss) { location[x, y] = boss; }

        public static void MarkAsDefeated(int x, int y) { location[x, y] = BossTypes.Defeated; }

        public static void LoadBoss(Point worldCursor, Player player)
        {
            switch (location[worldCursor.X, worldCursor.Y])
            {
                case BossTypes.Bull: boss = new Bull(player);
                    break;
                case BossTypes.Snake: boss = new Snake();
                    break;
                case BossTypes.Vampire: boss = new Vampire();
                    break;
                case BossTypes.Cyclops: boss = new Cyclops(player);
                    break;
                case BossTypes.SuperComputer: boss = new SuperComputer();
                    break;
                case BossTypes.Dragon: boss = new Dragon();
                    break;
                case BossTypes.Defeated:
                default: throw new Exception("Error - Invalid boss type.");
            }
        }

        public static void LoadBlockade(List<IGameObject> spriteSet)
        {
            Rectangle marker = new Rectangle(0, 0, 3, 3);

            for (int i = 1; i < Game1.GRID_SIZE_X - 1; i++)
            {
                SubLoadBlockade(spriteSet, marker, i, 0);
                SubLoadBlockade(spriteSet, marker, i, Game1.GRID_SIZE_Y - 1);
                SubLoadBlockade(spriteSet, marker, 0, i);
                SubLoadBlockade(spriteSet, marker, Game1.GRID_SIZE_X - 1, i);
            }
        }

        public static void SubLoadBlockade(List<IGameObject> spriteSet, Rectangle marker, int gridX, int gridY)
        {
            marker.X = gridX * Game1.TILE_SIZE;
            marker.Y = gridY * Game1.TILE_SIZE;

            bool isEmpty = true;

            foreach (IGameObject i in spriteSet)

                if (i is Terrain && marker.Intersects(i.Rect))
                {
                    isEmpty = false;
                    break;
                }

            if (isEmpty)
                blockade.Add(new Terrain(Images.BLOCKADE, gridX, gridY));
        }

        public static void ProcessPreBattle(Point worldCursor, Player player)
        {
            if (trigger == null)
                CheckTripZone(worldCursor, player);
            else if (trigger > 1)
            {
                if (trigger % 2 == 0)
                    boss.AppearingForm.Render = true;
                else boss.AppearingForm.Render = false;
            }
            else if (trigger == 1)
            {
                boss.AppearingForm.Render = false;
                Game1.PlayMusic(RetreiveMusic(worldCursor, player));
                MediaPlayer.IsRepeating = true;
            }

            if (trigger > 0)
                trigger--;
        }

        private static Song RetreiveMusic(Point worldCursor, Player player)
        {
            switch (location[worldCursor.X, worldCursor.Y])
            {
                case BossTypes.Bull:
                case BossTypes.SuperComputer:
                    return Sounds.Music.BOSS_THEME_A;
                case BossTypes.Snake:
                case BossTypes.Vampire:
                case BossTypes.Cyclops:
                    return Sounds.Music.BOSS_THEME_B;
                case BossTypes.Dragon:
                    return Sounds.Music.FINAL_BOSS;
                case BossTypes.Defeated:
                default: throw new Exception("Error - Invalid boss type.");
            }
        }

        public static void CheckTripZone(Point worldCursor, Player player)
        {
            if (location[worldCursor.X, worldCursor.Y] == BossTypes.Dragon && numDefeated < WIN_GAME_REQUIREMENT_A)

                return;

            if (location[worldCursor.X, worldCursor.Y] != BossTypes.Defeated &&
                player.Center.X >= tripZone.X &&
                player.Center.X < tripZone.X + tripZone.Width &&
                player.Center.Y >= tripZone.Y &&
                player.Center.Y < tripZone.Y + tripZone.Height)
            {
                LoadBoss(worldCursor, player);
                trigger = TRIGGER_DURATION + 1;
                Game1.PlayMusic(Sounds.Music.BOSS_APPEARS);
            }
        }

        public static void PlayDefeatAudio()
        {
            MediaPlayer.IsRepeating = false;
            Game1.StopMusic();
            Game1.PlaySound(Sounds.SLAY_BOSS);
        }
    }
}
