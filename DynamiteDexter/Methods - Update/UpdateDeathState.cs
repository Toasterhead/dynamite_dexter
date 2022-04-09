using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateDeathState()
        {
            int timeElapsed = universalTimer - universalTimeStamp;

            if (timeElapsed > SCRIPT_INTERVAL * 9)
            {
                bool flagsRaised = false;

                foreach (FlagStatus i in flagStatus)

                    if (i.raised)
                    { 
                        flagsRaised = true;
                        break;
                    }

                if (player.Lives <= 0)
                {
                    PlayMusic(Sounds.Music.GAME_OVER);
                    universalTimeStamp = universalTimer;
                    gameMode = GameModes.GameOver;
                }
                else if (flagsRaised)
                {
                    MenuManager.ConstructWaypointMenu();
                    gameMode = GameModes.Waypoint;
                }
                else
                {
                    worldCursor = new Point(BEGIN_ROOM_X, BEGIN_ROOM_Y);
                    ChangeRoom();
                    player.Reposition(BEGIN_POSITION_X, BEGIN_POSITION_Y);
                    gameMode = GameModes.Action;
                }
            }
            else if (timeElapsed == SCRIPT_INTERVAL * 4)
                PlaySound(Sounds.THUMP);

            universalTimer++;
        }
    }
}
