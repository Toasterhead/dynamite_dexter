using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawGameOverMode()
        {
            string text = "GAME OVER";

            MenuManager.TheTextfields[0].ChangeText(text);
            MenuManager.TheTextfields[0].Draw();

            int textMarginLeft = (fullfield.X / 2) - ((SUB_TILE_SIZE * text.Length) / 2);
            int textMarginTop = (fullfield.Y / 2) - (SUB_TILE_SIZE / 2);
            int angelLeft = (3 * (fullfield.X / 4)) - (TILE_SIZE / 2);
            int timeElapsed = universalTimer - universalTimeStamp;

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin();
            spriteBatch.Draw(
                MenuManager.TheTextfields[0].Image, 
                new Vector2(
                    textMarginLeft, 
                    textMarginTop), 
                Color.White);
            spriteBatch.Draw(
                Images.PLAYER_ANGEL, 
                new Vector2(
                    angelLeft, 
                    (2 * TILE_SIZE + fullfield.Y) - timeElapsed),
                sourceRectangle: new Rectangle(
                    timeElapsed % 10 < 5 ? 0 : 2 * TILE_SIZE, 
                    0, 
                    2 * TILE_SIZE, 
                    2 * TILE_SIZE),
                color: Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
