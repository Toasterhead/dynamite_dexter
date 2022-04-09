using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateHighScoreState() 
        {
            Textfield line2 = MenuManager.TheTextfields[1]; //Borrowing text fields from menu system.
            Textfield line3 = MenuManager.TheTextfields[2];
          
            line2.ChangeText(HighScore.PROMPT + HighScore.NameEntry + "*");
            line3.ChangeText(
                HighScore.VirtualCapsLockOn ? 
                HighScore.VIRTUAL_KEYPAD_UPPERCASE + HighScore.CONFIRMATION: 
                HighScore.VIRTUAL_KEYPAD_LOWERCASE + HighScore.CONFIRMATION);

            HighScore.ConfettiFalling = !HighScore.ConfettiFalling;

            foreach (IGameObject i in HighScore.SpriteSet)
            {
                i.Update();

                if (i.Image == Images.BALLOON)
                    i.Reposition(i.X, i.Y - 1);
                else if (i.Image == Images.STREAMER)
                    i.Reposition(i.X, i.Y + 1);
                else if (i.Image == Images.CONFETTI && HighScore.ConfettiFalling)
                    i.Reposition(i.X, i.Y + 1);

                if (i.Bottom < 0)
                    i.Reposition(rand.Next(fullfield.X), fullfield.Y + rand.Next(50));
                else if (i.Top > fullfield.Y)
                    i.Reposition(rand.Next(fullfield.X), -i.Height - rand.Next(50));
            }

            if (HighScore.Finished)
            {
                if (HighScore.SubmittingSpeedrun)
                    HighScore.InsertScore(HighScore.NameEntry, (int)inGameTimer, false, true);
                else HighScore.InsertScore(HighScore.NameEntry, player.Dollars, winBig ? true : false);

                HighScore.WriteToMenu();

                if (HighScore.SubmittingSpeedrun)
                {
                    foreach (HighScore.FieldEntry i in HighScore.TopScoreDollars)

                        if (player.Dollars > i.score)
                        {
                            HighScore.SubmittingSpeedrun = false;

                            gameMode = GameModes.HighScore;
                            HighScore.InitializeSprites();
                            HighScore.ResetFields();
                            PlayMusic(Sounds.Music.FANFARE);

                            //Change header to match match new score type.
                            MenuManager.TheTextfields[0].ChangeText(HighScore.Header);
                            MenuManager.TheTextfields[0].Draw();

                            break;
                        }
                }
                else
                { 
                    gameMode = GameModes.Title;
                    PlayMusic(Sounds.Music.TITLE);
                }
            }

            universalTimer++;
        }
    }
}
