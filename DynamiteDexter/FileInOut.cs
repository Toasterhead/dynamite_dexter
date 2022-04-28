using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamiteDexter
{
    public static class FileInOut
    {
        public enum GameDataFiles { Map = 0, BossLocation, DenizenLocations, EntryPoints, HighScore }

        private const char COMMAND_MARKER = '$';
        private const char DELIMITER_PRIMARY = ':';
        private static char[] delimiters = { COMMAND_MARKER, ':', ',' };

        private const string FILE_NAME_MAP = "dex_map.dat";
        private const string FILE_NAME_BOSS_LOCATION = "dex_boss_location.dat";
        private const string FILE_NAME_DENIZEN_LOCATION = "dex_denizen_location.dat";
        private const string FILE_NAME_ENTRY_POINTS = "dex_entry_points.dat";
        private const string FILE_NAME_HIGH_SCORE = "dex_high_score.dat";
        private const string ROOM_COMMAND = "$room:";

        private static readonly string[] _defaultHighScoreContents = new string[23]
        {
            "$dollars",
            "$Name0:1500:0",
            "$Name1:1350:0",
            "$Name2:1200:0",
            "$Name3:1050:0",
            "$Name4:900:0",
            "$Name5:750:0",
            "$Name6:600:0",
            "$Name7:450:0",
            "$Name8:300:0",
            "$Name9:150:0",
            "$speedrun",
            "$Name0:14400:0",
            "$Name1:19800:0",
            "$Name2:25200:0",
            "$Name3:30600:0",
            "$Name4:36000:0",
            "$Name5:41400:0",
            "$Name6:46800:0",
            "$Name7:52200:0",
            "$Name8:57600:0",
            "$Name9:63000:0",
            "$end"
        };

        public static List<string>[,] mapFileData;
        public static List<string> bossLocationFileData;
        public static List<string> denizenLocationFileData;
        public static List<string> entryPointsFileData;
        public static List<string> highScoreFileData;

        public static void Initialize()
        {
            mapFileData = new List<string>[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];
            bossLocationFileData = new List<string>();
            denizenLocationFileData = new List<string>();
            entryPointsFileData = new List<string>();
            highScoreFileData = new List<string>();

            for (int i = 0; i < Game1.WORLD_SIZE_X; i++)
                for (int j = 0; j < Game1.WORLD_SIZE_Y; j++)
                    mapFileData[i, j] = new List<string>();
        }

        public static async void LoadFromFileAsync(GameDataFiles gameDataFile)
        {
            Windows.Storage.StorageFolder storageFolder =
                gameDataFile == GameDataFiles.HighScore ?
                Windows.Storage.ApplicationData.Current.LocalFolder :
                Windows.ApplicationModel.Package.Current.InstalledLocation;

            string filename;

            switch (gameDataFile)
            {
                case GameDataFiles.Map:
                    filename = FILE_NAME_MAP;
                    break;
                case GameDataFiles.BossLocation:
                    filename = FILE_NAME_BOSS_LOCATION;
                    break;
                case GameDataFiles.DenizenLocations:
                    filename = FILE_NAME_DENIZEN_LOCATION;
                    break;
                case GameDataFiles.EntryPoints:
                    filename = FILE_NAME_ENTRY_POINTS;
                    break;
                case GameDataFiles.HighScore:
                    filename = FILE_NAME_HIGH_SCORE;
                    break;
                default: throw new Exception("Error - unable to recognize game data file type.");
            }

            if (!File.Exists(storageFolder.Path + "\\" + filename))
            {
                if (gameDataFile == GameDataFiles.HighScore)
                {
                    Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(
                        filename,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);

                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                    using (var outputStream = stream.GetOutputStreamAt(0))
                    {
                        using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                        {
                            foreach (string i in _defaultHighScoreContents)
                                dataWriter.WriteString(i + Environment.NewLine);

                            await dataWriter.StoreAsync();
                            await outputStream.FlushAsync();
                        }
                    }
                    stream.Dispose();
                }
                else throw new Exception("Error - file not found: " + filename);
            }

            Windows.Storage.StorageFile fileRead = await storageFolder.GetFileAsync(filename);

            string text = await Windows.Storage.FileIO.ReadTextAsync(fileRead);

            string[] lines = text.Split(new char[] { Environment.NewLine[0], Environment.NewLine[1] });

            switch (gameDataFile)
            {
                case GameDataFiles.Map:

                    int x = 0;
                    int y = 0;

                    foreach (string line in lines)
                    {
                        if (line.Length >= ROOM_COMMAND.Length && line.Substring(0, ROOM_COMMAND.Length) == ROOM_COMMAND)
                        {
                            string[] terms = line.Split(delimiters);
                            x = Convert.ToInt32(terms[2]);
                            y = Convert.ToInt32(terms[3]);
                        }

                        if (line != "") mapFileData[x, y].Add(line);
                    }

                    break;
                case GameDataFiles.BossLocation:
                    foreach (string line in lines)
                        if (line != "") bossLocationFileData.Add(line);
                    break;
                case GameDataFiles.DenizenLocations:
                    foreach (string line in lines)
                        if (line != "") denizenLocationFileData.Add(line);
                    break;
                case GameDataFiles.EntryPoints:
                    foreach (string line in lines)
                        if (line != "") entryPointsFileData.Add(line);
                    break;
                case GameDataFiles.HighScore:
                    foreach (string line in lines)
                        if (line != "") highScoreFileData.Add(line);
                    HighScore.LoadFromFileData(highScoreFileData);
                    HighScore.WriteToMenu();
                    break;
                default: throw new Exception("Error - unable to recognize game data file type.");
            }
        }

        public static async void WriteHighScoresToFileAsync(HighScore.FieldEntry[] topScoreDollars, HighScore.FieldEntry[] topScoreSpeedrun)
        {
            List<string> lines = new List<string>();

            lines.Add(COMMAND_MARKER + "dollars");

            foreach (HighScore.FieldEntry i in topScoreDollars)
                lines.Add(COMMAND_MARKER + i.name + delimiters[1] + i.score + delimiters[1] + (i.star ? 1 : 0));

            lines.Add(COMMAND_MARKER + "speedrun");

            foreach (HighScore.FieldEntry i in topScoreSpeedrun)
                lines.Add(COMMAND_MARKER + i.name + delimiters[1] + i.score + delimiters[1] + 0);

            lines.Add(COMMAND_MARKER + "end");

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (File.Exists(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\" + FILE_NAME_HIGH_SCORE))
            {
                Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(FILE_NAME_HIGH_SCORE);

                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        foreach (string line in lines)
                            dataWriter.WriteString(line + Environment.NewLine);

                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream.Dispose();
            }
        }
    }
}