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
        private const char COMMAND_MARKER = '$';
        private const char DELIMITER_PRIMARY = ':';
        private static char[] delimiters = { COMMAND_MARKER, ':', ','};

        private const string FILE_NAME_MAP = "dex_map.dat";
        private const string ROOM_COMMAND = "$room:";

        public static List<string>[,] mapFileData;
        public static string[] highScoreFileData;
        public static string[] speedrunFileData;

        public static void Initialize()
        {
            mapFileData = new List<string>[Game1.WORLD_SIZE_X, Game1.WORLD_SIZE_Y];
            highScoreFileData = new string[HighScore.LIST_LENGTH];
            speedrunFileData = new string[HighScore.LIST_LENGTH];

            for (int i = 0; i < Game1.WORLD_SIZE_X; i++)
                for (int j = 0; j < Game1.WORLD_SIZE_Y; j++)
                    mapFileData[i, j] = new List<string>();
        }

        public static async void LoadFromMapFileAsync()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            string filename = FILE_NAME_MAP;
            int x = 0;
            int y = 0;

            if (!File.Exists(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\" + filename))
                throw new Exception("Error - map file not found.");

            Windows.Storage.StorageFile fileRead = await storageFolder.GetFileAsync(filename);

            string text = await Windows.Storage.FileIO.ReadTextAsync(fileRead);

            string[] lines = text.Split(new char[] { Environment.NewLine[0], Environment.NewLine[1] });

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
        }
    }
}
