using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Textfield : Sprite
    {
        //Note - Memory "leak" found and addressed. Replace variations of this class in other projects.

        public enum WriteMode { Immediate, Auto, Manual }

        protected readonly CharacterSet characterSet;
        protected readonly WriteMode writeMode;
        protected readonly uint writeDelay;

        protected uint delayCount;
        protected uint charLimit;
        protected string[] words;

        private RenderTarget2D surface;

        public int CharacterWidth { get { return Width / characterSet.FontWidth; } }

        public Textfield(
            CharacterSet characterSet,
            string text,
            int x,
            int y,
            int? fieldWidth,
            WriteMode writeMode = WriteMode.Immediate,
            uint writeDelay = 10,
            int layer = 0)
            : base(Images.BLOCK, x, y, layer)
        {
            words = text.Split(' ');

            if (fieldWidth == null)
            {
                width = text.Length * characterSet.FontWidth;
                height = characterSet.FontHeight;
            }
            else
            {
                width = (int)fieldWidth;
                height = DetermineHeight(characterSet, text, (int)fieldWidth);
            }

            surface = new RenderTarget2D(Game1.theGraphicsDevice, width, height);

            this.writeMode = writeMode;
            this.writeDelay = writeDelay;
            this.characterSet = characterSet;

            delayCount = writeDelay;
            charLimit = 0;

            if (writeMode != WriteMode.Manual) Draw();
        }

        protected int DetermineHeight(CharacterSet characterSet, string text, int width)
        {
            int numLines = 1;
            int currentLineLength = 0;

            for (int i = 0; i < text.Length; i++)
            
                if (text[i] == '\n' || currentLineLength >= width / characterSet.FontWidth)
                {
                    numLines++;
                    currentLineLength = 0;
                }
                else currentLineLength++;

            return numLines * characterSet.FontHeight;
        }

        public virtual void ChangeText(string text) { words = text.Split(' ');  charLimit = 0; }

        public virtual void Draw()
        {
            if (writeMode == WriteMode.Auto && charLimit != int.MaxValue)
            {
                if (--delayCount == 0)
                    delayCount = writeDelay;
                else return;
            }
            else if (width == 0) return;

            Point cursor = new Point(0, 0);
            uint numDrawn = 0;

            Game1.theGraphicsDevice.SetRenderTarget(surface);
            Game1.theGraphicsDevice.Clear(Color.Transparent);

            Game1.spriteBatch.Begin();

            foreach (string word in words)
            {
                int wordWidth = word.Length * characterSet.FontWidth;

                if (cursor.X + wordWidth > width)
                    cursor = new Point(0, cursor.Y + characterSet.FontHeight);

                bool containsLineBreak = false;

                foreach (char ch in word)
                {
                    Texture2D charImage = characterSet.GetCharacterImage(ch);

                    if (ch == '\n')
                    { 
                        cursor = new Point(0, cursor.Y + characterSet.FontHeight);
                        containsLineBreak = true;
                    }
                    else
                    {
                        Game1.spriteBatch.Draw(
                            charImage == null ? characterSet.Exclamation : charImage,
                            new Rectangle(
                                cursor.X,
                                cursor.Y,
                                characterSet.FontWidth,
                                characterSet.FontHeight),
                            Color.White);

                        cursor = new Point(cursor.X + characterSet.FontWidth, cursor.Y);
                    }

                    if ((writeMode == WriteMode.Auto || writeMode == WriteMode.Manual) && ++numDrawn > charLimit)
                    {
                        int charactersPerLine = width / characterSet.FontWidth;
                        int numLines = height / characterSet.FontHeight;
                        int totalPossibleCharacters = numLines * charactersPerLine;

                        if (charLimit < totalPossibleCharacters)
                            charLimit++;
                        else charLimit = int.MaxValue;

                        Game1.spriteBatch.End();
                        image = surface;
                        return;
                    }
                }
                if (!containsLineBreak)
                {
                    if (cursor.X + characterSet.FontWidth <= width)
                    {
                        Game1.spriteBatch.Draw(
                            characterSet.GetCharacterImage(' '),
                            new Rectangle(
                                cursor.X,
                                cursor.Y,
                                characterSet.FontWidth,
                                characterSet.FontHeight),
                            Color.White);

                        cursor = new Point(cursor.X + characterSet.FontWidth, cursor.Y);
                    }
                    else cursor = new Point(0, cursor.Y + characterSet.FontHeight);
                }
            }

            Game1.spriteBatch.End();

            image = surface;
        }

        public virtual void DisposeSurface() { surface.Dispose(); }

        public override void Update()
        {
            if (writeMode == WriteMode.Auto) Draw();
            base.Update();
        }
    }
}
