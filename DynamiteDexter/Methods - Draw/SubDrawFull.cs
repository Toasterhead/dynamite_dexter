using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        private void SubDrawFull()
        {
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
        }
    }
}
