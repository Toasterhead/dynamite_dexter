using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public static class MenuManager
    {
        private const int NUM_FIELDS = 14;

        public static Textfield[] TheTextfields;
        public static Menu TitleMenu;
        public static Menu WaypointMenu;
        public static Menu PauseMenu;

        public static void ConstructTitleMenu()
        {
            //Obtain supported screen resolutions...

            List<Point> supportedResolutions = Game1.GetSupportedResolutions();
            Point[] gameResolutions = new Point[3];

            for (int i = 0; i < gameResolutions.Length; i++)
                gameResolutions[i] = new Point((i + 1) * Game1.fullfield.X, (i + 1) * Game1.fullfield.Y);

            List<string> resolutions = new List<string>();

            foreach (Point i in gameResolutions)
            {
                bool isPossible = false;

                foreach (Point j in supportedResolutions)
                
                    if (j.X > i.X && j.Y > i.X)
                    {
                        isPossible = true;
                        break;
                    }

                if (isPossible)
                    resolutions.Add(i.X + " x " + i.Y);
            }

            //Construct menus...

            List<DataBool> dataBooleans = new List<DataBool>();
            dataBooleans.Add(new DataBool("fullscreenOn", false));

            List<DataInt> dataIntegers = new List<DataInt>();
            dataIntegers.Add(new DataInt("resolution", 0));

            List<DataDouble> dataDoubles = new List<DataDouble>();
            dataDoubles.Add(new DataDouble("soundVolume", 0.3));

            List<DataString> dataStrings = new List<DataString>();
            for (int i = 0; i < 10; i++)
                dataStrings.Add(new DataString("highScore" + i, "[blank]"));
            for (int i = 0; i < 10; i++)
                dataStrings.Add(new DataString("speedRun" + i, "[blank]"));

            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu("Main", wrapAround: true));
            subMenus.Add(new SubMenu("High Score", wrapAround: true));
            subMenus.Add(new SubMenu("Speed Run", wrapAround: true));
            subMenus.Add(new SubMenu("Settings", wrapAround: true));
            subMenus.Add(new SubMenu("About", wrapAround: true));

            TitleMenu = new Menu(subMenus, dataBooleans, dataIntegers, dataDoubles, dataStrings);

            MenuItem[] mainItems = new MenuItem[5];
            mainItems[0] = new MICommand("Start", Start);
            mainItems[1] = new MILink("High Score", TitleMenu.GetSubMenu("High Score"));
            mainItems[2] = new MILink("Settings", TitleMenu.GetSubMenu("Settings"));
            mainItems[3] = new MILink("About", TitleMenu.GetSubMenu("About"));
            mainItems[4] = new MICommand("Quit", Quit);

            MenuItem[] settingsItems = new MenuItem[6];
            settingsItems[0] = new MIDial(TitleMenu.GetInt("resolution"), "Screen Resolution: ", resolutions.ToArray());
            settingsItems[1] = new MISwitch(TitleMenu.GetBool("fullscreenOn"), "Fullscreen: ", "Yes", "No");
            settingsItems[2] = new MISlider(TitleMenu.GetDouble("soundVolume"), "Sound Volume: ", 10, 1, reportAsRatio: false);
            settingsItems[3] = new MICommand("Apply", Apply);
            settingsItems[4] = new MISpace();
            settingsItems[5] = new MILink("Back", TitleMenu.GetSubMenu("Main"));

            MenuItem[] highScoreItems = new MenuItem[12];
            for (int i = 0; i < 10; i++)
                highScoreItems[i] = new MIHeadline(TitleMenu.GetString("highScore" + i).Value);
            highScoreItems[10] = new MISpace();
            highScoreItems[11] = new MILink("View Speed Runs", TitleMenu.GetSubMenu("Speed Run"));

            MenuItem[] speedRunItems = new MenuItem[12];
            for (int i = 0; i < 10; i++)
                speedRunItems[i] = new MIHeadline(TitleMenu.GetString("speedRun" + i).Value);
            speedRunItems[10] = new MISpace();
            speedRunItems[11] = new MILink("Back to Main Menu", TitleMenu.GetSubMenu("Main"));

            MenuItem[] aboutItems = new MenuItem[4];
            aboutItems[0] = new MIHeadline("Music, visuals, and programming");
            aboutItems[1] = new MIHeadline("by Leonard Young.");
            aboutItems[2] = new MISpace();
            aboutItems[3] = new MILink("Back", TitleMenu.GetSubMenu("Main"));

            TitleMenu.GetSubMenu("Main").SetItems(mainItems.ToList());
            TitleMenu.GetSubMenu("High Score").SetItems(highScoreItems.ToList());
            TitleMenu.GetSubMenu("Speed Run").SetItems(speedRunItems.ToList());
            TitleMenu.GetSubMenu("Settings").SetItems(settingsItems.ToList());
            TitleMenu.GetSubMenu("About").SetItems(aboutItems.ToList());
        }

        public static void ConstructWaypointMenu()
        {
            List<DataInt> flagNumbers = new List<DataInt>();
            int numEntries = 0;

            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu("Main", wrapAround: true));

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

            WaypointMenu.GetSubMenu("Main").SetItems(mainItems);
        }

        public static void ConstructPauseMenu()
        {
            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu("Main", wrapAround: true));

            PauseMenu = new Menu(subMenus);

            List <MenuItem> mainItems = new List<MenuItem>();
            mainItems.Add(new MIHeadline("  P A U S E D  "));
            mainItems.Add(new MISpace());
            mainItems.Add(new MICommand("Resume", Resume));
            mainItems.Add(new MICommand("Quit to Main Menu", QuitToMainMenu));
            mainItems.Add(new MICommand("Quit Program", Quit));

            PauseMenu.GetSubMenu("Main").SetItems(mainItems);
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

            if (TitleMenu.GetBool("fullscreenOn").Value == false)
            {
                Game1.graphics.PreferredBackBufferWidth = (selected + 1) * Game1.fullfield.X;
                Game1.graphics.PreferredBackBufferHeight = (selected + 1) * Game1.fullfield.Y;
                Game1.graphics.IsFullScreen = false;
                Game1.graphics.ApplyChanges();
            }
            else
            {
                List<Point> supportedResolutions = Game1.GetSupportedResolutions();

                foreach (Point i in supportedResolutions)
                {
                    if (i.X >= (selected + 1) * Game1.fullfield.X && i.Y >= (selected + 1) * Game1.fullfield.Y && (double)i.Y / i.X < 0.75)
                    {
                        Game1.graphics.PreferredBackBufferWidth = i.X;
                        Game1.graphics.PreferredBackBufferHeight = i.Y;
                        Game1.canvasMultiplier = selected + 1;
                        break;
                    }
                }

                Game1.graphics.IsFullScreen = true;
                Game1.graphics.ApplyChanges();
            }

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
            int selection = WaypointMenu.GetSubMenu("Main").SelectionIndex - 3;
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
