using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void SubUpdateGameOver()
        {
            universalTimeStamp = universalTimer;
            gameMode = GameModes.Title;

            //Memory cleanup.
            textLives.DisposeSurface();
            textDynamite.DisposeSurface();
            textDollars.DisposeSurface();
            textKeys.DisposeSurface();
            textMessage.DisposeSurface();
            textPlaque.DisposeSurface();

            HighScore.SubmittingSpeedrun = false;

            if (winCount != null)
            {
                foreach (HighScore.FieldEntry i in HighScore.TopScoreSpeedrun)

                    if (inGameTimer < i.score)
                    {
                        HighScore.SubmittingSpeedrun = true;

                        gameMode = GameModes.HighScore;
                        HighScore.InitializeSprites();
                        HighScore.ResetFields();
                        PlayMusic(Sounds.Music.FANFARE);

                        break;
                    }
            }

            if (!HighScore.SubmittingSpeedrun)
            {
                foreach (HighScore.FieldEntry i in HighScore.TopScoreDollars)

                    if (player.Dollars > i.score)
                    {
                        gameMode = GameModes.HighScore;
                        HighScore.InitializeSprites();
                        HighScore.ResetFields();
                        PlayMusic(Sounds.Music.FANFARE);

                        break;
                    }
            }

            //Borrowing text fields from menu system.
            MenuManager.TheTextfields[0].ChangeText(HighScore.Header);
            MenuManager.TheTextfields[1].ChangeText(HighScore.PROMPT + "_");
            MenuManager.TheTextfields[2].ChangeText(HighScore.VIRTUAL_KEYPAD_LOWERCASE + HighScore.CONFIRMATION);
            MenuManager.TheTextfields[3].ChangeText(HighScore.CONFIRMATION);

            //The first and fourth lines only needs to be drawn once.
            MenuManager.TheTextfields[0].Draw();
            MenuManager.TheTextfields[3].Draw();
        }
    }
}
