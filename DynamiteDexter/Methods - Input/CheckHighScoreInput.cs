using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckHighScoreInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            if ((keys.IsKeyDown(Keys.Left) && keysPrev.IsKeyUp(Keys.Left)) || DPadLeftPressed(gamepad, gamepadPrev))
                HighScore.MoveCursorLeft();
            else if ((keys.IsKeyDown(Keys.Right) && keysPrev.IsKeyUp(Keys.Right)) || DPadRightPressed(gamepad, gamepadPrev))
                HighScore.MoveCursorRight();
            else if ((keys.IsKeyDown(Keys.Enter) && keysPrev.IsKeyUp(Keys.Enter)) || ButtonPressed(gamepad, gamepadPrev))
                HighScore.ExecuteCommand();

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
