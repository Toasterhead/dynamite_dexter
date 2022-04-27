using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawDeathMode()
        {
            int timeElapsed = universalTimer - universalTimeStamp;

            Rectangle sourceRect =
                 timeElapsed < SCRIPT_INTERVAL * 4 ?
                 new Rectangle(0, 0, TILE_SIZE, TILE_SIZE) :
                 new Rectangle(TILE_SIZE, 0, TILE_SIZE, TILE_SIZE);

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin();
            if (timeElapsed < SCRIPT_INTERVAL * 2)
            {
                spriteBatch.Draw(
                   causeOfDeath.Image,
                   destinationRectangle: causeOfDeath.Rect,
                   sourceRectangle: causeOfDeath is IGameObjectAnimated ? (causeOfDeath as IGameObjectAnimated).SourceRect : new Rectangle(0, 0, causeOfDeath.Width, causeOfDeath.Height),
                   effects: causeOfDeath is IGameObjectPlus ? (causeOfDeath as IGameObjectPlus).Orientation : SpriteEffects.None);

                if (causeOfDeath is IHasAttachments)
                    foreach (IGameObject i in (causeOfDeath as IHasAttachments).Attachments)
                        spriteBatch.Draw(
                            i.Render ? i.Image : Images.BLANK,
                            destinationRectangle: i.Rect,
                            sourceRectangle: i is IGameObjectAnimated ? (i as IGameObjectAnimated).SourceRect : i.Rect,
                            effects: i is IGameObjectPlus ? (i as IGameObjectPlus).Orientation : SpriteEffects.None);
            }
            spriteBatch.Draw(Images.PLAYER_DEATH, new Vector2(player.X, player.Y), sourceRect, Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
