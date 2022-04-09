using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckTitleInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            bool isSlider = MenuManager.TitleMenu.GetCurrentSelected() is MISlider;
            bool isSwitch = MenuManager.TitleMenu.GetCurrentSelected() is MISwitch;
            bool isDial = MenuManager.TitleMenu.GetCurrentSelected() is MIDial;

            while (MenuManager.TitleMenu.GetCurrentSelected() is MIHeadline)
                MenuManager.TitleMenu.CurrentSubMenu.MoveCursorDown();

            if ((keys.IsKeyDown(Keys.Down) && keysPrev.IsKeyUp(Keys.Down)) || DPadDownPressed(gamepad, gamepadPrev))
                MenuManager.TitleMenu.CurrentSubMenu.MoveCursorDown();
            else if ((keys.IsKeyDown(Keys.Up) && keysPrev.IsKeyUp(Keys.Up)) || DPadUpPressed(gamepad, gamepadPrev))
                MenuManager.TitleMenu.CurrentSubMenu.MoveCursorUp();
            else if ((keys.IsKeyDown(Keys.Enter) && keysPrev.IsKeyUp(Keys.Enter)) || ButtonPressed(gamepad, gamepadPrev))
                MenuManager.TitleMenu.CurrentSubMenu.Select();
            else if ((keys.IsKeyDown(Keys.Left) && keysPrev.IsKeyUp(Keys.Left)) || DPadLeftPressed(gamepad, gamepadPrev))
            {
                if (isSlider)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MISlider).Decrease();
                else if (isSwitch)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MISwitch).Flip();
                else if (isDial)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MIDial).CycleCounterClockwise();
            }
            else if ((keys.IsKeyDown(Keys.Right) && keysPrev.IsKeyUp(Keys.Right)) || DPadRightPressed(gamepad, gamepadPrev))
            {
                if (isSlider)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MISlider).Increase();
                else if (isSwitch)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MISwitch).Flip();
                else if (isDial)
                    (MenuManager.TitleMenu.GetCurrentSelected() as MIDial).CycleClockwise();
            }

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}
