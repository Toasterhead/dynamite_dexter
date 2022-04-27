using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawTextMode()
        {
            bool inHouse = WorldCursorInRange() && worldSet[worldCursor.X, worldCursor.Y].TheMode == Room.Mode.House;
            bool isOminous = WorldCursorInRange() && worldSet[worldCursor.X, worldCursor.Y].TheMode == Room.Mode.Ominous;

            if (isOminous)
                textPlaque.Draw();
            else textMessage.Draw();

            GraphicsDevice.SetRenderTarget(canvasFull);

            if (isOminous)
            {
                const int NUM_FADE_FRAMES = 8;
                const int FADE_INTERVAL = 4;

                int elapsedTime = universalTimer - universalTimeStamp;

                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
                spriteBatch.Draw(Images.BACKGROUND_PLAQUE, new Vector2(0, 0), Color.White);
                INVERT.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(textPlaque.Image, new Vector2(4 * SUB_TILE_SIZE, 11 * SUB_TILE_SIZE), Color.White);
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
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Images.BORDER_TOP_LEFT, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(Images.BORDER_TOP_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, 0), Color.White);
                spriteBatch.Draw(Images.BORDER_BOTTOM_LEFT, new Vector2(0, fullfield.Y - SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.BORDER_BOTTOM_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, fullfield.Y - SUB_TILE_SIZE), Color.White);
                for (int i = 1; i < (fullfield.X / SUB_TILE_SIZE) - 1; i++)
                {
                    spriteBatch.Draw(Images.BORDER_TOP, new Vector2(i * SUB_TILE_SIZE, 0), Color.White);
                    spriteBatch.Draw(Images.BORDER_BOTTOM, new Vector2(i * SUB_TILE_SIZE, fullfield.Y - SUB_TILE_SIZE), Color.White);
                }
                for (int i = 1; i < (fullfield.Y / SUB_TILE_SIZE) - 1; i++)
                {
                    spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(0, i * SUB_TILE_SIZE), Color.White);
                    spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, i * SUB_TILE_SIZE), Color.White);
                }
                if (inHouse)
                {
                    spriteBatch.Draw(
                        Images.BACKGROUND_HOUSE,
                        new Vector2(
                            (fullfield.X / 2) - (Images.BACKGROUND_HOUSE.Width / 2),
                            SUB_TILE_SIZE),
                        Color.White);
                    spriteBatch.Draw(
                        HouseRoom.DetermineDenizenImage(worldCursor),
                        new Vector2(
                            (fullfield.X / 2) - TILE_SIZE - 4,
                            SUB_TILE_SIZE),
                        Color.White);
                }
                spriteBatch.Draw(
                    textMessage.Image, 
                    new Vector2(
                        2 * SUB_TILE_SIZE, 
                        inHouse ? 8 * SUB_TILE_SIZE : 2 * SUB_TILE_SIZE), 
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
