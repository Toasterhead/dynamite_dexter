using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckTextInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            if ((keys.GetPressedKeys().Length > 0 || ButtonPressed(gamepad, gamepadPrev)) && 
                universalTimer - universalTimeStamp > 30)
            {
                if (winCount != null)
                {
                    if (universalTimer - universalTimeStamp > 150)
                    {
                        if (winBig)
                        {
                            EndGameB.InitializeScene();
                            PlayMusic(Sounds.Music.VICTORY_LULLABY, repeat: true);
                            gameMode = GameModes.WinB;
                        }
                        else
                        {
                            EndGameA.InitializeScene();
                            PlayMusic(Sounds.Music.VICTORY_MARCH, repeat: true);
                            gameMode = GameModes.WinA;
                        }
                    }
                }
                else
                {
                    //Stop music if departing from the house.
                    Room currentRoom = WorldCursorInRange() ? worldSet[worldCursor.X, worldCursor.Y] : null;
                    if (currentRoom != null || (currentRoom.TheMode != Room.Mode.Ominous && currentRoom.TheMode != Room.Mode.House))
                        MediaPlayer.Stop();

                    gameMode = GameModes.Action;
                    player.Neutral();
                }
            }

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
