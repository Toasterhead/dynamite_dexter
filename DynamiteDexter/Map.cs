using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public static class Map
    {
        private static char markerCommand = '$';
        private static char markerItem = '*';

        private static Queue<Rectangle> boulderTripZone = new Queue<Rectangle>();
        private static Queue<int[]> boulderPath = new Queue<int[]>();
        private static Queue<Point> bladePath = new Queue<Point>();
        private static Queue<bool> bladeSpeed = new Queue<bool>();
        private static Queue<Rectangle> cloudTerritory = new Queue<Rectangle>();
        private static Queue<Game1.SyncStates> syncState = new Queue<Game1.SyncStates>();
        private static Game1.Environments environmentPrimary = Game1.Environments.Garden;
        private static Game1.Environments environmentSecondary = Game1.Environments.Garden;

        private static char[] delimiters = { '$', ':' };

        private static int flagNumber = 0;

        public static Room LoadMap(List<string> roomData)
        {
            //Room Data
            List<IGameObject> spriteSet = new List<IGameObject>();
            Stack<string> roomStack = new Stack<string>();
            string plaqueText = "";
            Room.Mode mode = Room.Mode.Standard;

            //Item Parameters
            boulderTripZone = new Queue<Rectangle>();
            boulderPath = new Queue<int[]>();
            bladePath = new Queue<Point>();
            bladeSpeed = new Queue<bool>();
            syncState = new Queue<Game1.SyncStates>();
            environmentPrimary = Game1.Environments.Garden;
            environmentSecondary = Game1.Environments.Garden;

            for (int i = roomData.Count - 1; i >= 0; i--)
                roomStack.Push(roomData[i]);

            string line;
            int mapRowCount = 0;
            bool end = false;

            while (!end)
            {
                line = roomStack.Pop();

                bool isValidLine = line != null && line != "";

                if (isValidLine && line[0] == markerCommand)
                {
                    string[] terms = line.Split(delimiters);

                    switch (terms[1])
                    {
                        case "house":
                            mode = Room.Mode.House;
                            break;
                        case "darkened":
                            mode = Room.Mode.Darkened;
                            break;
                        case "ominous":
                            mode = Room.Mode.Ominous;
                            break;
                        case "boss":
                            mode = Room.Mode.Boss;
                            break;
                        case "plaque_text":
                            if (terms[2] == "")
                            { plaqueText += "\n "; }
                            else plaqueText += terms[2] + "\n";
                            break;
                        case "skip":
                            mode = Room.Mode.Skip;
                            break;
                        case "environment_primary":
                            environmentPrimary = (Game1.Environments)Convert.ToInt32(terms[2]);
                            if (environmentPrimary == Game1.Environments.House)
                                mode = Room.Mode.House;
                            break;
                        case "environment_secondary":
                            environmentSecondary = (Game1.Environments)Convert.ToInt32(terms[2]);
                            break;
                        case "sync_state":
                            for (int i = 2; i < terms.Length; i++)
                                syncState.Enqueue((Game1.SyncStates)Convert.ToInt32(terms[i]));
                            break;
                        case "boulder":
                            Rectangle tripZoneEntry = new Rectangle(
                                Convert.ToInt32(terms[2]),
                                Convert.ToInt32(terms[3]),
                                Convert.ToInt32(terms[4]),
                                Convert.ToInt32(terms[5]));
                            int[] boulderPathEntry = new int[terms.Length - 6];
                            for (int i = 0; i < terms.Length - 6; i++)
                                boulderPathEntry[i] = Convert.ToInt32(terms[i + 6]);
                            boulderTripZone.Enqueue(tripZoneEntry);
                            boulderPath.Enqueue(boulderPathEntry);
                            break;
                        case "blade":
                            Point bladePathEntry = new Point(
                                Convert.ToInt32(terms[2]),
                                Convert.ToInt32(terms[3]));
                            bool bladeSpeedEntry = Convert.ToBoolean(Convert.ToInt32(terms[4]));
                            bladePath.Enqueue(bladePathEntry);
                            bladeSpeed.Enqueue(bladeSpeedEntry);
                            break;
                        case "cloud":
                            Rectangle cloudTerritoryEntry = new Rectangle(
                                Convert.ToInt32(terms[2]),
                                Convert.ToInt32(terms[3]),
                                Convert.ToInt32(terms[4]),
                                Convert.ToInt32(terms[5]));
                            cloudTerritory.Enqueue(cloudTerritoryEntry);
                            break;
                        case "room": //Ignore
                            break;
                        case "altered": //Ignore
                            break;
                        case "end":
                            end = true;
                            break;
                        default: throw new Exception("Error - Unable to recognize map command '" + terms[1] + "'.");
                    }
                }
                else if (isValidLine && line[0] == markerItem && mapRowCount < Game1.GRID_SIZE_Y)
                {
                    for (int i = 1; i < line.Length && i <= Game1.GRID_SIZE_X; i++)
                    {
                        IGameObject item = LoadItem(
                            mode,
                            line[i],
                            new Point(i - 1, mapRowCount),
                            spriteSet);

                        if (item != null)
                            spriteSet.Add(item);
                        if (item is IOutlined)
                            spriteSet.Add((item as IOutlined).TheOutlineMask);
                        if (item is IHasAttachments)
                            spriteSet.AddRange((item as IHasAttachments).Attachments);
                    }

                    mapRowCount++;
                }
            }

            foreach (IGameObject i in spriteSet)

                if (i is INavigates)
                    (i as INavigates).AcquireSet(spriteSet);
                else if (i is ISeeks)
                    (i as ISeeks).AcquireGraph(
                        Game1.GenerateGraph(
                            spriteSet,
                            i is Snake ? (i as Snake).Segment[0] : null));

            try
            {
                AssociateSignPost(spriteSet);
                WaterTileReplacement(spriteSet);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
            }

            return new Room(spriteSet, plaqueText, mode);
        }

        public static List<Terrain> LoadAltered(List<string> roomData)
        {
            List<Terrain> alteredSet = new List<Terrain>();
            List<IGameObject> emptySet = new List<IGameObject>();
            Stack<string> roomStack = new Stack<string>();
            Room.Mode mode = Room.Mode.Standard;
            environmentPrimary = Game1.Environments.Garden;
            environmentSecondary = Game1.Environments.Garden;
            bool alteredOn = false;

            for (int i = roomData.Count - 1; i >= 0; i--)
                roomStack.Push(roomData[i]);

            string line;
            int mapRowCount = 0;
            bool end = false;

            while (!end)
            {
                line = roomStack.Pop();

                bool isValidLine = line != null && line != "";

                if (isValidLine && line[0] == markerCommand)
                {
                    string[] terms = line.Split(delimiters);

                    switch (terms[1])
                    {
                        case "house": mode = Room.Mode.House;
                            break;
                        case "darkened": mode = Room.Mode.Darkened;
                            break;
                        case "ominous": mode = Room.Mode.Ominous;
                            break;
                        case "boss": mode = Room.Mode.Boss;
                            break;
                        case "skip": mode = Room.Mode.Skip;
                            break;
                        case "environment_primary":
                            environmentPrimary = (Game1.Environments)Convert.ToInt32(terms[2]);
                            if (environmentPrimary == Game1.Environments.House)
                                mode = Room.Mode.House;
                            break;
                        case "environment_secondary":
                            environmentSecondary = (Game1.Environments)Convert.ToInt32(terms[2]);
                            break;
                        case "room": //Ignore
                            break;
                        case "altered": alteredOn = true;
                            break;
                        case "end": end = true;
                            break;
                    }
                }
                else if (alteredOn && isValidLine && line[0] == markerItem && mapRowCount < Game1.GRID_SIZE_Y)
                {
                    for (int i = 1; i < line.Length && i <= Game1.GRID_SIZE_X; i++)
                    {
                        IGameObject item = LoadItem(
                            mode,
                            line[i],
                            new Point(i - 1, mapRowCount),
                            emptySet);

                        if (item != null && item is Terrain)
                            alteredSet.Add(item as Terrain);
                    }

                    mapRowCount++;
                }
            }

            try
            {
                //Justify the altered set as a collection of general game objects to use as a method argument.

                List<IGameObject> alteredGameObject = new List<IGameObject>();

                foreach (Terrain i in alteredSet)
                    alteredGameObject.Add(i as IGameObject);

                AssociateSignPost(alteredGameObject);
                WaterTileReplacement(alteredGameObject);

                alteredSet = new List<Terrain>();

                foreach (IGameObject i in alteredGameObject)
                    if (i is Terrain) alteredSet.Add(i as Terrain);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
            }

            return alteredSet;
        }

        public static BossRoom.BossTypes[,] LoadBossLocations(List<string> bossLocationFileData)
        {
            BossRoom.BossTypes[,] bossLocation = new BossRoom.BossTypes[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];

            foreach (string line in bossLocationFileData)
            {
                string[] terms = line.Split(delimiters);
                bossLocation[
                    Convert.ToInt32(terms[1]), 
                    Convert.ToInt32(terms[2])] = (BossRoom.BossTypes)Convert.ToInt32(terms[0]);
            }

            return bossLocation;
        }

        public static HouseRoom.DenizenTypes[,] LoadDenizenLocations(List<string> denizenLocationFileData)
        {
            HouseRoom.DenizenTypes[,] denizenLocation = new HouseRoom.DenizenTypes[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];

            foreach (string line in denizenLocationFileData)
            {
                string[] terms = line.Split(delimiters);
                denizenLocation[
                    Convert.ToInt32(terms[1]), 
                    Convert.ToInt32(terms[2])] = (HouseRoom.DenizenTypes)Convert.ToInt32(terms[0]);
            }

            return denizenLocation;
        }

        public static void ResetFlagNumber() { flagNumber = 0; }

        private static IGameObject LoadItem(
            Room.Mode mode,
            char key, 
            Point position,
            List<IGameObject> spriteSet)
        {
            switch (key)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': return LoadEnvironmental(environmentPrimary, Convert.ToInt32(key.ToString()), position);
                case ' ': return null;
                case '~': return new Water(Images.WATER_1, position.X, position.Y);
                case '\'': return new Water(Images.WATER_2, position.X, position.Y, 2);
                case '"': return new Water(Images.WATER_3, position.X, position.Y, 2);
                case '?': return new Breakable(Images.BARRICADE, position.X, position.Y);
                case '>': return new Alligator(position.X, position.Y);
                case '<': return new Alligator(position.X, position.Y, true);
                case '{': return new Tile(Images.BRIDGE_LEFT, position.X, position.Y, (int)Game1.Layers.Floor);
                case '}': return new Tile(Images.BRIDGE_RIGHT, position.X, position.Y, (int)Game1.Layers.Floor);
                case '[': return new Gate(position.X, position.Y, true, Game1.player);
                case ']': return new Gate(position.X, position.Y, false, Game1.player);
                case 'P': return new Casket(position.X, position.Y);
                case 'Q': return new Casket(position.X, position.Y, false);
                case '=': return new Tile(Images.STAIRS, position.X, position.Y, (int)Game1.Layers.Floor);
                case '^': return new Spikes(position.X, position.Y);
                case 'F': return new Flag(position.X, position.Y - 1, flagNumber++);
                case '|': return new Terrain(Images.SIGN_POST, position.X, position.Y);
                case '!': return new Terrain(Game1.IsOutdoor(environmentPrimary) ? Images.SIGN_OUTDOOR_DANGER : Images.SIGN_INDOOR_DANGER, position.X, position.Y);
                case 'N': return new Terrain(Game1.IsOutdoor(environmentPrimary) ? Images.SIGN_OUTDOOR_UP : Images.SIGN_INDOOR_UP, position.X, position.Y);
                case 'E': return new Terrain(Game1.IsOutdoor(environmentPrimary) ? Images.SIGN_OUTDOOR_RIGHT : Images.SIGN_INDOOR_RIGHT, position.X, position.Y);
                case 'S': return new Terrain(Game1.IsOutdoor(environmentPrimary) ? Images.SIGN_OUTDOOR_DOWN : Images.SIGN_INDOOR_DOWN, position.X, position.Y);
                case 'W': return new Terrain(Game1.IsOutdoor(environmentPrimary) ? Images.SIGN_OUTDOOR_LEFT : Images.SIGN_INDOOR_LEFT, position.X, position.Y);
                case 'K': return new Key(position.X, position.Y);
                case 'O': return new Orb(position.X, position.Y);
                case 'T': return new Terrain(Images.PILLAR, position.X, position.Y);
                case 'L': return new Terrain(Images.BLOCK, position.X, position.Y);
                case 'C': return new Breakable(Images.BRITTLE, position.X, position.Y);
                case 'B': return new Barrel(position.X, position.Y);
                case 'D': return new Drum(position.X, position.Y);
                case '*': return new Coins(position.X, position.Y);
                case '$': return new MoneyBag(position.X, position.Y);
                case 'V': return new Diamond(position.X, position.Y);
                case '+': return new Chalice(position.X, position.Y);
                case 'J': return new ExtraLife(position.X, position.Y);
                case '-': return new DynamiteSingleStick(position.X, position.Y);
                case ';': return new DynamitePack(position.X, position.Y);
                case '%': return new WorldMap(position.X, position.Y);
                case '&':
                    if (mode == Room.Mode.Ominous) //Move to separate method?
                        return new Plaque(Images.PLAQUE, position.X, position.Y);
                    else if (mode == Room.Mode.House)
                        return new Plaque(Images.DENIZEN, position.X, position.Y);
                    else if (Game1.IsOutdoor(environmentPrimary))
                        return new Plaque(Images.SIGN_OUTDOOR_TEXT, position.X, position.Y);
                    else return new Plaque(Images.SIGN_INDOOR_TEXT, position.X, position.Y);
                case '_': return mode == Room.Mode.Ominous ?
                        (IGameObject)new Sprite(
                            Images.LAIR_ENTRANCE, 
                            position.X * Game1.TILE_SIZE - Game1.TILE_SIZE, 
                            position.Y * Game1.TILE_SIZE, 
                            (int)Game1.Layers.Overhead) :
                        LoadDoor(environmentPrimary, position);                        
                case 'H': return LoadDoor(environmentPrimary, position, spriteSet);
                case '.': return LoadFloor(environmentPrimary, position);
                case ':': return LoadFloor(environmentPrimary, position, secondary: true);
                case '`': return LoadFloor(environmentSecondary, position);
                case '#': return LoadWall(environmentPrimary, position);
                case '@': return LoadWall(environmentSecondary, position);
                case '(': return LoadWall(environmentPrimary, position, passable: true);
                case ')': return LoadWall(environmentSecondary, position, passable: true);
                case '/': return LoadWall(environmentPrimary, position, breakable: true);
                case '\\': return LoadWall(environmentSecondary, position, breakable: true);
                case 'a': return new Beetle(position.X, position.Y);
                case 'b': return new Spider(position.X, position.Y);
                case 'c': return new Scorpion(position.X, position.Y, Game1.player);
                case 'd': return new Crab(position.X, position.Y, Game1.player);
                case 'e': return new Porcupine(position.X, position.Y);
                case 'f': return new Zombie(position.X, position.Y, Game1.player);
                case 'g': return new DrillMobile(position.X, position.Y);
                case 'h': return new Robot(position.X, position.Y, Game1.player);
                case 'i': return new FloatingHead(position.X, position.Y);
                case 'j': return new FloatingHead(position.X, position.Y, true);
                case 'k': return new Hypnoball(position.X, position.Y);
                case 'l': return new LiveTorch(position.X, position.Y);
                case 'm': return new GhostCasket(position.X, position.Y);
                case 'n': return new PillBox(position.X, position.Y);
                case 'o': return new Blade(position.X, position.Y, RetrieveFromQueue<Point>(bladePath), RetrieveFromQueue<bool>(bladeSpeed));
                case 'p': return new Cloud(position.X, position.Y, Game1.player, RetrieveFromQueue<Rectangle>(cloudTerritory));
                case 'q': return new RocketLauncher(position.X, position.Y, true, RetrieveSyncState(syncState));
                case 'r': return new RocketLauncher(position.X, position.Y, false, RetrieveSyncState(syncState));
                case 's': return new Cannon(position.X, position.Y, true, RetrieveSyncState(syncState));
                case 't': return new Cannon(position.X, position.Y, false, RetrieveSyncState(syncState));
                case 'u': return new ArrowLauncher(position.X, position.Y, true, RetrieveSyncState(syncState));
                case 'v': return new ArrowLauncher(position.X, position.Y, false, RetrieveSyncState(syncState));
                case 'w': return new ImpalerOutlet(position.X, position.Y, true, RetrieveSyncState(syncState));
                case 'x': return new ImpalerOutlet(position.X, position.Y, false, RetrieveSyncState(syncState));
                case 'y': return new Laser(position.X, position.Y, true, RetrieveSyncState(syncState) == Game1.SyncStates.FirstQuarter ? true : false);
                case 'z': return new Laser(position.X, position.Y, false, RetrieveSyncState(syncState) == Game1.SyncStates.FirstQuarter ? true : false);
                case 'A': return new BoulderOutlet(position.X, position.Y, RetrieveFromQueue<Rectangle>(boulderTripZone), RetrieveFromQueue<int[]>(boulderPath), Game1.player);
                case 'G': return new Tank(position.X, position.Y, Game1.player);
                case 'M': return new DirtDevil(position.X, position.Y);
                case 'R': return new Raccoon(position.X, position.Y, Game1.player, new Passable(Images.BUSH, position.X, position.Y));
                case 'U': return new SmallSnake(position.X, position.Y, Game1.player);
                case 'X': return new Imp(Game1.player);
                case 'Z': return new Terrain(Images.BLANK, position.X, position.Y);
            }

            return null;
        }

        private static IGameObject LoadEnvironmental(Game1.Environments environment, int key, Point position)
        {
            Texture2D image = Images.BLOCK;

            switch (environment)
            {
                case Game1.Environments.Garden:
                    if (key == 0) return new Breakable(Images.FENCE, position.X, position.Y);
                    else if (key == 1) return new BreakablePrize(Images.FLOWER, position.X, position.Y);
                    else if (key == 2) return new BreakablePrize(Images.PLANT, position.X, position.Y);
                    else if (key == 3) return new BreakablePrize(Images.ROCK, position.X, position.Y);
                    else if (key == 4) return new Breakable(Images.SCARECROW_TOP, position.X, position.Y);
                    else if (key == 5) return new Breakable(Images.SCARECROW_BOTTOM, position.X, position.Y);
                    else if (key == 6) return new BreakablePrize(Images.MUSHROOM, position.X, position.Y);
                    else if (key == 7) return new Plaque(Images.MAILBOX, position.X, position.Y);
                    break;
                case Game1.Environments.Forest:
                    if (key == 0) image = Images.MUSHROOM_LARGE_TOP_LEFT;
                    else if (key == 1) image = Images.MUSHROOM_LARGE_TOP_RIGHT;
                    else if (key == 2) image = Images.MUSHROOM_LARGE_BOTTOM_LEFT;
                    else if (key == 3) image = Images.MUSHROOM_LARGE_BOTTOM_RIGHT;
                    else if (key == 4) return new BreakablePrize(Images.MUSHROOM, position.X, position.Y);
                    else if (key == 5) return new BreakablePrize(Images.PLANT, position.X, position.Y);
                    else if (key == 6) image = Images.PINE_TREE_TOP;
                    else if (key == 7) image = Images.PINE_TREE_BOTTOM;
                    else if (key == 8) return new BreakablePrize(Images.ROCK, position.X, position.Y);
                    else if (key == 9) return new BreakablePrize(Images.BUSH, position.X, position.Y);
                    break;
                case Game1.Environments.Desert:
                    if (key == 0) image = Images.CACTUS_TOP;
                    else if (key == 1) image = Images.CACTUS_BOTTOM;
                    else if (key == 2) return new BreakablePrize(Images.ROCK, position.X, position.Y);
                    else if (key == 3) return new BreakablePrize(Images.BUSH, position.X, position.Y);
                    else if (key == 4) return new BreakablePrize(Images.BOVINE_SKULL, position.X, position.Y);
                    else if (key == 5) return new Breakable(Images.BONE, position.X, position.Y);
                    else if (key == 6) return new Breakable(Images.CHAIN_LINK, position.X, position.Y);
                    else if (key == 7) return new BreakablePrize(Images.PRICKLY_PEAR, position.X, position.Y);
                    break;
                case Game1.Environments.Jungle:
                    if (key == 0) return new Breakable(Images.VINE_TOP_LEFT, position.X, position.Y);
                    else if (key == 1) return new Breakable(Images.VINE_TOP_RIGHT, position.X, position.Y);
                    else if (key == 2) return new Breakable(Images.VINE_BOTTOM_LEFT, position.X, position.Y);
                    else if (key == 3) return new Breakable(Images.VINE_BOTTOM_RIGHT, position.X, position.Y);
                    else if (key == 4) return new Breakable(Images.VINE_HORIZONTAL, position.X, position.Y);
                    else if (key == 5) return new Breakable(Images.VINE_VERTICAL, position.X, position.Y);
                    else if (key == 6) image = Images.PALM_TREE_TOP;
                    else if (key == 7) image = Images.PALM_TREE_BOTTOM;
                    else if (key == 8) return new BreakablePrize(Images.PLANT, position.X, position.Y);
                    else if (key == 9) return new BreakablePrize(Images.ROCK, position.X, position.Y);
                    break;
                case Game1.Environments.Cave:
                    if (key == 0) image = Images.FOSSIL;
                    else if (key == 1) return new BreakablePrize(Images.MUSHROOM, position.X, position.Y);
                    else if (key == 2) return new Tile(Images.SPIDER_WEB_1, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 3) return new Tile(Images.SPIDER_WEB_2, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 4) return new Tile(Images.SPIDER_WEB_3, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 5) image = Images.EMBEDDED_CRYSTAL_1;
                    else if (key == 6) image = Images.EMBEDDED_CRYSTAL_2;
                    break;
                case Game1.Environments.Mine:
                    if (key == 0) image = Images.EMBEDDED_CRYSTAL_1;
                    else if (key == 1) image = Images.EMBEDDED_CRYSTAL_2;
                    else if (key == 2) image = Images.STEEL_BEAM_TOP_LEFT;
                    else if (key == 3) image = Images.STEEL_BEAM_TOP_RIGHT;
                    else if (key == 4) image = Images.STEEL_BEAM_BOTTOM_LEFT;
                    else if (key == 5) image = Images.STEEL_BEAM_BOTTOM_RIGHT;
                    else if (key == 6) image = Images.STEEL_BEAM_LEFT;
                    else if (key == 7) image = Images.STEEL_BEAM_RIGHT;
                    else if (key == 8) image = Images.STEEL_BEAM_TOP;
                    else if (key == 9) image = Images.STEEL_BEAM_BOTTOM;
                    break;
                case Game1.Environments.Catacomb:
                    if (key == 0) return new Eyeball(position.X, position.Y, Game1.player);
                    else if (key == 1) return new Eyeball(position.X, position.Y, Game1.player, true);
                    else if (key == 2) return new Eyeball(position.X, position.Y, Game1.player, false, 2);
                    else if (key == 3) return new Eyeball(position.X, position.Y, Game1.player, false, 0);
                    else if (key == 4) return new Eyeball(position.X, position.Y, Game1.player, false, 6);
                    else if (key == 5) return new Eyeball(position.X, position.Y, Game1.player, false, 4);
                    else if (key == 6) return new Terrain(Images.CANDLE_TOP, position.X, position.Y, 2, 4);
                    else if (key == 7) image = Images.CANDLE_BOTTOM;
                    else if (key == 8) image = Images.WALL_STONE;
                    else if (key == 9) return new BreakablePrize(Images.SKULL, position.X, position.Y);
                    break;
                case Game1.Environments.Cemetary:
                    if (key == 0) image = Images.TOMBSTONE;
                    else if (key == 1) image = Images.WALL_CEMETARY;
                    else if (key == 2) image = Images.GARGOYLE_LEFT;
                    else if (key == 3) image = Images.GARGOYLE_RIGHT;
                    else if (key == 4) return new Terrain(Images.LANTERN, position.X, position.Y, 2, 4);
                    else if (key == 5) return new Tile(Images.GRASS_1, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 6) return new BreakablePrize(Images.ROCK, position.X, position.Y);
                    else if (key == 7) image = Images.FENCE_CEMETARY;
                    else if (key == 8) image = Images.DEAD_TREE_TOP;
                    else if (key == 9) image = Images.DEAD_TREE_BOTTOM;
                    break;
                case Game1.Environments.Jail:
                    if (key == 0) return new Breakable(Images.BONE, position.X, position.Y);
                    else if (key == 1) return new BreakablePrize(Images.SKULL, position.X, position.Y);
                    else if (key == 2) return new Tile(Images.WINDOW_JAIL_LIGHT_TOP, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 3) return new Tile(Images.WINDOW_JAIL_LIGHT_BOTTOM, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 4) image = Images.WINDOW_JAIL;
                    else if (key == 5) return new Terrain(Images.TORCH, position.X, position.Y, 2, 4);
                    else if (key == 6) image = Images.ELECTRIC_CHAIR_TOP;
                    else if (key == 7) image = Images.ELECTRIC_CHAIR_BOTTOM;
                    else if (key == 8) image = Images.JAIL_BARS;
                    else if (key == 9) return new Tile(Images.SPIDER_WEB_1, position.X, position.Y, (int)Game1.Layers.Floor);
                    break;
                case Game1.Environments.House:
                    if (key == 0) return new Terrain(Images.WALL_PLAIN, position.X, position.Y);
                    else if (key == 1) return new Breakable(Images.VASE, position.X, position.Y);
                    else if (key == 2) return new Breakable(Images.TABLE, position.X, position.Y);
                    else if (key == 3) return new Breakable(Images.BOOK_SHELF_TOP, position.X, position.Y);
                    else if (key == 4) return new Breakable(Images.BOOK_SHELF_BOTTOM, position.X, position.Y);
                    else if (key == 5) return new Terrain(Images.FIREPLACE, position.X, position.Y, 2, 4);
                    else if (key == 6) image = Images.WINDOW_HOUSE;
                    else if (key == 7) return new Tile(Images.WINDOW_HOUSE_LIGHT_TOP, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 8) return new Tile(Images.WINDOW_HOUSE_LIGHT_BOTTOM, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 9) image = Images.WALL_BRICK;
                    break;
                case Game1.Environments.Castle:
                    if (key == 0) image = Images.WINDOW_CASTLE;
                    else if (key == 1) return new Tile(Images.WINDOW_CASTLE_LIGHT_TOP, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 2) return new Tile(Images.WINDOW_CASTLE_LIGHT_BOTTOM, position.X, position.Y, (int)Game1.Layers.Floor);
                    else if (key == 3) return new Terrain(Images.CANDLE_TOP, position.X, position.Y, 2, 4);
                    else if (key == 4) image = Images.CANDLE_BOTTOM;
                    else if (key == 5) return new Terrain(Images.TORCH, position.X, position.Y, 2, 4);
                    else if (key == 6) image = Images.KNIGHT_ARMOR_TOP;
                    else if (key == 7) image = Images.KNIGHT_ARMOR_BOTTOM;
                    else if (key == 8) return new Tile(Images.CARPET_MIDDLE, position.X, position.Y, (int)Game1.Layers.Floor);
                    break;
                case Game1.Environments.Temple:
                    if (key == 0) image = Images.STATUE_TOP;
                    else if (key == 1) image = Images.STATUE_BOTTOM;
                    else if (key == 2) image = Images.SCULPTURE_FACING_RIGHT_TOP;
                    else if (key == 3) image = Images.SCULPTURE_FACING_RIGHT_BOTTOM;
                    else if (key == 4) image = Images.SCULPTURE_FACING_LEFT_TOP;
                    else if (key == 5) image = Images.SCULPTURE_FACING_LEFT_BOTTOM;
                    else if (key == 6) image = Images.TEMPLE_PATTERN;
                    else if (key == 7) image = Images.SANDBLOCK;
                    else if (key == 8) return new BreakablePrize(Images.JUG, position.X, position.Y);
                    break;
                case Game1.Environments.Facility:
                    if (key == 0) return new Terrain(Images.VESSEL, position.X, position.Y, 4, 4);
                    else if (key == 1) image = Images.LAB_1;
                    else if (key == 2) image = Images.LAB_2;
                    else if (key == 3) image = Images.LAB_3_LEFT;
                    else if (key == 4) image = Images.LAB_3_RIGHT;
                    else if (key == 5) image = Images.METAL_PLATE;
                    else if (key == 6) image = Images.TUBES;
                    else if (key == 7) image = Images.TUBE_HUB;
                    else if (key == 8) image = Images.WIRES;
                    else if (key == 9) return new Terrain(Images.MONITOR, position.X, position.Y, 8, 2);
                    break;
            }

            return new Terrain(image, position.X, position.Y);
        }

        private static Terrain LoadWall(Game1.Environments environment, Point position, bool passable = false, bool breakable = false)
        {
            Texture2D image = Images.BLOCK;

            switch (environment)
            {
                case Game1.Environments.Garden:
                case Game1.Environments.Forest:
                case Game1.Environments.Desert:
                case Game1.Environments.Jungle: image = breakable ? Images.WALL_ROCK_CRACKED : Images.WALL_ROCK;
                    break;
                case Game1.Environments.Cave: image = breakable ? Images.WALL_PEBBLE_CRACKED : Images.WALL_PEBBLE;
                    break;
                case Game1.Environments.Castle: image = breakable ? Images.WALL_BRICK_LARGE_CRACKED : Images.WALL_BRICK_LARGE;
                    break;
                case Game1.Environments.Temple: image = breakable ? Images.WALL_TEMPLE_CRACKED : Images.WALL_TEMPLE;
                    break;
                case Game1.Environments.Facility: image = breakable ? Images.WALL_TILE_CRACKED : Images.WALL_TILE;
                    break;
                case Game1.Environments.Mine: image = breakable? Images.WALL_PEBBLE_CRACKED : Images.WALL_PEBBLE;
                    break;
                case Game1.Environments.Catacomb: image = breakable ? Images.WALL_GRANITE_CRACKED : Images.WALL_GRANITE;
                    break;
                case Game1.Environments.Cemetary: image = breakable ? Images.WALL_BRICK_CRACKED : Images.WALL_BRICK;
                    break;
                case Game1.Environments.Jail: image = breakable ? Images.WALL_BRICK_CRACKED : Images.WALL_BRICK;
                    break;
                case Game1.Environments.House: image = Images.WALL_HOUSE;
                    break;
            }

            if (breakable)
                return new Breakable(image, position.X, position.Y);

            if (passable)
                return new Passable(image, position.X, position.Y);

            return new Terrain(image, position.X, position.Y);
        }

        private static Tile LoadFloor(Game1.Environments environment, Point position, bool secondary = false)
        {
            Texture2D image = Images.DIRT_1;

            switch (environment)
            {
                case Game1.Environments.Garden:
                case Game1.Environments.Forest: image = secondary ? Images.GRASS_2 : Images.GRASS_1;
                    break;
                case Game1.Environments.Cemetary:
                case Game1.Environments.Desert:
                case Game1.Environments.Jungle:
                case Game1.Environments.Jail:
                case Game1.Environments.Catacomb:
                case Game1.Environments.Cave: image = secondary ? Images.DIRT_2 : Images.DIRT_1;
                    break;
                case Game1.Environments.Castle: image = secondary ? Images.CARPET_BOTTOM : Images.CARPET_TOP;
                    break;
                case Game1.Environments.Temple: image = secondary ? Images.DIRT_2 : Images.DIRT_1;
                    break;
                case Game1.Environments.Facility: image = Images.TILE_FLOOR;
                    break;
                case Game1.Environments.House: image = Images.RUG;
                    break;
            }

            return new Tile(image, position.X, position.Y, (int)Game1.Layers.Floor);
        }

        private static Door LoadDoor(Game1.Environments environment, Point position, List<IGameObject> spriteSet = null)
        {
            Texture2D image;

            if (spriteSet != null)
            {
                foreach (IGameObject i in spriteSet)

                    if (i is Door && (i as Door).GridX == position.X && (i as Door).GridY == position.Y - 1)
                    {
                        image = Images.DOOR_WOODEN_BOTTOM;

                        if (environment == Game1.Environments.Castle)
                            image = Images.DOOR_CASTLE_BOTTOM;
                        else if (environment == Game1.Environments.Facility)
                            image = Images.DOOR_LAB_BOTTOM;
                        else if (environment == Game1.Environments.Jail)
                            image = Images.DOOR_JAIL_BOTTOM;
                        else if (environment == Game1.Environments.Cemetary)
                            image = Images.DOOR_CEMETARY_BOTTOM;

                        Door iDoor = i as Door;

                        Door doorBottom = new Door(image, position.X, position.Y);
                        doorBottom.LinkTo(iDoor);
                        iDoor.LinkTo(doorBottom);

                        return doorBottom;
                    }
            }

            image = Images.DOOR_WOODEN_TOP;

            if (environment == Game1.Environments.Castle)
                image = Images.DOOR_CASTLE_TOP;
            else if (environment == Game1.Environments.Facility)
                image = Images.DOOR_LAB_TOP;
            else if (environment == Game1.Environments.Jail)
                image = Images.DOOR_JAIL_TOP;
            else if (environment == Game1.Environments.Cemetary)
                image = Images.DOOR_CEMETARY_TOP;

            return new Door(image, position.X, position.Y);
        }

        private static void AssociateSignPost(List<IGameObject> spriteSet)
        {
            Stack<Point> positions = new Stack<Point>();
            Stack<IGameObject> toRemove = new Stack<IGameObject>();

            foreach (IGameObject i in spriteSet)
            {
                if (i is Terrain && (i as Terrain).Image == Images.SIGN_POST)

                    foreach (IGameObject j in spriteSet)
                    {
                        Rectangle rect = new Rectangle(i.X, i.Y - Game1.TILE_SIZE, Game1.TILE_SIZE, Game1.TILE_SIZE);
                        
                        if (j is Plaque && j.Rect.Intersects(rect))
                        {
                            Terrain iTerrain = i as Terrain;
                            positions.Push(new Point(iTerrain.GridX, iTerrain.GridY));
                            toRemove.Push(iTerrain);
                        }
                    }

                while (toRemove.Count != 0)
                    spriteSet.Remove(toRemove.Pop());
                while (positions.Count != 0)
                {
                    Point position = positions.Pop();
                    spriteSet.Add(new Plaque(Images.SIGN_POST, position.X, position.Y));
                }
            }
        }

        private static void WaterTileReplacement(List<IGameObject> spriteSet)
        {	
	        List<Water> toAdd = new List<Water>();
	        List<Water> toRemove = new List<Water>();

	        Rectangle relativeTopLeft = new Rectangle(-Game1.TILE_SIZE, -Game1.TILE_SIZE, 1, 1);
	        Rectangle relativeTop = new Rectangle(0, -Game1.TILE_SIZE, 1, 1);
	        Rectangle relativeTopRight = new Rectangle(Game1.TILE_SIZE, -Game1.TILE_SIZE, 1, 1);
	        Rectangle relativeLeft = new Rectangle(-Game1.TILE_SIZE, 0, 1, 1);
	        Rectangle relativeRight = new Rectangle(Game1.TILE_SIZE, 0, 1, 1);
	        Rectangle relativeBottomLeft = new Rectangle(-Game1.TILE_SIZE, Game1.TILE_SIZE, 1, 1);
            Rectangle relativeBottom = new Rectangle(0, Game1.TILE_SIZE, 1, 1);
	        Rectangle relativeBottomRight = new Rectangle(Game1.TILE_SIZE, Game1.TILE_SIZE, 1, 1);

	        Rectangle absoluteTopLeft;
	        Rectangle absoluteTop;
	        Rectangle absoluteTopRight;
	        Rectangle absoluteLeft;
	        Rectangle absoluteRight;
	        Rectangle absoluteBottomLeft;
            Rectangle absoluteBottom;
	        Rectangle absoluteBottomRight;

	        bool topLeftCollision;
	        bool topCollision;
	        bool topRightCollision;
	        bool leftCollision;
	        bool rightCollision;
	        bool bottomLeftCollision;
	        bool bottomCollision;
	        bool bottomRightCollision;

	        foreach (IGameObject i in spriteSet)
	        {
		        if (i is Water)
		        {
			        Water iWater = i as Water;

			        absoluteTopLeft = WaterTileGetAbsolute(relativeTopLeft, i);
			        absoluteTop = WaterTileGetAbsolute(relativeTop, i);
			        absoluteTopRight = WaterTileGetAbsolute(relativeTopRight, i);
			        absoluteLeft = WaterTileGetAbsolute(relativeLeft, i);
			        absoluteRight = WaterTileGetAbsolute(relativeRight, i);
			        absoluteBottomLeft = WaterTileGetAbsolute(relativeBottomLeft, i);
			        absoluteBottom = WaterTileGetAbsolute(relativeBottom, i);
			        absoluteBottomRight = WaterTileGetAbsolute(relativeBottomRight, i);

			        topLeftCollision = WaterTileCollides(absoluteTopLeft, spriteSet);
			        topCollision = WaterTileCollides(absoluteTop, spriteSet);
			        topRightCollision = WaterTileCollides(absoluteTopRight, spriteSet);
			        leftCollision = WaterTileCollides(absoluteLeft, spriteSet);
			        rightCollision = WaterTileCollides(absoluteRight, spriteSet);
			        bottomLeftCollision = WaterTileCollides(absoluteBottomLeft, spriteSet);
			        bottomCollision = WaterTileCollides(absoluteBottom, spriteSet);
			        bottomRightCollision = WaterTileCollides(absoluteBottomRight, spriteSet);

			        if (!leftCollision && !topLeftCollision && !topCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_TOP_LEFT, iWater.GridX, iWater.GridY));
			        }
			        else if (!rightCollision && !topRightCollision && !topCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_TOP_RIGHT, iWater.GridX, iWater.GridY));
			        }
			        else if (!leftCollision && !bottomLeftCollision && !bottomCollision)
			        {	
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_BOTTOM_LEFT, iWater.GridX, iWater.GridY));
			        }
			        else if(!rightCollision && !bottomRightCollision && !bottomCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_BOTTOM_RIGHT, iWater.GridX, iWater.GridY));
			        }
			        else if (!leftCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_LEFT, iWater.GridX, iWater.GridY));
			        }
			        else if (!rightCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_RIGHT, iWater.GridX, iWater.GridY));
			        }
			        else if (!topCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_TOP, iWater.GridX, iWater.GridY));
			        }
			        else if (!bottomCollision)
			        {
				        toRemove.Add(i as Water);
				        toAdd.Add(new Water(Images.WATER_EDGE_BOTTOM, iWater.GridX, iWater.GridY));
			        }
		        }
	        }

	        foreach (Water i in toRemove)
		        spriteSet.Remove(i);

	        foreach (Water i in toAdd)
		        spriteSet.Add(i);
        }

        private static Rectangle WaterTileGetAbsolute(Rectangle relative, IGameObject i)
        {
	        return new Rectangle(i.X + relative.X, i.Y + relative.Y, relative.Width, relative.Height);
        }

        private static bool WaterTileCollides(Rectangle absolute, List<IGameObject> spriteSet)
        {
            if (absolute.X / Game1.TILE_SIZE < 0 || 
                absolute.X / Game1.TILE_SIZE >= Game1.GRID_SIZE_X || 
                absolute.Y / Game1.TILE_SIZE < 0 || 
                absolute.Y / Game1.TILE_SIZE >= Game1.GRID_SIZE_Y)

                return true;

	        foreach (IGameObject i in spriteSet)
            { 
                bool isBridge = i is Tile && (i.Image == Images.BRIDGE_LEFT || i.Image == Images.BRIDGE_RIGHT);

                if ((i is Water || i is Alligator || isBridge) && absolute.Intersects(i.Rect))
			
			        return true;
            }

            return false;
        }

        private static Game1.SyncStates RetrieveSyncState(Queue<Game1.SyncStates> syncState)
        {
            if (syncState.Count > 0)

                return syncState.Dequeue();

            return Game1.SyncStates.FirstQuarter;
        }

        private static T RetrieveFromQueue<T>(Queue<T> queue)
        {
            if (queue.Count > 0)
                return queue.Dequeue();

            return default(T);
        }
    }
}
