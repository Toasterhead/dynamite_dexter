using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public static void ChangeRoom(int previousLocationX = 0, int previousLocationY = 0)
        {
            spriteSet.Remove(player);
            spriteSet.Remove(player.TheOutlineMask);

            SkipRoom(previousLocationX, previousLocationY);

            if (WorldCursorInRange())
            {
                try
                {
                    spriteSet = worldSet[worldCursor.X, worldCursor.Y].SpriteSet;

                    if (alteredStates[worldCursor.X, worldCursor.Y])
                    {
                        List<Terrain> toRemove = new List<Terrain>();
                        foreach (IGameObject i in spriteSet)
                            if (i is Terrain)
                                toRemove.Add(i as Terrain);

                        foreach (Terrain terrain in toRemove)
                            spriteSet.Remove(terrain);

                        foreach (Terrain terrain in alteredSet[worldCursor.X, worldCursor.Y])
                            spriteSet.Add(terrain);
                    }
                }
                catch (System.NullReferenceException) { spriteSet = new List<IGameObject>(); }
            }
            else try { spriteSet = ForbiddenWorld.GenerateSpriteSet(
                player,
                worldCursor); }
                catch (System.NullReferenceException)
                {
                    spriteSet = ForbiddenWorld.GenerateSpriteSet(
                        player,
                        worldCursor);
                }
            spriteSet.Add(player);
            spriteSet.Add(player.TheOutlineMask);

            foreach (IGameObject i in spriteSet)

                if (i is IHostile)
                {
                    IHostile iHostile = i as IHostile;
                    i.Reposition(iHostile.StartX, iHostile.StartY);
                    iHostile.Reset();
                }
                else if (i is GravityArc)
                {
                    removalSet.Add(i);
                    removalSet.Add((i as GravityArc).Subject);
                    if (i is IOutlined)
                        removalSet.Add((i as IOutlined).TheOutlineMask);
                }
                else if (i is IProjectile)
                {
                    removalSet.Add(i);
                    if (i is IOutlined)
                        removalSet.Add((i as IOutlined).TheOutlineMask);
                }
                else if (i is DynamiteIgnited || i is Explosion)
                    removalSet.Add(i);
                else if (i is IResets)
                    (i as IResets).Reset();


            Room currentRoom;

            if (WorldCursorInRange())
                currentRoom = worldSet[worldCursor.X, worldCursor.Y];
            else currentRoom = new Room(spriteSet, "", Room.Mode.Standard);

            if (currentRoom != null && currentRoom.TheMode == Room.Mode.House)
                PlayMusic(Sounds.Music.HOUSE);
            else if (currentRoom != null && currentRoom.TheMode == Room.Mode.Ominous)
                PlayMusic(Sounds.Music.INSCRIPTION);
            else if (currentRoom != null && currentRoom.TheMode == Room.Mode.Boss)
            {
                if (BossRoom.TheBoss != null &&
                    BossRoom.TheBoss is IGameObject &&
                    spriteSet.Contains(BossRoom.TheBoss as IGameObject))
                {
                    spriteSet.Remove(BossRoom.TheBoss as IGameObject);

                    if (BossRoom.TheBoss is IHasAttachments)
                        foreach (IGameObject i in (BossRoom.TheBoss as IHasAttachments).Attachments)
                        {
                            spriteSet.Remove(i);
                            if (i is IOutlined)
                                spriteSet.Remove((i as IOutlined).TheOutlineMask);
                        }

                    if (BossRoom.Blockade.Count > 0)
                        foreach (Terrain i in BossRoom.Blockade)
                            spriteSet.Remove(i);
                }

                if (BossRoom.Defeated(worldCursor))
                    StopMusic();

                BossRoom.Reset();
            }
            else StopMusic();
        }

        private static void SkipRoom(int previousLocationX, int previousLocationY)
        {
            if (!WorldCursorInRange())

                return;

            Room currentRoom = worldSet[worldCursor.X, worldCursor.Y];

            while (currentRoom == null || currentRoom.TheMode == Room.Mode.Skip)
            {
                if (previousLocationX == worldCursor.X - 1 && worldCursor.X + 1 < WORLD_SIZE_X) //Traveling rightward.
                {
                    worldCursor.X++;
                    previousLocationX++;
                }
                else if (previousLocationY == worldCursor.Y - 1 && worldCursor.Y + 1 < WORLD_SIZE_Y) //Traveling downward.
                {
                    worldCursor.Y++;
                    previousLocationY++;
                }
                else if (previousLocationX == worldCursor.X + 1 && worldCursor.X - 1 >= 0) //Traveling leftward.
                {
                    worldCursor.X--;
                    previousLocationX--;
                }
                else if (previousLocationY == worldCursor.Y + 1 && worldCursor.Y - 1 >= 0) //Traveling upward.
                {
                    worldCursor.Y--;
                    previousLocationY--;
                }
                else
                {
                    currentRoom = worldSet[worldCursor.X, worldCursor.Y];
                    break;
                }

                currentRoom = worldSet[worldCursor.X, worldCursor.Y];
            }
        }
    }
}
