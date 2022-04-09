using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawHighscoreMode()
        {
            Textfield line1 = MenuManager.TheTextfields[0]; //Borrowing text fields from menu system.
            Textfield line2 = MenuManager.TheTextfields[1];
            Textfield line3 = MenuManager.TheTextfields[2];
            Textfield line4 = MenuManager.TheTextfields[3];

            line2.Draw();
            line3.Draw();

            int elapsedTime = universalTimer - universalTimeStamp;

            Texture2D cursorImage = characterSet.GetCharacterImage(HighScore.Selection);

            if (HighScore.Selection == '!')
                cursorImage = line4.Image;
            else if (cursorImage == null)
                cursorImage = Images.Characters.EXCLAMATION;

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
            foreach (IGameObject i in HighScore.SpriteSet)
            {
                if (i is SpriteSheet)
                {
                    SpriteSheet iSheet = i as SpriteSheet;

                    spriteBatch.Draw(
                        i.Image,
                        new Vector2(i.X, i.Y),
                        iSheet.SourceRect,
                        iSheet.TheColor);
                }
                else spriteBatch.Draw(i.Image, new Vector2(i.X, i.Y), Color.White);
            }
            spriteBatch.End();

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
            //Upper Dialog Box
            spriteBatch.Draw(Images.BORDER_TOP_LEFT, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(0, 1 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(0, 2 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_LEFT, new Vector2(0, 3 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_TOP_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, 0), Color.White);
            spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, 1 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, 2 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, 3 * SUB_TILE_SIZE), Color.White);
            for (int i = 1; i < fullfield.X / SUB_TILE_SIZE - 1; i++)
                spriteBatch.Draw(Images.BORDER_TOP, new Vector2(i * SUB_TILE_SIZE, 0), Color.White);
            for (int j = 1; j <= 2; j++)
                for (int i = 1; i < fullfield.X / SUB_TILE_SIZE - 1; i++)
                    spriteBatch.Draw(Images.Characters.SPACE, new Vector2(i * SUB_TILE_SIZE, j * SUB_TILE_SIZE), Color.White);
            for (int i = 1; i < fullfield.X / SUB_TILE_SIZE - 1; i++)
                spriteBatch.Draw(Images.BORDER_BOTTOM, new Vector2(i * SUB_TILE_SIZE, 3 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(line1.Image, new Vector2(SUB_TILE_SIZE, SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(line2.Image, new Vector2(SUB_TILE_SIZE, 2 * SUB_TILE_SIZE), Color.White);
            //Lower Dialog Box
            spriteBatch.Draw(Images.BORDER_TOP_LEFT, new Vector2(0, fullfield.Y - (3 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(0, fullfield.Y - (2 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_LEFT, new Vector2(0, fullfield.Y - (1 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(Images.BORDER_TOP_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, fullfield.Y - (3 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, fullfield.Y - (2 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_RIGHT, new Vector2(fullfield.X - SUB_TILE_SIZE, fullfield.Y - (1 * SUB_TILE_SIZE)), Color.White);
            for (int i = 1; i < fullfield.X / SUB_TILE_SIZE - 1; i++)
                spriteBatch.Draw(Images.BORDER_TOP, new Vector2(i * SUB_TILE_SIZE, fullfield.Y - (3 * SUB_TILE_SIZE)), Color.White);
            for (int i = 1; i < fullfield.X / SUB_TILE_SIZE - 1; i++)
                spriteBatch.Draw(Images.BORDER_BOTTOM, new Vector2(i * SUB_TILE_SIZE, fullfield.Y - (1 * SUB_TILE_SIZE)), Color.White);
            spriteBatch.Draw(line3.Image, new Vector2(SUB_TILE_SIZE, fullfield.Y - (2 * SUB_TILE_SIZE)), Color.White);
            if (elapsedTime % 10 < 5)
            {
                INVERT.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(
                    cursorImage,
                    new Vector2(
                            SUB_TILE_SIZE + (HighScore.VirtualKeypadIndex * SUB_TILE_SIZE),
                        fullfield.Y - (2 * SUB_TILE_SIZE)),
                    Color.White);
            }
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (elapsedTime < SCREEN_FLASH_TIME && elapsedTime % 2 == 0)
                INVERT.CurrentTechnique.Passes[0].Apply();
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
