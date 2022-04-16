using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawActionMode()
        {
            if (gameMode == GameModes.Pause)
            {
                for (int i = 0; i < MenuManager.PauseMenu.CurrentSubMenuLength; i++)
                {
                    MenuManager.TheTextfields[i].ChangeText(MenuManager.PauseMenu.GetCurrentAtIndex(i).ToString());
                    MenuManager.TheTextfields[i].Draw();
                }
            }

            bool isDark =
                WorldCursorInRange() &&
                worldSet[worldCursor.X, worldCursor.Y] != null &&
                worldSet[worldCursor.X, worldCursor.Y].TheMode == Room.Mode.Darkened &&
                (lightUpTimer == 0 || (lightUpTimer < (LIGHT_UP_TIME / 2) && lightUpTimer % 2 == 0));

            GraphicsDevice.SetRenderTarget(canvasAction);
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
            foreach (IGameObject i in spriteSet)
            {
                if (i.Render)
                {
                    if (i is IGameObjectAnimated)
                    {
                        IGameObjectAnimated iAnimated = i as IGameObjectAnimated;
                        spriteBatch.Draw(
                            i.Image,
                            destinationRectangle: i.Rect,
                            sourceRectangle: iAnimated.SourceRect,
                            effects: iAnimated.Orientation,
                            layerDepth: iAnimated.Layer / (float)Layers.EnumSize);
                    }
                    else if (i is IGameObjectPlus)
                    {
                        IGameObjectPlus iPlus = i as IGameObjectPlus;
                        spriteBatch.Draw(
                            i.Image,
                            destinationRectangle: i.Rect,
                            effects: iPlus.Orientation,
                            layerDepth: iPlus.Layer / (float)Layers.EnumSize);
                    }
                    else spriteBatch.Draw(
                        i.Image,
                        destinationRectangle: i.Rect,
                        layerDepth: i.Layer / (float)Layers.EnumSize);
                }
            }
            if (isDark)
                spriteBatch.Draw(
                    Images.SPOTLIGHT, 
                    new Vector2(
                        player.Center.X - (Images.SPOTLIGHT.Width / 2), 
                        player.Center.Y - (Images.SPOTLIGHT.Height / 2)), 
                    Color.White);
            if (curtainTimer > 0)
            {
                uint curtainTimerAdjusted = curtainTimer % 2 == 0 ? curtainTimer : curtainTimer + 1; 
                int halfWidth = playfield.X / 2;
                int displacement = halfWidth - ((int)(curtainTimerAdjusted * halfWidth) / CURTAIN_PULL_DURATION);
                spriteBatch.Draw(
                    Images.BLANK,
                    new Rectangle(
                        -displacement,
                        0,
                        playfield.X / 2,
                        playfield.Y),
                    Color.Green);
                spriteBatch.Draw(
                    Images.BLANK,
                    new Rectangle(
                        displacement + halfWidth,
                        0,
                        playfield.X / 2,
                        playfield.Y),
                    Color.Red);
                curtainTimer--;
            }
            spriteBatch.End();

            //Draw Pause Menu if the game is paused.

            const int PAUSE_LEFT = 4;
            const int PAUSE_RIGHT = 26;
            const int PAUSE_TOP = 10;
            const int PAUSE_BOTTOM = 19;

            if (gameMode == GameModes.Pause)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
                spriteBatch.Draw(Images.BORDER_TOP_LEFT, new Vector2(PAUSE_LEFT * SUB_TILE_SIZE, PAUSE_TOP * SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.BORDER_TOP_RIGHT, new Vector2(PAUSE_RIGHT * SUB_TILE_SIZE, PAUSE_TOP * SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.BORDER_BOTTOM_LEFT, new Vector2(PAUSE_LEFT * SUB_TILE_SIZE, PAUSE_BOTTOM * SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.BORDER_BOTTOM_RIGHT, new Vector2(PAUSE_RIGHT * SUB_TILE_SIZE, PAUSE_BOTTOM * SUB_TILE_SIZE), Color.White);
                for (int i = PAUSE_LEFT + 1; i < PAUSE_RIGHT; i++)
                {
                    spriteBatch.Draw(Images.BORDER_TOP, new Vector2(i * SUB_TILE_SIZE, PAUSE_TOP * SUB_TILE_SIZE), Color.White);
                    spriteBatch.Draw(Images.BORDER_BOTTOM, new Vector2(i * SUB_TILE_SIZE, PAUSE_BOTTOM * SUB_TILE_SIZE), Color.White);
                }
                for (int i = PAUSE_TOP + 1; i < PAUSE_BOTTOM; i++)
                {
                    spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(PAUSE_LEFT * SUB_TILE_SIZE, i * SUB_TILE_SIZE), Color.White);
                    spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(PAUSE_RIGHT * SUB_TILE_SIZE, i * SUB_TILE_SIZE), Color.White);
                }
                for (int i = PAUSE_LEFT + 1; i < PAUSE_RIGHT; i++)
                    for (int j = PAUSE_TOP + 1; j < PAUSE_BOTTOM; j++)
                        spriteBatch.Draw(Images.Characters.SPACE, new Vector2(i * SUB_TILE_SIZE, j * SUB_TILE_SIZE), Color.White);
                for (int i = 0; i < MenuManager.PauseMenu.CurrentSubMenuLength; i++)
                    spriteBatch.Draw(
                        MenuManager.TheTextfields[i].Image,
                        new Vector2((PAUSE_LEFT + 4) * SUB_TILE_SIZE , (PAUSE_TOP + 2 + i) * SUB_TILE_SIZE),
                        Color.White);
                spriteBatch.Draw(
                    Images.ICON_CURSOR,
                    new Vector2(
                        (PAUSE_LEFT + 2) * SUB_TILE_SIZE,
                        (PAUSE_TOP + 2 + MenuManager.PauseMenu.CurrentSubMenu.SelectionIndex) * SUB_TILE_SIZE),
                    Color.White);
                spriteBatch.End();
            }

            textLives.ChangeText(player.Lives.ToString());
            textDynamite.ChangeText(player.DynamiteSticks.ToString());
            textKeys.ChangeText(player.Keys.ToString());
            textDollars.ChangeText(player.Dollars.ToString());
            textLives.Draw();
            textDynamite.Draw();
            textKeys.Draw();
            textDollars.Draw();

            GraphicsDevice.SetRenderTarget(canvasHud);

            spriteBatch.Begin();
            spriteBatch.Draw(Images.BORDER_TOP_LEFT, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(Images.BORDER_TOP_RIGHT, new Vector2(canvasHud.Width - SUB_TILE_SIZE, 0), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_LEFT, new Vector2(0, canvasHud.Height - SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.BORDER_BOTTOM_RIGHT, new Vector2(canvasHud.Width - SUB_TILE_SIZE, canvasHud.Height - SUB_TILE_SIZE), Color.White);
            for (int i = 1; i< (canvasHud.Width / SUB_TILE_SIZE) - 1; i++)
            {
                spriteBatch.Draw(Images.BORDER_TOP, new Vector2(i* SUB_TILE_SIZE, 0), Color.White);
                spriteBatch.Draw(Images.BORDER_BOTTOM, new Vector2(i* SUB_TILE_SIZE, canvasHud.Height - SUB_TILE_SIZE), Color.White);
            }
            for (int i = 1; i< (canvasHud.Height / SUB_TILE_SIZE) - 1; i++)
            {
                spriteBatch.Draw(Images.BORDER_LEFT, new Vector2(0, i* SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.BORDER_RIGHT, new Vector2(canvasHud.Width - SUB_TILE_SIZE, i* SUB_TILE_SIZE), Color.White);
            }
            spriteBatch.Draw(Images.ICON_LIVES, new Vector2(1 * SUB_TILE_SIZE, 1 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.COUNT, new Vector2(3 * SUB_TILE_SIZE, 1 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.ICON_DYNAMITE, new Vector2(1 * SUB_TILE_SIZE, 3 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.COUNT, new Vector2(3 * SUB_TILE_SIZE, 3 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.ICON_KEY, new Vector2(1 * SUB_TILE_SIZE, 5 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.COUNT, new Vector2(3 * SUB_TILE_SIZE, 5 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.ICON_DOLLARS, new Vector2(1 * SUB_TILE_SIZE, 7 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(textLives.Image, new Vector2(4 * SUB_TILE_SIZE, SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(textDynamite.Image, new Vector2(4 * SUB_TILE_SIZE, 3 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(textKeys.Image, new Vector2(4 * SUB_TILE_SIZE, 5 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(textDollars.Image, new Vector2(4 * SUB_TILE_SIZE, 7 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(Images.ICON_FLAG, new Vector2(1 * SUB_TILE_SIZE, 25 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[0].raised? Images.FLAG_1 : Images.FLAG_NEGATIVE, new Vector2(3 * SUB_TILE_SIZE, 25 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[1].raised? Images.FLAG_2 : Images.FLAG_NEGATIVE, new Vector2(4 * SUB_TILE_SIZE, 25 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[2].raised? Images.FLAG_3 : Images.FLAG_NEGATIVE, new Vector2(5 * SUB_TILE_SIZE, 25 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[3].raised? Images.FLAG_4 : Images.FLAG_NEGATIVE, new Vector2(6 * SUB_TILE_SIZE, 25 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[4].raised? Images.FLAG_5 : Images.FLAG_NEGATIVE, new Vector2(3 * SUB_TILE_SIZE, 26 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[5].raised? Images.FLAG_6 : Images.FLAG_NEGATIVE, new Vector2(4 * SUB_TILE_SIZE, 26 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[6].raised? Images.FLAG_7 : Images.FLAG_NEGATIVE, new Vector2(5 * SUB_TILE_SIZE, 26 * SUB_TILE_SIZE), Color.White);
            spriteBatch.Draw(flagStatus[7].raised? Images.FLAG_8 : Images.FLAG_NEGATIVE, new Vector2(6 * SUB_TILE_SIZE, 26 * SUB_TILE_SIZE), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (graphics.IsFullScreen)
                spriteBatch.Draw(
                    canvasHud,
                    new Rectangle(
                        (int)(0.5 * (graphics.PreferredBackBufferWidth - (canvasMultiplier * fullfield.X))) + (canvasMultiplier * playfield.X),
                        (int)(0.5 * (graphics.PreferredBackBufferHeight - (canvasMultiplier * fullfield.Y))),
                        canvasMultiplier * hudfield.X,
                        canvasMultiplier * hudfield.Y),
                    Color.White);
            else spriteBatch.Draw(
                canvasHud,
                new Rectangle(
                    (int)(ACTION_CANVAS_RATIO * graphics.PreferredBackBufferWidth),
                    0,
                    (int)((1.0 - ACTION_CANVAS_RATIO) * graphics.PreferredBackBufferWidth),
                    graphics.PreferredBackBufferHeight),
                Color.White);
            if (isDark)
            {
                float rectLeft = player.Center.X - (Images.SPOTLIGHT.Width / 2);
                float rectRight = player.Center.Y - (Images.SPOTLIGHT.Height / 2);
                DARKEN.Parameters["rectLeft"].SetValue(gameMode == GameModes.Pause ? PAUSE_LEFT * SUB_TILE_SIZE : rectLeft);
                DARKEN.Parameters["rectTop"].SetValue(gameMode == GameModes.Pause ? PAUSE_TOP * SUB_TILE_SIZE : rectRight);
                DARKEN.Parameters["rectRight"].SetValue(gameMode == GameModes.Pause ? (PAUSE_RIGHT + 1) * SUB_TILE_SIZE : rectLeft + Images.SPOTLIGHT.Width);
                DARKEN.Parameters["rectBottom"].SetValue(gameMode == GameModes.Pause ? (PAUSE_BOTTOM + 1) * SUB_TILE_SIZE : rectRight + Images.SPOTLIGHT.Height);
                DARKEN.Parameters["fieldWidth"].SetValue((float)playfield.X);
                DARKEN.Parameters["fieldHeight"].SetValue((float)playfield.Y);
                DARKEN.CurrentTechnique.Passes[0].Apply();
            }
            if (screenFlashTimer > 0 && screenFlashTimer % 2 == 0)
                INVERT.CurrentTechnique.Passes[0].Apply();
            SubDrawAction();
            spriteBatch.End();
        }
    }
}
