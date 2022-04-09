using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawLoadMode()
        {
            const int LOAD_BAR_SIZE = 16;

            Textfield textfield = MenuManager.TheTextfields[0];
            int marginLeft = (fullfield.X / 2) - ((LOAD_BAR_SIZE / 2) * SUB_TILE_SIZE) - SUB_TILE_SIZE;
            int marginTop = (fullfield.Y / 2) - SUB_TILE_SIZE;
            int marginTextLeft = (fullfield.X / 2) - (5 * SUB_TILE_SIZE);
            int marginTextTop = marginTop - (2 * SUB_TILE_SIZE);
            int fillSize = mapLoadIndex.X >= 0 && mapLoadIndex.Y >= 0 ?
                (int)(((float)mapLoadIndex.Y / WORLD_SIZE_Y) * LOAD_BAR_SIZE) + 1 : 0;

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
            spriteBatch.Draw(textfield.Image, new Vector2(marginTextLeft, marginTextTop), Color.White);
            spriteBatch.Draw(Images.LOAD_BAR_LEFT, new Vector2(marginLeft, marginTop), Color.White);
            for (int i = 0; i < LOAD_BAR_SIZE; i++)
                spriteBatch.Draw(Images.LOAD_BAR_MIDDLE, new Vector2(i * SUB_TILE_SIZE + SUB_TILE_SIZE + marginLeft, marginTop), Color.White);
            for (int i = 0; i < fillSize; i++)
                spriteBatch.Draw(Images.LOAD_BAR_FILL, new Vector2(i * SUB_TILE_SIZE + SUB_TILE_SIZE + marginLeft, marginTop), Color.White);
            spriteBatch.Draw(Images.LOAD_BAR_RIGHT, new Vector2(LOAD_BAR_SIZE * SUB_TILE_SIZE + SUB_TILE_SIZE + marginLeft, marginTop), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
