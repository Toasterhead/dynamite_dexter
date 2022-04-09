using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckActionInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            if (keys.IsKeyDown(Keys.Up) || gamepad.DPad.Up == ButtonState.Pressed) { player.MoveUp(); }
            else if (keys.IsKeyDown(Keys.Down) || gamepad.DPad.Down == ButtonState.Pressed) { player.MoveDown(); }
            else if (keys.IsKeyDown(Keys.Left) || gamepad.DPad.Left == ButtonState.Pressed) { player.MoveLeft(); }
            else if (keys.IsKeyDown(Keys.Right) || gamepad.DPad.Right == ButtonState.Pressed) { player.MoveRight(); }
            else { player.Neutral(); }

            if ((keys.IsKeyDown(Keys.Space) && keysPrev.IsKeyUp(Keys.Space)) || ButtonPressed(gamepad, gamepadPrev))
            
                player.PlaceDynamite();

            else if (
                (keys.IsKeyDown(Keys.Escape) && keysPrev.IsKeyUp(Keys.Escape)) || 
                (gamepad.Buttons.Start == ButtonState.Pressed && gamepadPrev.Buttons.Start == ButtonState.Released))
            
                gameMode = GameModes.Pause;

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
