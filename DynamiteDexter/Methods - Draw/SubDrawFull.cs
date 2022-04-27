using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        private void SubDrawFull()
        {
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
        }

        private void ProcessEffect()
        {
            switch (renderEffect)
            {
                case RenderEffects.ScanlinesGreen:
                    SCANLINE.Parameters["surfaceHeight"].SetValue(480.0f);
                    SCANLINE.Parameters["brightness"].SetValue(0.25f);
                    SCANLINE.Parameters["tintR"].SetValue(0.5f);
                    SCANLINE.Parameters["tintG"].SetValue(1.0f);
                    SCANLINE.Parameters["tintB"].SetValue(0.1f);
                    SCANLINE.CurrentTechnique.Passes[0].Apply();
                    break;
                case RenderEffects.ScanlinesBlue:
                    SCANLINE.Parameters["surfaceHeight"].SetValue(480.0f);
                    SCANLINE.Parameters["brightness"].SetValue(0.1f);
                    SCANLINE.Parameters["tintR"].SetValue(0.7f);
                    SCANLINE.Parameters["tintG"].SetValue(0.9f);
                    SCANLINE.Parameters["tintB"].SetValue(1.0f);
                    SCANLINE.CurrentTechnique.Passes[0].Apply();
                    break;
                case RenderEffects.Terminal:
                    BACK_COLOR.Parameters["addR"].SetValue(0.0f);
                    BACK_COLOR.Parameters["addG"].SetValue(0.0f);
                    BACK_COLOR.Parameters["addB"].SetValue(0.5f);
                    BACK_COLOR.CurrentTechnique.Passes[0].Apply();
                    break;
                case RenderEffects.PhaseShift:
                    PHASE_SHIFT.CurrentTechnique.Passes[0].Apply();
                    break;
            }
        }
    }
}
