using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateWinAState()
        {
            int elapsedTime = universalTimer - universalTimeStamp;

            foreach (SpriteSheet i in EndGameA.Animals)
                i.Update();

            EndGameA.CheckHalt();
            if (EndGameA.CheckLeave())
            {
                universalTimeStamp = universalTimer;
                elapsedTime = 0;
            }

            if (EndGameA.Marching && elapsedTime % EndGameA.CHANGE_TEXT_INTERVAL == 0)
                EndGameA.ChangeText();

            if (!EndGameA.Leaving)
                EndGameA.UpdateScroll();
            else if (EndGameA.Leaving && elapsedTime > EndGameA.OUTRO_DURATION)
                SubUpdateGameOver(); //Note - Changes game mode to Title or HighScore.

            universalTimer++;
        }
    }
}
