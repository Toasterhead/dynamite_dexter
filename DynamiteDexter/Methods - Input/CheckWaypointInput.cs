using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckWaypointInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            while (MenuManager.WaypointMenu.GetCurrentSelected() is MIHeadline)
                MenuManager.WaypointMenu.CurrentSubMenu.MoveCursorDown();

            if ((keys.IsKeyDown(Keys.Down) && keysPrev.IsKeyUp(Keys.Down)) || DPadDownPressed(gamepad, gamepadPrev))
                MenuManager.WaypointMenu.CurrentSubMenu.MoveCursorDown();
            else if ((keys.IsKeyDown(Keys.Up) && keysPrev.IsKeyUp(Keys.Up)) || DPadUpPressed(gamepad, gamepadPrev))
                MenuManager.WaypointMenu.CurrentSubMenu.MoveCursorUp();
            else if (
                (keys.IsKeyDown(Keys.Enter) && keysPrev.IsKeyUp(Keys.Enter)) || 
                ButtonPressed(gamepad, gamepadPrev) ||
                gamepad.Buttons.Start == ButtonState.Pressed && gamepadPrev.Buttons.Start == ButtonState.Released)

                MenuManager.WaypointMenu.CurrentSubMenu.Select();

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
