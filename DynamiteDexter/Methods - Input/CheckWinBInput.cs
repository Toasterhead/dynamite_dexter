using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckWinBInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            if (EndGameB.SequenceComplete && (keys.GetPressedKeys().Length > 0 || ButtonPressed(gamepad, gamepadPrev)))
                EndGameB.ButtonPressed = true;

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
