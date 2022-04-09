using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public static class EndGameB
    {
        public class AvatarScripted : SpriteSheet
        {
            private const float SPEED = 2.0f;
            private const int T_START = -10;

            private readonly int _initialY;

            private int t;
            private uint scaleDelay;
            private uint scaleDelayCount;
            private Rectangle scaleRect;

            public enum Actions { Standing, Walking, Jumping }
            public enum Directions { Left, Right, TowardHorizon }

            public int InitialY { get { return _initialY; } }
            public Actions CurrentAction { get; set; }
            public Directions CurrentDirection { get; set; }
            public Rectangle ScaleRect { get { return scaleRect; } }

            public AvatarScripted(bool inSpacesuit = false)
                : base(
                    new SpriteInfo(
                        inSpacesuit ? Images.PLAYER_SPACESUIT : Images.PLAYER,
                        inSpacesuit ? -10 : Game1.fullfield.X + (2 * Game1.TILE_SIZE),
                        inSpacesuit ? Game1.fullfield.Y - 20 : Game1.fullfield.Y - (2 * Game1.TILE_SIZE),
                        (int)Game1.Layers.Actor),
                    new CollisionInfo(null, null),
                    new AnimationInfo(3, 4, 3))
            {
                _initialY = Y;
                Reset(inSpacesuit);
            }

            public void Reset(bool inSpacesuit)
            {
                t = T_START;
                scaleDelay = 0;
                scaleDelayCount = 0;
                scaleRect = new Rectangle(X, Y, Width, Height);
                CurrentAction = Actions.Walking;
                CurrentDirection = inSpacesuit ? Directions.Right : Directions.Left;
                render = inSpacesuit ? false : true;
            }

            private float JumpVelocity()
            {
                const double COEFFICIENT = 0.25;

                double jumpVelocity = (COEFFICIENT * Math.Pow(t, 2)) - (COEFFICIENT * Math.Pow(t - 1, 2));
                t++;

                return (float)jumpVelocity;
            }

            protected override void Animate()
            {
                if (CurrentAction == Actions.Standing)
                    tileSelection.X = 2;
                else if (CurrentAction == Actions.Jumping)
                    tileSelection.X = 1;
                else if (CurrentAction == Actions.Walking && ++delayCount == interval)
                {
                    delayCount = 0;
                    tileSelection.X = tileSelection.X == 1 ? 2 : 1;
                }

                if (CurrentDirection == Directions.Left)
                    tileSelection.Y = 0;
                else if (CurrentDirection == Directions.Right)
                    tileSelection.Y = 1;
                else tileSelection.Y = 2;

                SetFrame();
            }

            public override void Update()
            {
                scaleRect = new Rectangle(X, Y, scaleRect.Width, scaleRect.Height);

                if (CurrentAction == Actions.Walking)
                {
                    if (CurrentDirection == Directions.Left)
                        velocity = new Vector2(-SPEED, 0.0f);
                    else if (CurrentDirection == Directions.Right)
                        velocity = new Vector2(SPEED, 0.0f);
                    else
                    {
                        if (++scaleDelayCount >= scaleDelay)
                        {
                            scaleDelayCount = 0;

                            Reposition(X, Y - 1);
                            velocity = new Vector2();

                            if (scaleRect.Width > 2) scaleRect.Width--;
                            if (scaleRect.Height > 2) scaleRect.Height--;

                            scaleDelay++;
                        }
                    }
                }
                else if (CurrentAction == Actions.Jumping)
                {
                    if (CurrentDirection == Directions.Left)
                        velocity = new Vector2(-SPEED, JumpVelocity());
                    else if (CurrentDirection == Directions.Right)
                        velocity = new Vector2(SPEED, JumpVelocity());
                }

                base.Update();
            }
        }

        public class SpaceshipScripted : SpritePlus
        {
            public enum Actions { Stationary, TakingOff, Landing }

            private const float MAX_SPEED = 4.0f;
            private const float ACCELERATION_RATE = 0.1f;

            private readonly int _landingPointY;
            private readonly Sprite _exhaust;

            private float currentSpeed;

            public Actions CurrentAction { get; set; }
            public Sprite Exhaust { get { return _exhaust; } }

            public SpaceshipScripted(int landingPointY)
                : base(Images.SPACESHIP, 32, Game1.fullfield.Y - Game1.TILE_SIZE - Images.SPACESHIP.Height, (int)Game1.Layers.Actor)
            {
                _landingPointY = landingPointY;
                _exhaust = new Sprite(Images.SPACESHIP_EXHAUST, Left, Bottom, (int)Game1.Layers.Actor);
                CurrentAction = Actions.Stationary;
                currentSpeed = 0.0f;
                render = true;
                _exhaust.Render = false;
            }

            public void ResetSpeed() { currentSpeed = 0.0f; }

            public override void Update()
            {
                if (CurrentAction == Actions.TakingOff)
                {
                    if (currentSpeed < MAX_SPEED)
                        currentSpeed += ACCELERATION_RATE;

                    orientation = SpriteEffects.FlipHorizontally;
                    velocity = new Vector2(0.0f, -currentSpeed);
                    _exhaust.Render = _exhaust.Render ? false : true;
                }
                else if (CurrentAction == Actions.Landing)
                {
                    orientation = SpriteEffects.FlipVertically;
                    velocity = new Vector2(0.0f, MAX_SPEED);
                    _exhaust.Render = false;
                }
                else
                {
                    orientation = SpriteEffects.None;
                    velocity = new Vector2();
                    _exhaust.Render = false;
                }

                base.Update();

                _exhaust.Reposition(Left, Bottom);
            }
        }

        public class CelestialBody : SpriteSheet
        {
            private const int SPAN_HORIZONTAL = 1000;
            private const int SPAN_VERTICAL = 750;

            public CelestialBody(Texture2D image)
                : base(
                    new SpriteInfo(
                        image,
                        Game1.rand.Next(-SPAN_HORIZONTAL, SPAN_HORIZONTAL),
                        Game1.rand.Next(-SPAN_VERTICAL, SPAN_VERTICAL),
                        Game1.rand.Next(Z_FRINGE, 0)),
                    new CollisionInfo(null, null),
                    new AnimationInfo(4, 1, 0)) { render = false; }

            protected override void Animate()
            {
                int quarterDistance = Z_FRINGE / sheetDimensions.X;

                for (int i = 0; i < sheetDimensions.X; i++)

                    if (Z > (i + 1) * quarterDistance)
                    {
                        tileSelection.X = sheetDimensions.X - (i + 1);
                        break;
                    }

                SetFrame();
            }

            public override void Update()
            {
                if (++layer > 0)
                {
                    layer = Z_FRINGE;
                    Reposition(
                        Game1.rand.Next(-SPAN_HORIZONTAL, SPAN_HORIZONTAL),
                        Game1.rand.Next(-SPAN_VERTICAL, SPAN_VERTICAL));
                    render = true;
                }
                base.Update();
            }
        }

        public class Fade : SpriteSheet
        {
            private readonly bool _reverse;

            private bool complete;

            public bool Complete { get { return complete; } }

            public Fade(uint interval, bool reverse = false)
                : base(
                      new SpriteInfo(Images.FADE_SCREEN, 0, 0, (int)Game1.Layers.Overhead),
                      new CollisionInfo(null, null),
                      new AnimationInfo(8, 1, interval))
            {
                _reverse = reverse;
                tileSelection.X = _reverse ? sheetDimensions.X - 1 : 0;
                render = false;
            }

            protected override void Animate()
            {
                if (++delayCount == interval)
                {
                    delayCount = 0;

                    if (_reverse && tileSelection.X >= 0)
                        tileSelection.X--;
                    else if (!_reverse && tileSelection.X < sheetDimensions.X)
                        tileSelection.X++;
                }

                SetFrame();
            }

            public override void Update()
            {
                if ((_reverse && tileSelection.X < 0) ||
                    (!_reverse && tileSelection.X >= sheetDimensions.X))
                { 
                    render = false;
                    complete = true;
                }
                base.Update();
            }
        }

        private enum Phases { Boarding, Cruising, Landing }

        private const int NUM_CELESTIAL_BODIES = 24;

        public const int Z_FRINGE = -300;
        public const int SPACESHIP_VERTICAL_LIMIT = -200;
        public const int SPACESHIP_LANDING_X = 37;
        public const int CHANGE_TEXT_INTERVAL = 150;
        public const int DISPLAY_TEXT_DURATION = 125;
        public const int MOUNTAIN_ONE_VERTICAL_POSITION = 100;
        public const int MOUNTAIN_TWO_VERTICAL_POSITION = 80;
        public const int PLANET_HORIZON = 165;
        public const int VERTICAL_POSITION_STARS = 2;
        public const int VERTICAL_POSITION_MOUNTAINS_1 = 60;
        public const int VERTICAL_POSITION_MOUNTAINS_2 = 95;
        public const int VERTICAL_POSITION_LANDING = 220;
        public const int VERTICAL_POSITION_TEXT = 127;

        public static readonly Textfield Text = new Textfield(Game1.characterSet, "", 0, 0, 320, Textfield.WriteMode.Immediate);

        private static readonly string[] textLines = new string[7]
        {
            "  You have completed Dynamite Dexter.  ",
            "Programming, Music, Visuals, and Design",
            "            by Lenny Young             ",
            "            Developed With             ",
            "  C#, Monogame, HLSL, and Famitracker  ",
            "          Thanks for playing!          ",
            "    (Press any key to continue...)     "
        };

        private static bool sequenceComplete;
        private static int textIndex;
        private static uint quake;
        private static Phases currentPhase;
        private static AvatarScripted avatarScripted;
        private static AvatarScripted avatarSpacesuitScripted;
        private static SpaceshipScripted spaceshipScripted;
        private static SpriteSheet spaceshipSmoke;
        private static CelestialBody[] celestialBodies;

        public static bool ButtonPressed { get; set; }
        public static bool SequenceComplete { get { return sequenceComplete; } }
        public static bool CreditsComplete { get { return textIndex == textLines.Length; } }
        public static bool CrashLanded { get; set; }
        public static bool Boarding { get { return currentPhase == Phases.Boarding; } }
        public static bool Cruising { get { return currentPhase == Phases.Cruising; } }
        public static bool Landing { get { return currentPhase == Phases.Landing; } }
        public static uint Quake { get { return quake; } }
        public static Fade TheFade { get; set; }
        public static AvatarScripted TheAvatarScripted { get { return avatarScripted; } }
        public static AvatarScripted TheAvatarSpacesuitScripted { get { return avatarSpacesuitScripted; } }
        public static SpaceshipScripted TheSpaceshipScripted { get { return spaceshipScripted; } }
        public static SpriteSheet SpaceshipSmoke { get { return spaceshipSmoke; } }
        public static CelestialBody[] CelestialBodies { get { return celestialBodies; } }

        public static void InitializeScene()
        {
            sequenceComplete = false;
            textIndex = 0;
            quake = 0;
            TheFade = new Fade(6);
            Text.ChangeText("");
            ButtonPressed = false;
            CrashLanded = false;
            currentPhase = Phases.Boarding;
            celestialBodies = new CelestialBody[NUM_CELESTIAL_BODIES];
            avatarScripted = new AvatarScripted();
            avatarSpacesuitScripted = new AvatarScripted(true);
            spaceshipScripted = new SpaceshipScripted(Game1.fullfield.Y - 20);
            spaceshipSmoke = new SpriteSheet(Images.SPACESHIP_SMOKE, 50, 125, 2, 1, 5);
            spaceshipSmoke.Render = false;

            for (int i = 0; i < NUM_CELESTIAL_BODIES; i++)

                if (i < 1)
                    celestialBodies[i] = new CelestialBody(Images.PLANET);
                else if (i < 2)
                    celestialBodies[i] = new CelestialBody(Images.PLANET_RINGED);
                else if (i < NUM_CELESTIAL_BODIES / 3)
                    celestialBodies[i] = new CelestialBody(Images.STAR_BRIGHT);
                else celestialBodies[i] = new CelestialBody(Images.STAR);

            Game1.universalTimeStamp = Game1.universalTimer;
        }

        public static void ProcessBoardingPhase()
        {
            if (TheSpaceshipScripted.Bottom < SPACESHIP_VERTICAL_LIMIT)
                currentPhase = Phases.Cruising;
            else if (TheAvatarScripted.X < 0)
                TheSpaceshipScripted.CurrentAction = SpaceshipScripted.Actions.TakingOff;
            else if (TheAvatarScripted.X < TheSpaceshipScripted.Right - 10)
                TheAvatarScripted.Render = false;
            else if (TheAvatarScripted.X < TheSpaceshipScripted.Right + 20)
                TheAvatarScripted.CurrentAction = AvatarScripted.Actions.Jumping;
        }

        public static Vector2 PerspectiveTransform(Sprite sprite)
        {
            const double SCALE_RATE = 0.99;

            Vector2 a = new Vector2(sprite.Center.X, sprite.Center.Y);
            Vector2 vanishingPoint = new Vector2(Game1.fullfield.X / 2.0f, Game1.fullfield.Y / 2.0f);
            Vector2 translated = a - vanishingPoint;
            Vector2 transformed = new Vector2(
                sprite.X * (float)Math.Pow(SCALE_RATE, -sprite.Z), 
                sprite.Y * (float)Math.Pow(SCALE_RATE, -sprite.Z));
            Vector2 aPrime = transformed + vanishingPoint;

            return aPrime;
        }

        public static void ChangeText()
        {
            if (textIndex < textLines.Length)
                Text.ChangeText(textLines[textIndex++]);
        }

        public static void BeginLanding()
        {
            TheFade = new Fade(4, true);
            TheFade.Render = true;
            TheFade.Update();
            currentPhase = Phases.Landing;
            spaceshipScripted.CurrentAction = SpaceshipScripted.Actions.Landing;
            spaceshipScripted.ResetSpeed();
            spaceshipScripted.Reposition(SPACESHIP_LANDING_X, SPACESHIP_VERTICAL_LIMIT);
        }

        public static void BeginQuake() { quake = 30; }
        public static void ProcessQuake() { if (quake > 0) quake--; }
        public static void ConcludeSequence() { sequenceComplete = true; }
    }
}
