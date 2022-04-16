using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public static void NewGame()
        {
            player = new Player();
            player.Reposition(BEGIN_POSITION_X, BEGIN_POSITION_Y);

            worldCursor = new Point(BEGIN_ROOM_X, BEGIN_ROOM_Y);

            curtainTimer = CURTAIN_PULL_DURATION;
            inGameTimer = 0;

            winCount = null;
            winBig = false;

            flagStatus = new FlagStatus[NUM_FLAGS];
            for (int i = 0; i < flagStatus.Length; i++)
                flagStatus[i] = new FlagStatus();

            Map.ResetFlagNumber();
            BossRoom.ResetNumDefeated();

            BossRoom.MarkLocation(Map.LoadBossLocations(FileInOut.bossLocationFileData));
            HouseRoom.MarkLocation(Map.LoadDenizenLocations(FileInOut.denizenLocationFileData));

            worldSet = new Room[WORLD_SIZE_X, WORLD_SIZE_Y];
            alteredSet = new List<Terrain>[WORLD_SIZE_X, WORLD_SIZE_Y];
            alteredStates = new bool[WORLD_SIZE_X, WORLD_SIZE_Y];pendingSet = new List<IGameObject>();
            removalSet = new List<IGameObject>();
            gates = new List<Gate>();

            for (int i = 0; i < WORLD_SIZE_X; i++)//
                for (int j = 0; j < WORLD_SIZE_Y; j++)//
                    worldSet[i, j] = new Room(new List<IGameObject>(), "", Room.Mode.Standard);//
            spriteSet = new List<IGameObject>();//

            string textMessageString = ""; //Default string to establish height of text field.
            for (int i = 0; i < 36 * 26; i++)
                textMessageString += ".";
            string textPlaqueString = "";
            for (int i = 0; i < 32 * 15; i++)
                textPlaqueString += ".";

            textLives = new Textfield(characterSet, player.Lives.ToString(), 0, 0, 5 * SUB_TILE_SIZE);
            textDynamite = new Textfield(characterSet, player.DynamiteSticks.ToString(), 0, 0, 5 * SUB_TILE_SIZE);
            textDollars = new Textfield(characterSet, player.Dollars.ToString(), 0, 0, 5 * SUB_TILE_SIZE);
            textKeys = new Textfield(characterSet, player.Keys.ToString(), 0, 0, 5 * SUB_TILE_SIZE);
            textMessage = new Textfield(characterSet, textMessageString, 0, 0, 36 * SUB_TILE_SIZE, Textfield.WriteMode.Auto, 2);
            textPlaque = new Textfield(characterSet, textPlaqueString, 0, 0, 32 * SUB_TILE_SIZE, Textfield.WriteMode.Immediate);

            currentSound = Sounds.COUNTDOWN; //To prevent null exceptions.
            queuedSound = null;
            MediaPlayer.Stop();
            MediaPlayer.Volume = (float)MenuManager.TitleMenu.GetDoubleVal("soundVolume");
            currentSound.Volume = (float)MenuManager.TitleMenu.GetDoubleVal("soundVolume");
            
            mapLoadIndex = new Point(0, 0);
        }
    }
}
