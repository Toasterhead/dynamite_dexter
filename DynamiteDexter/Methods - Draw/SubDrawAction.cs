using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        private void SubDrawAction()
        {
            if (graphics.IsFullScreen)
                spriteBatch.Draw(
                    canvasAction,
                    new Rectangle(
                        (int)(0.5 * (graphics.PreferredBackBufferWidth - (canvasMultiplier * fullfield.X))),
                        (int)(0.5 * (graphics.PreferredBackBufferHeight - (canvasMultiplier * fullfield.Y))),
                        canvasMultiplier * playfield.X,
                        canvasMultiplier * playfield.Y),
                    Color.White);
            else spriteBatch.Draw(
                canvasAction,
                new Rectangle(
                    0,
                    0,
                    (int)(ACTION_CANVAS_RATIO * graphics.PreferredBackBufferWidth),
                    graphics.PreferredBackBufferHeight),
                Color.White);
        }
    }
}
