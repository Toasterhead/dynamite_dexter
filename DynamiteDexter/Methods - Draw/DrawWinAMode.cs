using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawWinAMode()
        {
            const int NUM_FADE_FRAMES = 8;
            const int FADE_INTERVAL = 6;
            const int FADE_DURATION = NUM_FADE_FRAMES * FADE_INTERVAL;

            int elapsedTime = universalTimer - universalTimeStamp;
            bool displayText =
                (EndGameA.Marching && 
                elapsedTime % EndGameA.CHANGE_TEXT_INTERVAL < EndGameA.DISPLAY_TEXT_DURATION && 
                elapsedTime % EndGameA.CHANGE_TEXT_INTERVAL > 5) 
                ||
                (EndGameA.CreditsComplete && !EndGameA.Leaving);

            EndGameA.Text.Draw();

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
            spriteBatch.Draw(Images.VALLEY_CLOUDS, new Vector2(0, EndGameA.VERTICAL_POSITION_CLOUDS), Color.White);
            spriteBatch.Draw(Images.VALLEY_MOUNTAINS_1, new Vector2(EndGameA.MountainOneScroll % fullfield.X, EndGameA.VERTICAL_POSITION_MOUNTAINS_1), Color.White);
            spriteBatch.Draw(Images.VALLEY_MOUNTAINS_1, new Vector2(EndGameA.MountainOneScroll % fullfield.X - fullfield.X, EndGameA.VERTICAL_POSITION_MOUNTAINS_1), Color.White);
            spriteBatch.Draw(Images.VALLEY_MOUNTAINS_2, new Vector2(EndGameA.MountainTwoScroll % fullfield.X, EndGameA.VERTICAL_POSITION_MOUNTAINS_2), Color.White);
            spriteBatch.Draw(Images.VALLEY_MOUNTAINS_2, new Vector2(EndGameA.MountainTwoScroll % fullfield.X - fullfield.X, EndGameA.VERTICAL_POSITION_MOUNTAINS_2), Color.White);
            spriteBatch.Draw(Images.VALLEY_GRASS, new Vector2(EndGameA.GrassScroll % fullfield.X, fullfield.Y - Images.VALLEY_GRASS.Height), Color.White);
            spriteBatch.Draw(Images.VALLEY_GRASS, new Vector2(EndGameA.GrassScroll % fullfield.X - fullfield.X, fullfield.Y - Images.VALLEY_GRASS.Height), Color.White);
            if (displayText)
                spriteBatch.Draw(EndGameA.Text.Image, new Vector2(0, EndGameA.VERTICAL_POSITION_TEXT), Color.White);
            foreach (SpriteSheet i in EndGameA.Animals)
            {
                IGameObjectAnimated iAnimated = i as IGameObjectAnimated;
                spriteBatch.Draw(
                    i.Image,
                    destinationRectangle: i.Rect,
                    sourceRectangle: iAnimated.SourceRect,
                    effects: iAnimated.Orientation,
                    layerDepth: iAnimated.Layer / (float)Layers.EnumSize);
            }
            if (EndGameA.Leaving && elapsedTime > EndGameA.OUTRO_DURATION - FADE_DURATION)
            {
                int sourceRectX = ((FADE_DURATION - (EndGameA.OUTRO_DURATION - elapsedTime)) * fullfield.X) / FADE_INTERVAL;
                sourceRectX -= sourceRectX % fullfield.X;
                spriteBatch.Draw(
                            Images.FADE_SCREEN,
                            new Vector2(0, 0),
                            new Rectangle(
                                sourceRectX,
                                0,
                                fullfield.X,
                                fullfield.Y),
                            Color.White);
            }
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
