using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawWinBMode()
        {
            const int VISIBILITY_THRESHOLD = 2;

            int elapsedTime = universalTimer - universalTimeStamp;

            EndGameB.Text.Draw();

            GraphicsDevice.SetRenderTarget(canvasFull);

            if (EndGameB.Boarding)
            {
                EndGameB.SpaceshipScripted spaceship = EndGameB.TheSpaceshipScripted;
                EndGameB.AvatarScripted avatar = EndGameB.TheAvatarScripted;

                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(Images.VALLEY_STARS, new Vector2(0, EndGameB.VERTICAL_POSITION_STARS), Color.White);
                spriteBatch.Draw(Images.VALLEY_MOUNTAINS_1, new Vector2(0, EndGameB.VERTICAL_POSITION_MOUNTAINS_1), Color.White);
                spriteBatch.Draw(Images.VALLEY_MOUNTAINS_1, new Vector2(0, EndGameB.VERTICAL_POSITION_MOUNTAINS_1), Color.White);
                spriteBatch.Draw(Images.VALLEY_MOUNTAINS_2, new Vector2(0, EndGameB.VERTICAL_POSITION_MOUNTAINS_2), Color.White);
                spriteBatch.Draw(Images.VALLEY_MOUNTAINS_2, new Vector2(0, EndGameB.VERTICAL_POSITION_MOUNTAINS_2), Color.White);
                spriteBatch.Draw(Images.VALLEY_GRASS, new Vector2(0, fullfield.Y - Images.VALLEY_GRASS.Height), Color.White);
                spriteBatch.Draw(Images.VALLEY_GRASS, new Vector2(0, fullfield.Y - Images.VALLEY_GRASS.Height), Color.White);
                if (avatar.Render)
                    spriteBatch.Draw(avatar.Image, avatar.ScaleRect, avatar.SourceRect, Color.White);
                if (spaceship.Render)
                    spriteBatch.Draw(spaceship.Image, spaceship.Rect, Color.White);
                if (spaceship.Exhaust.Render)
                    spriteBatch.Draw(spaceship.Exhaust.Image, spaceship.Exhaust.Rect, Color.White);
                if (spaceship.Bottom <= 0)
                {
                    EndGameB.TheFade.Render = true;
                    spriteBatch.Draw(EndGameB.TheFade.Image, EndGameB.TheFade.Rect, EndGameB.TheFade.SourceRect, Color.White);
                }
                spriteBatch.End();
            }
            else if (EndGameB.Cruising)
            {
                bool displayText = (
                    elapsedTime % EndGameA.CHANGE_TEXT_INTERVAL < EndGameA.DISPLAY_TEXT_DURATION &&
                    elapsedTime % EndGameA.CHANGE_TEXT_INTERVAL > 5)
                    ||
                    EndGameB.CreditsComplete;

                spriteBatch.Begin();
                foreach (EndGameB.CelestialBody i in EndGameB.CelestialBodies)
                    if (i.Render && i.Layer > EndGameB.Z_FRINGE + VISIBILITY_THRESHOLD)
                    {
                        Vector2 centerTransformed = EndGameB.PerspectiveTransform(i);

                        spriteBatch.Draw(
                            i.Image,
                            new Rectangle(
                                (int)centerTransformed.X - (i.Width / 2),
                                (int)centerTransformed.Y - (i.Height / 2),
                                i.Width,
                                i.Height),
                            i.SourceRect,
                            i.TheColor);
                    }
                if (displayText)
                    spriteBatch.Draw(EndGameB.Text.Image, new Vector2(0.0f, EndGameB.VERTICAL_POSITION_TEXT), Color.White);
                spriteBatch.End();
            }
            else if (EndGameB.Landing)
            {
                EndGameB.SpaceshipScripted spaceship = EndGameB.TheSpaceshipScripted;
                EndGameB.AvatarScripted avatar = EndGameB.TheAvatarSpacesuitScripted;
                EndGameB.Fade fade = EndGameB.TheFade;
                SpriteSheet smoke = EndGameB.SpaceshipSmoke;
                Texture2D background = EndGameB.CrashLanded ? Images.SPACE_CITY_CRASH : Images.SPACE_CITY;

                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(background, new Vector2(0.0f, EndGameB.Quake % 2 == 1 ? 1.0f : 0.0f), Color.White);
                if (spaceship.Render)
                    spriteBatch.Draw(spaceship.Image, destinationRectangle: spaceship.Rect, effects: spaceship.Orientation);
                if (smoke.Render)
                    spriteBatch.Draw(smoke.Image, smoke.Rect, smoke.SourceRect, Color.White);
                if (avatar.Render)
                    spriteBatch.Draw(avatar.Image, avatar.ScaleRect, avatar.SourceRect, Color.White);
                if (fade.Render)
                    spriteBatch.Draw(fade.Image, fade.Rect, fade.SourceRect, Color.White);
                if (EndGameB.SequenceComplete)
                    spriteBatch.Draw(EndGameB.Text.Image, new Vector2(0.0f, EndGameB.VERTICAL_POSITION_TEXT), Color.White);
                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
