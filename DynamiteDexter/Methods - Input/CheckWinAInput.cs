using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckWinAInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            if (EndGameA.Marching && 
                EndGameA.CreditsComplete && 
                ((keys.GetPressedKeys().Length > 0) || ButtonPressed(gamepad, gamepadPrev)))

                EndGameA.ButtonPressed = true;

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
