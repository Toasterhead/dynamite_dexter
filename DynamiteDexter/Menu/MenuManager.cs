using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public static class MenuManager
    {
        public static class MenuName
        {
            public const string MAIN = "Main";
            public const string HIGH_SCORE = "High Score";
            public const string SPEED_RUN = "Speed Run";
            public const string SETTINGS = "Settings";
            public const string ABOUT = "About";
        }

        public static class MenuItemIndices
        {
            public const int RESOLUTION = 1;
            public const int FILTER = 2;
        }

        private const int NUM_FIELDS = 14;

        public static Textfield[] TheTextfields;
        public static Menu TitleMenu;
        public static Menu WaypointMenu;
        public static Menu PauseMenu;

        public static void ConstructTitleMenu()
        {
            //Obtain supported screen resolutions...

            Point currentResolution = Game1.GetCurrentResolution();
            Point[] gameResolutions = new Point[8];

            for (int i = 0; i < gameResolutions.Length; i++)
                gameResolutions[i] = new Point((i + 1) * Game1.FULLFIELD_SIZE_X, (i + 1) * Game1.FULLFIELD_SIZE_Y);

            List<string> resolutions = new List<string>();

            foreach (Point i in gameResolutions)

                if (currentResolution.X > i.X && currentResolution.Y > i.Y)
                    resolutions.Add(i.X + " x " + i.Y);

            string[] filters = new string[(int)Game1.RenderEffects.EnumSize];

            for (int i = 0; i < (int)Game1.RenderEffects.EnumSize; i++)
                filters[i] = Game1.FilterName((Game1.RenderEffects)i);

            //Construct menus...

            List<DataBool> dataBooleans = new List<DataBool>();
            dataBooleans.Add(new DataBool("fullscreenOn", false));

            List<DataInt> dataIntegers = new List<DataInt>();
            dataIntegers.Add(new DataInt("resolution", 0));
            dataIntegers.Add(new DataInt("filter", 0));

            List<DataDouble> dataDoubles = new List<DataDouble>();
            dataDoubles.Add(new DataDouble("soundVolume", 0.3));

            List<DataString> dataStrings = new List<DataString>();
            for (int i = 0; i < 10; i++)
                dataStrings.Add(new DataString("highScore" + i, "[blank]"));
            for (int i = 0; i < 10; i++)
                dataStrings.Add(new DataString("speedRun" + i, "[blank]"));

            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu(MenuName.MAIN, wrapAround: true));
            subMenus.Add(new SubMenu(MenuName.HIGH_SCORE, wrapAround: true));
            subMenus.Add(new SubMenu(MenuName.SPEED_RUN, wrapAround: true));
            subMenus.Add(new SubMenu(MenuName.SETTINGS, wrapAround: true));
            subMenus.Add(new SubMenu(MenuName.ABOUT, wrapAround: true));

            TitleMenu = new Menu(subMenus, dataBooleans, dataIntegers, dataDoubles, dataStrings);

            MenuItem[] mainItems = new MenuItem[5];
            mainItems[0] = new MICommand("Start", Start);
            mainItems[1] = new MILink("High Score", TitleMenu.GetSubMenu(MenuName.HIGH_SCORE));
            mainItems[2] = new MILink("Settings", TitleMenu.GetSubMenu(MenuName.SETTINGS));
            mainItems[3] = new MILink("About", TitleMenu.GetSubMenu(MenuName.ABOUT));
            mainItems[4] = new MICommand("Quit", Quit);

            MenuItem[] settingsItems = new MenuItem[7];
            settingsItems[0] = new MISwitch(TitleMenu.GetBool("fullscreenOn"), "Fullscreen: ", "Yes", "No");
            settingsItems[1] = new MIDial(TitleMenu.GetInt("resolution"), "Screen Resolution: ", resolutions.ToArray());
            settingsItems[2] = new MIDial(TitleMenu.GetInt("filter"), "Filter: ", filters);
            settingsItems[3] = new MISlider(TitleMenu.GetDouble("soundVolume"), "Sound Volume: ", 10, 1, reportAsRatio: false);
            settingsItems[4] = new MICommand("Apply", Apply);
            settingsItems[5] = new MISpace();
            settingsItems[6] = new MILink("Back", TitleMenu.GetSubMenu(MenuName.MAIN));

            (settingsItems[MenuItemIndices.RESOLUTION] as MIDial).Backward();
            (settingsItems[MenuItemIndices.RESOLUTION] as ISelectable).Muted = true;
            (settingsItems[MenuItemIndices.FILTER] as ISelectable).Muted = true;

            MenuItem[] highScoreItems = new MenuItem[12];
            for (int i = 0; i < 10; i++)
                highScoreItems[i] = new MIHeadline(TitleMenu.GetString("highScore" + i).Value);
            highScoreItems[10] = new MISpace();
            highScoreItems[11] = new MILink("View Speed Runs", TitleMenu.GetSubMenu(MenuName.SPEED_RUN));

            MenuItem[] speedRunItems = new MenuItem[12];
            for (int i = 0; i < 10; i++)
                speedRunItems[i] = new MIHeadline(TitleMenu.GetString("speedRun" + i).Value);
            speedRunItems[10] = new MISpace();
            speedRunItems[11] = new MILink("Back to Main Menu", TitleMenu.GetSubMenu(MenuName.MAIN));

            MenuItem[] aboutItems = new MenuItem[4];
            aboutItems[0] = new MIHeadline("Music, visuals, and programming");
            aboutItems[1] = new MIHeadline("by Leonard Young.");
            aboutItems[2] = new MISpace();
            aboutItems[3] = new MILink("Back", TitleMenu.GetSubMenu(MenuName.MAIN));

            TitleMenu.GetSubMenu(MenuName.MAIN).SetItems(mainItems.ToList());
            TitleMenu.GetSubMenu(MenuName.HIGH_SCORE).SetItems(highScoreItems.ToList());
            TitleMenu.GetSubMenu(MenuName.SPEED_RUN).SetItems(speedRunItems.ToList());
            TitleMenu.GetSubMenu(MenuName.SETTINGS).SetItems(settingsItems.ToList());
            TitleMenu.GetSubMenu(MenuName.ABOUT).SetItems(aboutItems.ToList());
        }

        public static void ConstructWaypointMenu()
        {
            List<DataInt> flagNumbers = new List<DataInt>();
            int numEntries = 0;

            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu(MenuName.MAIN, wrapAround: true));

            WaypointMenu = new Menu(subMenus, dataIntegers: flagNumbers);

            List<MenuItem> mainItems = new List<MenuItem>();
            mainItems.Add(new MIHeadline("Where would you like to start from?"));
            mainItems.Add(new MISpace());
            mainItems.Add(new MICommand("House", BeginAtHouse));
            for (int i = 0; i < Game1.NUM_FLAGS; i++)
                if (Game1.flagStatus[i].raised)
                {
                    flagNumbers.Add(new DataInt((numEntries++).ToString(), i));
                    mainItems.Add(new MICommand("Flag #" + (i + 1), BeginAtWaypoint));
                }

            WaypointMenu.GetSubMenu(MenuName.MAIN).SetItems(mainItems);
        }

        public static void ConstructPauseMenu()
        {
            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu(MenuName.MAIN, wrapAround: true));

            PauseMenu = new Menu(subMenus);

            List <MenuItem> mainItems = new List<MenuItem>();
            mainItems.Add(new MIHeadline("  P A U S E D  "));
            mainItems.Add(new MISpace());
            mainItems.Add(new MICommand("Resume", Resume));
            mainItems.Add(new MICommand("Quit to Main Menu", QuitToMainMenu));
            mainItems.Add(new MICommand("Quit Program", Quit));

            PauseMenu.GetSubMenu(MenuName.MAIN).SetItems(mainItems);
        }

        public static void ConstructTextFields()
        {
            TheTextfields = new Textfield[NUM_FIELDS];
            for (int i = 0; i < TheTextfields.Length; i++)
                TheTextfields[i] = new Textfield(Game1.characterSet, "", 0, 0, 30 * Game1.TILE_SIZE);
        }

        //Delegate Methods

        public static void Start(Data[] data)
        {
            TheTextfields[0].ChangeText(Messages.LOAD);
            TheTextfields[0].Draw();
            Game1.gameMode = Game1.GameModes.Load;
        }
        public static void Apply(Data[] data)
        {
            int selected = TitleMenu.GetInt("resolution").Value;
            int[] filterCompatibleMultipliers = { 2, 4, 8, 16 };

            ISelectable toggleResolution = TitleMenu.GetSubMenu(MenuName.SETTINGS).GetAtIndex(MenuItemIndices.RESOLUTION) as ISelectable;
            ISelectable toggleFilter = TitleMenu.GetSubMenu(MenuName.SETTINGS).GetAtIndex(MenuItemIndices.FILTER) as ISelectable;

            if (TitleMenu.GetBool("fullscreenOn").Value == false)
            {
                //Note: the following requests to change the window size may not succeed.
                Windows.Foundation.Size size = new Windows.Foundation.Size(
                    (selected + 1) * Game1.FULLFIELD_SIZE_X,
                    (selected + 1) * Game1.FULLFIELD_SIZE_Y);
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(size);
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(size);
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(
                    Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);

                Game1.canvasMultiplier = selected + 1;
                Game1.graphics.IsFullScreen = false;

                toggleResolution.Muted = true;
                toggleFilter.Muted = true;
            }
            else
            {
                Point currentResolution = Game1.GetCurrentResolution();
                int selectedWidth = (selected + 1) * Game1.FULLFIELD_SIZE_X;
                int selectedHeight = (selected + 1) * Game1.FULLFIELD_SIZE_Y;

                if (currentResolution.X >= selectedWidth &&
                    currentResolution.Y >= selectedHeight &&
                    (double)currentResolution.Y / currentResolution.X < 0.75)
                {
                    Game1.canvasMultiplier = selected + 1;
                    Game1.graphics.IsFullScreen = true;
                }

                toggleResolution.Muted = false;
                toggleFilter.Muted = true;

                foreach (int i in filterCompatibleMultipliers)
                    if (Game1.canvasMultiplier == i)
                        toggleFilter.Muted = false;            
            }

            Game1.graphics.ApplyChanges();

            if (toggleFilter.Muted)
                Game1.renderEffect = Game1.RenderEffects.None;
            else Game1.renderEffect = (Game1.RenderEffects)TitleMenu.GetInt("filter").Value;

            MediaPlayer.Volume = (float)TitleMenu.GetDoubleVal("soundVolume");
        }
        public static void Quit(Data[] data) { Game1.quit = true; }
        public static void BeginAtHouse(Data[] data)
        {
            Game1.curtainTimer = Game1.CURTAIN_PULL_DURATION;
            Game1.worldCursor = new Point(Game1.BEGIN_ROOM_X, Game1.BEGIN_ROOM_Y);
            Game1.player.Reposition(Game1.BEGIN_POSITION_X, Game1.BEGIN_POSITION_Y);
            Game1.ChangeRoom();
            Game1.gameMode = Game1.GameModes.Action;
        }
        public static void BeginAtWaypoint(Data[] data)
        {
            int selection = WaypointMenu.GetSubMenu(MenuName.MAIN).SelectionIndex - 3;
            int flagNumber = WaypointMenu.GetInt(selection.ToString()).Value;
            Game1.curtainTimer = Game1.CURTAIN_PULL_DURATION;
            Game1.worldCursor = new Point(Game1.flagStatus[flagNumber].mapLocation.X, Game1.flagStatus[flagNumber].mapLocation.Y);
            Game1.player.Reposition(Game1.flagStatus[flagNumber].startLocation.X, Game1.flagStatus[flagNumber].startLocation.Y);
            Game1.ChangeRoom();
            Game1.gameMode = Game1.GameModes.Action;
        }
        public static void Resume(Data[] data) { Game1.gameMode = Game1.GameModes.Action; }
        public static void QuitToMainMenu(Data[] data)
        {
            Game1.gameMode = Game1.GameModes.Title;
            Game1.PlayMusic(Sounds.Music.TITLE);
        }
    }
}
