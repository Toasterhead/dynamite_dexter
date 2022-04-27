using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

/*
    After moving source files to Windows Universal Application (UWP) project, HighScore.cs and Map.cs have persistent errors related to
    the StreamReader object type and AppDomain. Investigate later.
*/

/*
    IMPORTANT

    If the project is to be deployed for public use, remember to enable the following security features under the project's settings:
    Signing     - Check "Sign the ClickOnce manifests".
    Security    - Check "Enable ClickOnce security settings".
*/


namespace DynamiteDexter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public partial class Game1 : Game
    {
        public enum GameModes { Title, Action, Death, Text, Illustration, Waypoint, GameOver, HighScore, Pause, Load, WinA, WinB }
        public enum Environments { Garden, Forest, Desert, Jungle, Cave, Mine, Catacomb, Cemetary, Jail, House, Castle, Temple, Facility, EnumSize }
        public enum Layers { Overhead = 0, Actor, Passable, Outline, Terrain, Shadow, Floor, EnumSize }
        public enum SyncStates
        {
            FirstQuarter = 0,
            SecondQuarter,
            ThirdQuarter,
            FourthQuarter,
            FirstAndThirdQuarter,
            SecondAndFourthQuarter
        }

        public const int TILE_SIZE = 16;
        public const int SUB_TILE_SIZE = TILE_SIZE / 2;
        public const int GRID_SIZE_X = 15;
        public const int GRID_SIZE_Y = 15;
        public const int WORLD_SIZE_X = 20;
        public const int WORLD_SIZE_Y = 20;
        public const int BEGIN_ROOM_X = 10;
        public const int BEGIN_ROOM_Y = 10;
        public const int BEGIN_POSITION_X = 5 * TILE_SIZE;
        public const int BEGIN_POSITION_Y = 5 * TILE_SIZE;
        public const int SYNC_QUARTER = 15;
        public const int NUM_FLAGS = 8;
        public const int SCRIPT_INTERVAL = 10;
        public const int SCREEN_FLASH_TIME = 15;
        public const int LIGHT_UP_TIME = 20;
        public const int CURTAIN_PULL_DURATION = 20;
        public const double ACTION_CANVAS_RATIO = 0.75;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static KeyboardState keys;
        public static KeyboardState keysPrev;
        public static GamePadState gamepad;
        public static GamePadState gamepadPrev;

        public static GraphicsDevice theGraphicsDevice;//

        public static Player player;
        public static IGameObject causeOfDeath;

        public static Point playfield;
        public static Point hudfield;
        public static Point fullfield;
        public static CharacterSet characterSet;
        public static Textfield textLives;
        public static Textfield textDynamite;
        public static Textfield textKeys;
        public static Textfield textDollars;
        public static Textfield textMessage;
        public static Textfield textPlaque;
        public static FlagStatus[] flagStatus;
        public static List<IGameObject> spriteSet;
        public static List<IGameObject> pendingSet;
        public static List<IGameObject> removalSet;
        public static Sprite playerHouse;
        public static Room[,] worldSet;
        public static List<Terrain>[,] alteredSet;
        public static bool[,] alteredStates;
        public static List<Gate> gates;
        public static Point worldCursor;
        public static Point mapLoadIndex;
        public static Graph graph;

        public static Random rand;
        public static bool quit;
        public static bool winBig;
        public static int universalTimeStamp;
        public static int universalTimer;
        public static uint inGameTimer;
        public static uint screenFlashTimer;
        public static uint lightUpTimer;
        public static uint curtainTimer;
        public static uint? winCount;
        public static GameModes gameMode;

        public static RenderTarget2D canvasAction;
        public static RenderTarget2D canvasHud;
        public static RenderTarget2D canvasFull;
        public static Texture2D illustration;
        public static int canvasMultiplier; //Fullscreen mode only;

        public static Song currentSong;

        public static SoundEffectInstance currentSound;
        public static SoundEffectInstance queuedSound;

        public static Effect DARKEN;
        public static Effect INVERT;
        public static Effect PHASE_SHIFT;
        public static Effect SCANLINE;

        public static string contentDir;//

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Tile.TileSize = TILE_SIZE;

            playfield = new Point(GRID_SIZE_X * TILE_SIZE, GRID_SIZE_Y * TILE_SIZE); //(240, 240)
            hudfield = new Point(80, 240);
            fullfield = new Point(playfield.X + hudfield.X, playfield.Y); //(320, 240)

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f); // --- Adjust framerate.

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 2 * fullfield.X;
            graphics.PreferredBackBufferHeight = 2 * fullfield.Y;
            graphics.ApplyChanges();

            theGraphicsDevice = GraphicsDevice;

            canvasAction = new RenderTarget2D(
                GraphicsDevice,
                playfield.X,
                playfield.Y,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            canvasHud = new RenderTarget2D(
                GraphicsDevice,
                hudfield.X,
                hudfield.Y,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            canvasFull = new RenderTarget2D(
                GraphicsDevice,
                fullfield.X,
                fullfield.Y,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            rand = new Random();
            quit = false;

            characterSet = new CharacterSet(SUB_TILE_SIZE, SUB_TILE_SIZE);
            LoadCharacterSet(characterSet);

            FileInOut.Initialize();
            FileInOut.LoadFromFileAsync(FileInOut.GameDataFiles.Map);
            FileInOut.LoadFromFileAsync(FileInOut.GameDataFiles.BossLocation);
            FileInOut.LoadFromFileAsync(FileInOut.GameDataFiles.DenizenLocations);
            FileInOut.LoadFromFileAsync(FileInOut.GameDataFiles.EntryPoints);
            FileInOut.LoadFromFileAsync(FileInOut.GameDataFiles.HighScore);

            MenuManager.ConstructTitleMenu();
            MenuManager.ConstructPauseMenu();
            MenuManager.ConstructTextFields();

            //HighScore.LoadFromFileData(); //Now called in FileInOut.LoadFromFileAsync();
            //HighScore.WriteToMenu();

            //NewGame();

            gameMode = GameModes.Title;
            MediaPlayer.Volume = (float)MenuManager.TitleMenu.GetDoubleVal("soundVolume");
            MediaPlayer.Play(Sounds.Music.TITLE);
            mapLoadIndex = new Point(-1, -1);

            base.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (quit) Exit();

            switch (gameMode)
            {
                case GameModes.Action:
                    CheckActionInput();
                    UpdateActionState();
                    break;
                case GameModes.Title:
                    CheckTitleInput();
                    break;
                case GameModes.Text:
                    CheckTextInput();
                    universalTimer++;
                    break;
                case GameModes.Illustration:
                    CheckTextInput();
                    universalTimer++;
                    break;
                case GameModes.Death:
                    UpdateDeathState();
                    break;
                case GameModes.Waypoint:
                    CheckWaypointInput();
                    break;
                case GameModes.GameOver:
                    UpdateGameOverState();
                    break;
                case GameModes.HighScore:
                    CheckHighScoreInput();
                    UpdateHighScoreState();
                    break;
                case GameModes.Pause:
                    CheckPauseInput();
                    universalTimer++;
                    break;
                case GameModes.Load:
                    IterateLoader();
                    break;
                case GameModes.WinA:
                    CheckWinAInput();
                    UpdateWinAState();
                    break;
                case GameModes.WinB:
                    CheckWinBInput();
                    UpdateWinBState();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameMode)
            {
                case GameModes.Action:
                    DrawActionMode();
                    break;
                case GameModes.Title:
                    DrawTitleMode();
                    break;
                case GameModes.Text:
                    DrawTextMode();
                    break;
                case GameModes.Illustration:
                    DrawIllustrationMode();
                    break;
                case GameModes.Death:
                    DrawDeathMode();
                    break;
                case GameModes.Waypoint:
                    DrawWaypointMode();
                    break;
                case GameModes.GameOver:
                    DrawGameOverMode();
                    break;
                case GameModes.HighScore:
                    DrawHighscoreMode();
                    break;
                case GameModes.Pause:
                    DrawActionMode();
                    break;
                case GameModes.Load:
                    DrawLoadMode();
                    break;
                case GameModes.WinA:
                    DrawWinAMode();
                    break;
                case GameModes.WinB:
                    DrawWinBMode();
                    break;
                default: throw new Exception("Error - Unable to recognize game mode.");
            }

            base.Draw(gameTime);
        }
    }
}