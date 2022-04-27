using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawIllustrationMode()
        {
            GraphicsDevice.SetRenderTarget(canvasFull);

            const int NUM_FADE_FRAMES = 8;
            const int FADE_INTERVAL = 2;

            int elapsedTime = universalTimer - universalTimeStamp;

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
            spriteBatch.Draw(illustration != null ? illustration : Images.BLANK, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            if (FADE_INTERVAL * NUM_FADE_FRAMES - elapsedTime > 0)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(
                        Images.FADE_SCREEN,
                        new Vector2(0, 0),
                        new Rectangle(
                            ((FADE_INTERVAL * NUM_FADE_FRAMES - elapsedTime) * fullfield.X) / FADE_INTERVAL,
                            0,
                            fullfield.X,
                            fullfield.Y),
                        Color.White);
                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            ProcessEffect();
            if (graphics.IsFullScreen)
                spriteBatch.Draw(
                    canvasFull,
                    new Rectangle(
                        (int)(0.5 * (graphics.PreferredBackBufferWidth - (canvasMultiplier * fullfield.X))),
                        (int)(0.5 * (graphics.PreferredBackBufferHeight - (canvasMultiplier * fullfield.Y))),
                        canvasMultiplier * fullfield.X,
                        canvasMultiplier * fullfield.Y),
                    Color.White);
            else spriteBatch.Draw(
                canvasFull,
                    new Rectangle(
                        0,
                        0,
                        graphics.PreferredBackBufferWidth,
                        graphics.PreferredBackBufferHeight),
                    Color.White);
            spriteBatch.End();
        }
    }
}