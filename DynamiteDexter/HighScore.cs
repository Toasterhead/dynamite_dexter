using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamiteDexter
{
    public static class HighScore
    {
        public struct FieldEntry
        {
            public string name;
            public int score;
            public bool star;

            public FieldEntry(string name, int score, bool star)
            {
                this.name = name;
                this.score = score;
                this.star = star;
            }
        }

        private const int MAX_ENTRY_LENGTH = 22;

        public const int LIST_LENGTH = 10;

        private static FieldEntry[] topScoreDollars = new FieldEntry[LIST_LENGTH];
        private static FieldEntry[] topScoreSpeedrun = new FieldEntry[LIST_LENGTH];
        private static string nameEntry = "";
        private static int virtualKeypadIndex = 1;
        private static bool virtualCapsLockOn = false;
        private static bool finished = false;
        private static char markerCommand = '$';

        private static char[] delimiters = { '$', ':' };
        private static List<IGameObject> spriteSet;

        public const string PROMPT = "Enter name: ";
        public const string VIRTUAL_KEYPAD_LOWERCASE = "^abcdefghijklmnopqrstuvwxyz <";
        public const string VIRTUAL_KEYPAD_UPPERCASE = "~ABCDEFGHIJKLMNOPQRSTUVWXYZ <";
        public const string CONFIRMATION = "    End";
        public const int NUM_LAYERS = 6;

        public static string Header { get { return SubmittingSpeedrun ? "NEW SPEED RUN!" : "NEW HIGH SCORE!"; } }
        public static bool VirtualCapsLockOn { get { return virtualCapsLockOn; } }
        public static bool Finished { get { return finished; } }
        public static int VirtualKeypadIndex { get { return virtualKeypadIndex; } }
        public static string NameEntry { get { return nameEntry; } }
        public static List<IGameObject> SpriteSet { get { return spriteSet; } }
        public static bool ConfettiFalling { get; set; }
        public static bool SubmittingSpeedrun { get; set; }
        public static char Selection
        {
            get
            {
                if (virtualKeypadIndex == VIRTUAL_KEYPAD_LOWERCASE.Length)

                    return '!';

                else if (virtualCapsLockOn)

                    return VIRTUAL_KEYPAD_UPPERCASE[virtualKeypadIndex];

                return VIRTUAL_KEYPAD_LOWERCASE[virtualKeypadIndex];
            }
        }
        public static FieldEntry[] TopScoreDollars
        {
            get
            {
                FieldEntry[] temp = new FieldEntry[topScoreDollars.Length];

                for (int i = 0; i < topScoreDollars.Length; i++)
                    temp[i] = topScoreDollars[i];

                return temp;
            }
        }
        public static FieldEntry[] TopScoreSpeedrun
        {
            get
            {
                FieldEntry[] temp = new FieldEntry[topScoreSpeedrun.Length];

                for (int i = 0; i < topScoreSpeedrun.Length; i++)
                    temp[i] = topScoreSpeedrun[i];

                return temp;
            }
        }

        public static void ResetFields()
        {
            finished = false;
            nameEntry = "";
            virtualCapsLockOn = true;
            virtualKeypadIndex = 1;
        }

        public static void MoveCursorLeft()
        {
            if (--virtualKeypadIndex < 0)
                virtualKeypadIndex = VIRTUAL_KEYPAD_LOWERCASE.Length;
            Game1.PlaySound(Sounds.COUNTDOWN);
        }

        public static void MoveCursorRight()
        {
            if (++virtualKeypadIndex > VIRTUAL_KEYPAD_LOWERCASE.Length)
                virtualKeypadIndex = 0;
            Game1.PlaySound(Sounds.COUNTDOWN);
        }

        public static void ExecuteCommand()
        {
            if (virtualKeypadIndex == VIRTUAL_KEYPAD_LOWERCASE.Length)
                finished = true;
            else if (VIRTUAL_KEYPAD_LOWERCASE[virtualKeypadIndex] == '<')
            {
                if (nameEntry.Length > 0)
                {
                    nameEntry = nameEntry.Substring(0, nameEntry.Length - 1);
                    Game1.PlaySound(Sounds.COUNTDOWN);
                }
                else Game1.PlaySound(Sounds.DENY);
            }
            else if (VIRTUAL_KEYPAD_LOWERCASE[virtualKeypadIndex] == '_')
            {
                nameEntry += " ";
                Game1.PlaySound(Sounds.COUNTDOWN);
            }
            else if (virtualKeypadIndex == 0)
            {
                virtualCapsLockOn = !virtualCapsLockOn;
                Game1.PlaySound(Sounds.COUNTDOWN);
            }
            else
            {
                if (nameEntry.Length <= MAX_ENTRY_LENGTH)
                {
                    if (virtualCapsLockOn)
                        nameEntry += VIRTUAL_KEYPAD_UPPERCASE[virtualKeypadIndex];
                    else nameEntry += VIRTUAL_KEYPAD_LOWERCASE[virtualKeypadIndex];

                    Game1.PlaySound(Sounds.COUNTDOWN);
                }
                else Game1.PlaySound(Sounds.DENY);
            }         
        }

        public static void InitializeSprites()
        {
            spriteSet = new List<IGameObject>();
            for (int i = 0; i < 4; i++)
            {
                spriteSet.Add(new Sprite(
                    new SpriteInfo(
                        Images.BALLOON,
                        Game1.rand.Next(Game1.fullfield.X),
                        Game1.fullfield.Y + Game1.rand.Next(50),
                        0)));
                spriteSet.Add(new Sprite(
                    new SpriteInfo(
                        Images.STREAMER,
                        Game1.rand.Next(Game1.fullfield.X),
                        -Images.STREAMER.Height - Game1.rand.Next(50),
                        1)));
                spriteSet.Add(new SpriteSheet(
                    new SpriteInfo(
                        Images.CONFETTI,
                        Game1.rand.Next(Game1.fullfield.X),
                        -Images.CONFETTI.Height - Game1.rand.Next(50),
                        2),
                    new CollisionInfo(null, null),
                    new AnimationInfo(2, 1, 10)));
            }
        }
        
        public static void LoadFromFileData(List<string> highScoreData)
        {
            for (int i = 0; i < topScoreDollars.Length; i++)
                topScoreDollars[i] = new FieldEntry("[blank]", 0, false);
            for (int i = 0; i < topScoreSpeedrun.Length; i++)
                topScoreSpeedrun[i] = new FieldEntry("[blank]", 0, false);
 
            bool end = false;
            bool enteringDollars = true;
            int dollarScoreCount = 0;
            int speedrunScoreCount = 0;

            foreach (string line in highScoreData)
            {
                if (end) break;

                bool isValidLine = line != null && line != "";

                if (isValidLine && line[0] == markerCommand)
                {
                    string[] terms = line.Split(delimiters);

                    if (terms.Length == 2 && terms[1] == "dollars")
                        enteringDollars = true;
                    else if (terms.Length == 2 && terms[1] == "speedrun")
                        enteringDollars = false;
                    else if (terms.Length == 2 && terms[1] == "end")
                        end = true;
                    else if (terms.Length == 4)
                    {
                        if (enteringDollars && dollarScoreCount < topScoreDollars.Length)
                        {
                            topScoreDollars[dollarScoreCount] = new FieldEntry(terms[1], Convert.ToInt32(terms[2]), Convert.ToBoolean(Convert.ToInt32(terms[3])));
                            dollarScoreCount++;
                        }
                        else if (!enteringDollars && speedrunScoreCount < topScoreSpeedrun.Length)
                        {
                            topScoreSpeedrun[speedrunScoreCount] = new FieldEntry(terms[1], Convert.ToInt32(terms[2]), false);
                            speedrunScoreCount++;
                        }
                    }
                }
            }    
        }
        
        public static void WriteToMenu()
        {
            List<DataString> dataStringsOne = new List<DataString>();
            List<DataString> dataStringsTwo = new List<DataString>();

            for (int i = 0; i < topScoreDollars.Length; i++)
            {
                string name = topScoreDollars[i].name;
                string score = topScoreDollars[i].score.ToString();

                FormatName(ref name);

                if (topScoreDollars[i].score >= 100000)
                    score = "99999";

                dataStringsOne.Add(new DataString("highScore" + i, (i + 1) + (i + 1 < 10 ? " " : "") + ". " + name + " $ " + score));
            }

            for (int i = 0; i < topScoreSpeedrun.Length; i++)
            {
                const int DURATION_SECOND = 30;
                const int DURATION_MINUTE = 60 * DURATION_SECOND;
                const int DURATION_HOUR = 60 * DURATION_MINUTE;

                string name = topScoreSpeedrun[i].name;
                string score = topScoreSpeedrun[i].score.ToString();

                FormatName(ref name);

                int scoreInt = topScoreSpeedrun[i].score;

                string hours = (scoreInt / DURATION_HOUR) + ":";
                string minutes = ((scoreInt % DURATION_HOUR) / DURATION_MINUTE) + ":";
                string seconds = ((scoreInt % DURATION_MINUTE) / DURATION_SECOND) + ":";
                string thirds = (scoreInt % DURATION_SECOND).ToString();

                hours = hours.Length < 3 ? "0" + hours : hours;
                minutes = minutes.Length < 3 ? "0" + minutes : minutes;
                seconds = seconds.Length < 3 ? "0" + seconds : seconds;
                thirds = thirds.Length < 2 ? "0" + thirds : thirds;

                score = hours + minutes + seconds + thirds;

                if (scoreInt >= 100 * DURATION_HOUR)
                    score = "99:99:99:99";

                dataStringsTwo.Add(new DataString("speedRun" + i, (i + 1) + (i + 1 < 10 ? " " : "") + ". " + name + " " + score));
            }

            for (int i = 0; i < LIST_LENGTH; i++)
                if (!(MenuManager.TitleMenu.RemoveData("highScore" + i, typeof(string))))
                    throw new Exception("Error - String data not found.");
            foreach (DataString i in dataStringsOne)
                MenuManager.TitleMenu.AddData(i);

            for (int i = 0; i < LIST_LENGTH; i++)
                if (!(MenuManager.TitleMenu.RemoveData("speedRun" + i, typeof(string))))
                    throw new Exception("Error - String data not found.");
            foreach (DataString i in dataStringsTwo)
                MenuManager.TitleMenu.AddData(i);

            MenuItem[] highScoreItems = new MenuItem[LIST_LENGTH + 2];
            for (int i = 0; i < LIST_LENGTH; i++)
                highScoreItems[i] = new MIHeadline(MenuManager.TitleMenu.GetString("highScore" + i).Value);
            highScoreItems[LIST_LENGTH + 0] = new MISpace();
            highScoreItems[LIST_LENGTH + 1] = new MILink("View Speed Runs", MenuManager.TitleMenu.GetSubMenu("Speed Run"));

            MenuItem[] speedRunItems = new MenuItem[LIST_LENGTH + 2];
            for (int i = 0; i < LIST_LENGTH; i++)
                speedRunItems[i] = new MIHeadline(MenuManager.TitleMenu.GetString("speedRun" + i).Value);
            speedRunItems[LIST_LENGTH + 0] = new MISpace();
            speedRunItems[LIST_LENGTH + 1] = new MILink("Back to Main Menu", MenuManager.TitleMenu.GetSubMenu("Main"));

            MenuManager.TitleMenu.GetSubMenu("High Score").SetItems(highScoreItems.ToList());
            MenuManager.TitleMenu.GetSubMenu("Speed Run").SetItems(speedRunItems.ToList());
        }

        public static void InsertScore(string name, int score, bool star, bool speedrun = false)
        {
            //Note - Assumes that the high score list is sorted.

            FieldEntry[] topScoreList = speedrun ? topScoreSpeedrun : topScoreDollars;
            int insertionIndex = -1;

            for (int i = 0; i < LIST_LENGTH; i++)

                if ((speedrun && score < topScoreList[i].score) || 
                    (!speedrun && score > topScoreList[i].score))
                {
                    insertionIndex = i;
                    break;
                }

            if (insertionIndex == -1)

                return;

            for (int i = LIST_LENGTH - 1; i >= insertionIndex && i > 0; i--)
                topScoreList[i] = topScoreList[i - 1];

            topScoreList[insertionIndex] = new FieldEntry(name, score, star);
        }

        private static void FormatName(ref string name)
        {
            const int MAX_NAME_LENGTH = 18;

            if (name == null) return;//

            if (name.Length > MAX_NAME_LENGTH)
                name = name.Substring(0, MAX_NAME_LENGTH);
            else if (name.Length < MAX_NAME_LENGTH)
                for (int j = name.Length; j < MAX_NAME_LENGTH; j++)
                    name += " ";
        }
    }
}
