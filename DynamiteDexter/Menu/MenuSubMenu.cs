using System;
using System.Collections.Generic;

namespace DynamiteDexter
{
    public class SubMenu
    {
        public readonly string Title;

        protected List<MenuItem> menuItems;
        protected int previous;
        protected int selectionIndex;
        protected bool wrapAround;
        protected bool gridLayout;
        protected int itemsPerRow = 10;

        public Menu Master { get; set; }
        public int SelectionIndex { get { return selectionIndex; } }

        public SubMenu(string title = null, bool wrapAround = false, bool gridLayout = false)
        {
            menuItems = new List<MenuItem>();
            Title = title;
            this.wrapAround = wrapAround;
            this.gridLayout = gridLayout;
            selectionIndex = 0;
            previous = 0;
        }

        public SubMenu(List<MenuItem> menuItems, string title = null, bool wrapAround = false, bool gridLayout = false)
        {
            this.menuItems = menuItems;
            Title = title;
            this.wrapAround = wrapAround;
            this.gridLayout = gridLayout;
            selectionIndex = 0;
            previous = 0;
        }

        #region Item Management

        public void SetItems(List<MenuItem> menuItems) { this.menuItems = menuItems; }

        public void AddItem(MenuItem menuItem, int? index = null)
        {
            if (index == null)
                menuItems.Add(menuItem);
            else menuItems.Insert((int)index, menuItem);
        }

        public void RemoveItem(MenuItem menuItem) { menuItems.Remove(menuItem); }

        public void RemoveItemByIndex(int index) { menuItems.Remove(menuItems[index]); }

        public int Length { get { return menuItems.Count; } }

        #endregion

        #region Item Selection Methods

        //Implement iterator later.
        public MenuItem GetAtIndex(int index) { return menuItems[index]; }

        public MenuItem GetSelection() { return menuItems[selectionIndex]; }

        public void Select(bool reverse = false)
        {
            MenuItem menuItem = menuItems[selectionIndex];

            if (!(menuItem is ISelectable))
                throw new Exception("Error - It shouldn't be possible to select this type.");
            else if (menuItem is MILink)
                Master.CurrentSubMenu = (menuItem as MILink).TheSubMenu;
            else if (menuItem is MICommandLink)
            {
                MICommandLink commandLink = menuItem as MICommandLink;
                commandLink.Execute();
                Master.CurrentSubMenu = commandLink.TheSubMenu;
            }
            else if (menuItem is MICommand)
                (menuItem as MICommand).Execute();
            else if (menuItem is IDirectional)
            {
                IDirectional directionalItem = menuItem as IDirectional;

                if (reverse)
                    directionalItem.Backward();
                else directionalItem.Forward();
            }
            else throw new Exception("Error - Unable to recognize menu item type.");
        }

        #endregion

        #region Cursor Movement Methods

        public void MoveCursorUp()
        {
            if (!gridLayout)
            {
                selectionIndex--;

                while (
                    selectionIndex < 0 ||
                    !(menuItems[selectionIndex] is ISelectable) ||
                    (menuItems[selectionIndex] is ISelectable && (menuItems[selectionIndex] as ISelectable).Muted) ||
                    (menuItems[selectionIndex] is ISelectable && (menuItems[selectionIndex] as ISelectable).Invisible))
                {
                    if (selectionIndex < 0)
                    {
                        if (wrapAround)
                            selectionIndex = Length - 1;
                        else selectionIndex = 0;
                    }
                    else selectionIndex--;
                }
            }
        }

        public void MoveCursorRight()
        {
            if (!gridLayout) MoveCursorDown();
        }

        public void MoveCursorDown()
        {
            if (!gridLayout)
            {
                //Note -    If a space or headline exists as the last item and wrap-around
                //          is off, the program will be caught in an infinite loop. Fix later.

                selectionIndex++;

                while (
                    selectionIndex >= Length ||
                    !(menuItems[selectionIndex] is ISelectable) ||
                    (menuItems[selectionIndex] is ISelectable && (menuItems[selectionIndex] as ISelectable).Muted) ||
                    (menuItems[selectionIndex] is ISelectable && (menuItems[selectionIndex] as ISelectable).Invisible))
                {
                    if (selectionIndex >= Length)
                    {
                        if (wrapAround)
                            selectionIndex = 0;
                        else selectionIndex = Length - 1;
                    }
                    else selectionIndex++;
                }
            }
        }

        public void MoveCursorLeft()
        {
            if (!gridLayout) MoveCursorUp();
        }

        #endregion

        #region Index-to-grid-position Converters

        protected int GridX(int index) { return index % itemsPerRow; }

        protected int GridY(int index) { return index / itemsPerRow; }

        #endregion
    }
}
