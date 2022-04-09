using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public static class EndGameA
    {
        public class Animal : SpriteSheet
        {
            public bool Stationary { get; set; }
            public bool Offset { get; set; }

            public Animal(Texture2D image, int x, int y, int layer)
                : base(
                      new SpriteInfo(image, x, y, layer),
                      new CollisionInfo(null, null),
                      new AnimationInfo(4, 1, MARCHING_DELAY)) { }

            protected override void Animate()
            {
                if (Stationary)
                {
                    if (++delayCount == interval)
                    {
                        if (Offset)
                            tileSelection.X = tileSelection.X == 1 ? 3 : 1; 
                        else tileSelection.X = tileSelection.X == 0 ? 2 : 0;

                        delayCount = 0;
                    }

                    SetFrame();
                }
                else base.Animate();
            }
        }

        private enum Phases { Entering, Marching, Leaving }

        private const int MARCHING_DELAY = 7;
        private const int DISTANCE_APART = 50;
        private const int NUM_ANIMALS = 5;
        private const int LEADER_HORIZONTAL_POSITION = 42;

        public const int CHANGE_TEXT_INTERVAL = 150;
        public const int DISPLAY_TEXT_DURATION = 125;
        public const int OUTRO_DURATION = 210;
        public const int MOUNTAIN_ONE_VERTICAL_POSITION = 100;
        public const int MOUNTAIN_TWO_VERTICAL_POSITION = 80;
        public const int VERTICAL_POSITION_CLOUDS = 10;
        public const int VERTICAL_POSITION_MOUNTAINS_1 = 60;
        public const int VERTICAL_POSITION_MOUNTAINS_2 = 95;
        public const int VERTICAL_POSITION_TEXT = 127;

        public static readonly Textfield Text = new Textfield(Game1.characterSet, "", 0, 0, 320, Textfield.WriteMode.Immediate);

        private static readonly string[] textLines = new string[9]
        {
            "  You have completed Dynamite Dexter.  ",
            "Programming, Music, Visuals, and Design",
            "            by Lenny Young             ",
            "            Developed With             ",
            "  C#, Monogame, HLSL, and Famitracker  ",
            "       For an alternate ending,        ",
            "complete the game defeating every boss.",
            "          Thanks for playing!          ",
            "    (Press any key to continue...)     ",
        };

        private static bool leaveInitiated;
        private static int scroll;
        private static int textIndex;
        private static Phases currentPhase;
        private static SpriteSheet[] animals;

        private static SpriteSheet Leader { get; set; }

        public static bool ButtonPressed { get; set; }
        public static bool CreditsComplete { get { return textIndex == textLines.Length; } }
        public static bool Entering { get { return currentPhase == Phases.Entering; } }
        public static bool Marching { get { return currentPhase == Phases.Marching; } }
        public static bool Leaving { get { return currentPhase == Phases.Leaving; } }
        public static Animal Pig { get; set; }
        public static Animal Mouse { get; set; }
        public static Animal Elephant { get; set; }
        public static Animal Rabbit { get; set; }
        public static Animal Turtle { get; set; }
        public static SpriteSheet[] Animals
        {
            get
            {
                SpriteSheet[] temp = new SpriteSheet[NUM_ANIMALS];
                for (int i = 0; i < NUM_ANIMALS; i++)
                    temp[i] = animals[i];

                return temp;
            }
        }

        public static int MountainOneScroll { get { return (scroll / 3) % Game1.fullfield.X; } }
        public static int MountainTwoScroll { get { return (scroll / 2) % Game1.fullfield.X; } }
        public static int GrassScroll { get { return scroll % Game1.TILE_SIZE; } }

        public static void InitializeScene()
        {
            int marcherNumber = 0;

            Pig = new Animal(Images.ANIMAL_PIG, Game1.fullfield.X + (marcherNumber++ * DISTANCE_APART), Game1.fullfield.Y - Game1.TILE_SIZE - Images.ANIMAL_PIG.Height, (int)Game1.Layers.Overhead);
            Mouse = new Animal(Images.ANIMAL_MOUSE, Game1.fullfield.X + (marcherNumber++ * DISTANCE_APART), Game1.fullfield.Y - Game1.TILE_SIZE - Images.ANIMAL_MOUSE.Height, (int)Game1.Layers.Actor);
            Elephant = new Animal(Images.ANIMAL_ELEPHANT, Game1.fullfield.X + (marcherNumber++ * DISTANCE_APART), Game1.fullfield.Y - Game1.TILE_SIZE - Images.ANIMAL_ELEPHANT.Height, (int)Game1.Layers.Actor);
            Rabbit = new Animal(Images.ANIMAL_RABBIT, Game1.fullfield.X + (marcherNumber++ * DISTANCE_APART), Game1.fullfield.Y - Game1.TILE_SIZE - Images.ANIMAL_RABBIT.Height, (int)Game1.Layers.Overhead);
            Turtle = new Animal(Images.ANIMAL_TURTLE, Game1.fullfield.X + (marcherNumber++ * DISTANCE_APART), Game1.fullfield.Y - Game1.TILE_SIZE - Images.ANIMAL_TURTLE.Height, (int)Game1.Layers.Actor);

            Pig.Velocity = Mouse.Velocity = Elephant.Velocity = Rabbit.Velocity = Turtle.Velocity = new Vector2(-1.0f, 0.0f);
            Leader = Pig;
            animals = new SpriteSheet[NUM_ANIMALS] { Pig, Mouse, Elephant, Rabbit, Turtle };
            textIndex = 0;
            Text.ChangeText("");
            ButtonPressed = false;
            currentPhase = Phases.Entering;
            leaveInitiated = false;

            Game1.universalTimeStamp = Game1.universalTimer;
        }

        public static void CheckHalt()
        {
            if (currentPhase == Phases.Entering && Leader.X < LEADER_HORIZONTAL_POSITION)
            {
                Pig.Velocity = Mouse.Velocity = Elephant.Velocity = Rabbit.Velocity = Turtle.Velocity = new Vector2(0.0f, 0.0f);
                currentPhase = Phases.Marching;
            }
        }

        public static bool CheckLeave()
        {
            if (!leaveInitiated && CreditsComplete && ButtonPressed)
            {
                leaveInitiated = true;
                Pig.Velocity = Mouse.Velocity = Elephant.Velocity = Turtle.Velocity = new Vector2(-3.0f, 0.0f);
                Rabbit.Stationary = true;
                Rabbit.Offset = true;
                currentPhase = Phases.Leaving;
                Game1.PlayMusic(Sounds.Music.VICTORY_MARCH_OUTRO);

                return true;
            }

            return false;
        }

        public static void ChangeText()
        {
            if (textIndex < textLines.Length)
                Text.ChangeText(textLines[textIndex++]);
        }

        public static void UpdateScroll() { scroll++; }
    }
}
