﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void CheckPauseInput()
        {
            keys = Keyboard.GetState();
            gamepad = GamePad.GetState(PlayerIndex.One);

            while (MenuManager.PauseMenu.GetCurrentSelected() is MIHeadline)
                MenuManager.PauseMenu.CurrentSubMenu.MoveCursorDown();

            if ((keys.IsKeyDown(Keys.Down) && keysPrev.IsKeyUp(Keys.Down)) || DPadDownPressed(gamepad, gamepadPrev))
                MenuManager.PauseMenu.CurrentSubMenu.MoveCursorDown();
            else if ((keys.IsKeyDown(Keys.Up) && keysPrev.IsKeyUp(Keys.Up)) || DPadUpPressed(gamepad, gamepadPrev))
                MenuManager.PauseMenu.CurrentSubMenu.MoveCursorUp();
            else if ((keys.IsKeyDown(Keys.Escape) && keysPrev.IsKeyUp(Keys.Escape)) || ButtonPressed(gamepad, gamepadPrev))
                gameMode = GameModes.Action;
            else if (
                (keys.IsKeyDown(Keys.Enter) && keysPrev.IsKeyUp(Keys.Enter)) || 
                ButtonPressed(gamepad, gamepadPrev) ||
                (gamepad.Buttons.Start == ButtonState.Pressed && gamepadPrev.Buttons.Start == ButtonState.Released))

                MenuManager.PauseMenu.CurrentSubMenu.Select();

            keysPrev = keys;
            gamepadPrev = gamepad;
        }
    }
}