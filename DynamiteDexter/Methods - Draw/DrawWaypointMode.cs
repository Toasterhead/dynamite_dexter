using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawWaypointMode()
        {
            int mapLeft = (fullfield.X / 2) - (((WORLD_SIZE_X / 2) * SUB_TILE_SIZE) / 2);
            int mapTop = fullfield.Y - ((WORLD_SIZE_Y / 2) * SUB_TILE_SIZE) - SUB_TILE_SIZE;

            for (int i = 0; i < MenuManager.WaypointMenu.CurrentSubMenuLength; i++)
            {
                MenuManager.TheTextfields[i].ChangeText(MenuManager.WaypointMenu.GetCurrentAtIndex(i).ToString());
                MenuManager.TheTextfields[i].Draw();
            }

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin();
            for (int i = 0; i < MenuManager.WaypointMenu.CurrentSubMenuLength; i++)
                spriteBatch.Draw(
                    MenuManager.TheTextfields[i].Image,
                    new Vector2((3 * SUB_TILE_SIZE), (i * SUB_TILE_SIZE) + SUB_TILE_SIZE),
                    Color.White);
            spriteBatch.Draw(
                Images.ICON_CURSOR,
                new Vector2(SUB_TILE_SIZE, (MenuManager.WaypointMenu.CurrentSubMenu.SelectionIndex * SUB_TILE_SIZE) + SUB_TILE_SIZE),
                Color.White);
            for (int i = 0; i < WORLD_SIZE_X / 2; i++)
                for (int j = 0; j < WORLD_SIZE_Y / 2; j++)
                {
                    Texture2D image;

                    if (i == BEGIN_ROOM_X / 2 && j == BEGIN_ROOM_Y / 2)
                        image = Images.Characters.H;
                    else image = Images.Characters.PERIOD;

                    for (int k = 0; k < flagStatus.Length; k++)
                    {
                        if (flagStatus[k].raised && i == flagStatus[k].mapLocation.X / 2 && j == flagStatus[k].mapLocation.Y / 2)
                        {
                            switch (k)
                            {
                                case 0: image = Images.Characters.NUM_1;
                                    break;
                                case 1: image = Images.Characters.NUM_2;
                                    break;
                                case 2: image = Images.Characters.NUM_3;
                                    break;
                                case 3: image = Images.Characters.NUM_4;
                                    break;
                                case 4: image = Images.Characters.NUM_5;
                                    break;
                                case 5: image = Images.Characters.NUM_6;
                                    break;
                                case 6: image = Images.Characters.NUM_7;
                                    break;
                                case 7: image = Images.Characters.NUM_8;
                                    break;
                                case 8: image = Images.Characters.NUM_9;
                                    break;
                                default: image = Images.Characters.NUM_0;
                                    break;
                            }

                            break;
                        }
                    }

                    spriteBatch.Draw(image, new Vector2(mapLeft + (i * SUB_TILE_SIZE), mapTop + (j * SUB_TILE_SIZE)), Color.White);
                }
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
