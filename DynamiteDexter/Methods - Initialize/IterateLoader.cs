﻿using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public static void IterateLoader()
        {
            if (mapLoadIndex.X >= 0 && mapLoadIndex.Y >= 0)
            {
                worldSet[mapLoadIndex.X, mapLoadIndex.Y] = Map.LoadMap(
                    FileInOut.mapFileData[mapLoadIndex.X, mapLoadIndex.Y]);
                alteredSet[mapLoadIndex.X, mapLoadIndex.Y] = Map.LoadAltered(
                    FileInOut.mapFileData[mapLoadIndex.X, mapLoadIndex.Y]);

                if (++mapLoadIndex.X == WORLD_SIZE_X)
                {
                    mapLoadIndex.X = 0;

                    if (++mapLoadIndex.Y == WORLD_SIZE_Y)
                    {
                        mapLoadIndex = new Point(-1, -1);

                        playerHouse = new SpriteSheet(
                            new SpriteInfo(Images.HOUSE_PUNCTURED, 4 * TILE_SIZE, 1 * TILE_SIZE, (int)Layers.Floor),
                            new CollisionInfo(null, null),
                            new AnimationInfo(2, 1, 15, false));

                        spriteSet = worldSet[worldCursor.X, worldCursor.Y].SpriteSet;
                        spriteSet.Add(player);
                        spriteSet.Add(player.TheOutlineMask);
                        spriteSet.Add(playerHouse);

                        foreach (Room i in worldSet)
                            foreach (IGameObject j in i.SpriteSet)
                                if (j is Gate)
                                    gates.Add(j as Gate);

                        textMessage.ChangeText(Messages.DEPARTURE);
                        gameMode = GameModes.Text;
                        PlayMusic(Sounds.Music.DEPARTURE);
                    }
                }
            }
            else NewGame();
        }
    }
}
