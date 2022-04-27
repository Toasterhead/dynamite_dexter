using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void DrawTitleMode()
        {
            const int MARGIN_TOP = 14 * SUB_TILE_SIZE;
            const int MAIN_MARGIN_LEFT = 15 * SUB_TILE_SIZE;
            const int HIGH_SCORE_MARGIN_LEFT = 4 * SUB_TILE_SIZE;
            const int SETTINGS_MARGIN_LEFT = 8 * SUB_TILE_SIZE;
            const int ABOUT_MARGIN_LEFT = 5 * SUB_TILE_SIZE;
            const int TITLE_MARGIN_TOP = 6 * SUB_TILE_SIZE;
            const int SMALL_TITLE_MARGIN_LEFT = 14 * SUB_TILE_SIZE;
            const int SMALL_TITLE_MARGIN_TOP = 6 * SUB_TILE_SIZE;

            bool displayTitleGraphic = true;
            bool displayTrophies = false;
            bool displayStarIcons = false;
            int marginLeft;

            switch (MenuManager.TitleMenu.CurrentSubMenu.Title)
            {
                case "Main": marginLeft = MAIN_MARGIN_LEFT;
                    break;
                case "High Score":  marginLeft = HIGH_SCORE_MARGIN_LEFT;
                    displayTitleGraphic = false;
                    displayTrophies = true;
                    displayStarIcons = true;
                    break;
                case "Speed Run": marginLeft = HIGH_SCORE_MARGIN_LEFT;
                    displayTitleGraphic = false;
                    displayTrophies = true;
                    break;
                case "Settings": marginLeft = SETTINGS_MARGIN_LEFT;
                    break;
                case "About": marginLeft = ABOUT_MARGIN_LEFT;
                    break;
                default: throw new System.Exception("Error - unable to recognize sub-menu title.");
            }

            for (int i = 0; i < MenuManager.TitleMenu.CurrentSubMenuLength; i++)
            {
                MenuManager.TheTextfields[i].ChangeText(MenuManager.TitleMenu.GetCurrentAtIndex(i).ToString());
                MenuManager.TheTextfields[i].Draw();
            }

            if (!displayTitleGraphic)
            {
                string menuTitle = MenuManager.TitleMenu.CurrentSubMenu.Title;
                menuTitle = menuTitle.ToUpper();

                MenuManager.TheTextfields[MenuManager.TheTextfields.Length - 1].ChangeText(menuTitle);
                MenuManager.TheTextfields[MenuManager.TheTextfields.Length - 1].Draw();
            }

            GraphicsDevice.SetRenderTarget(canvasFull);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (displayTitleGraphic)
                spriteBatch.Draw(Images.TITLE, new Vector2((fullfield.X / 2) - (Images.TITLE.Width / 2), TITLE_MARGIN_TOP), Color.White);
            else spriteBatch.Draw(
                MenuManager.TheTextfields[MenuManager.TheTextfields.Length - 1].Image,
                new Vector2(
                    SMALL_TITLE_MARGIN_LEFT,
                    SMALL_TITLE_MARGIN_TOP),
                Color.White);
            if (displayTrophies)
            {
                spriteBatch.Draw(Images.TROPHY, new Vector2(4 * SUB_TILE_SIZE, SMALL_TITLE_MARGIN_TOP - SUB_TILE_SIZE), Color.White);
                spriteBatch.Draw(Images.TROPHY, new Vector2(fullfield.X - (6 * SUB_TILE_SIZE), SMALL_TITLE_MARGIN_TOP - SUB_TILE_SIZE), Color.White);
            }
            if (displayStarIcons)
            {
                for (int i = 0; i < HighScore.TopScoreDollars.Length; i++)
                    if (HighScore.TopScoreDollars[i].star)
                        spriteBatch.Draw(
                            Images.ICON_STAR, 
                            new Vector2(
                                marginLeft - (2 * SUB_TILE_SIZE), 
                                i * Images.ICON_STAR.Height + MARGIN_TOP),
                            Color.White);
            }
            for (int i = 0; i < MenuManager.TitleMenu.CurrentSubMenuLength; i++)
            {
                MenuItem menuItem = MenuManager.TitleMenu.CurrentSubMenu.GetAtIndex(i);

                if (menuItem is ISelectable && (menuItem as ISelectable).Muted)
                    spriteBatch.Draw(
                        textUnavailable.Image,
                        new Vector2(marginLeft, i * SUB_TILE_SIZE + MARGIN_TOP),
                        Color.White);
                else spriteBatch.Draw(
                    MenuManager.TheTextfields[i].Image, 
                    new Vector2(marginLeft, i * SUB_TILE_SIZE + MARGIN_TOP), 
                    Color.White);
            }
            spriteBatch.Draw(
                Images.ICON_CURSOR, 
                new Vector2(marginLeft - (2 * SUB_TILE_SIZE), 
                MenuManager.TitleMenu.CurrentSubMenu.SelectionIndex * SUB_TILE_SIZE + MARGIN_TOP), 
                Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SubDrawFull();
            spriteBatch.End();
        }
    }
}
