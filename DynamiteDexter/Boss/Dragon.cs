using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Dragon : SpritePlus, IBoss, IHostile, IFires, IHasAttachments
    {
        private class DragonGate : SpriteSheet
        {
            private bool opening;

            public DragonGate(Texture2D image, int x, int y)
                : base(
                      new SpriteInfo(image, x, y, (int)Game1.Layers.Outline),
                      new CollisionInfo(null, null),
                      new AnimationInfo(5, 1, 3))
            { opening = false; }

            public void Open() { opening = true; }
            public void Close() { opening = false; }

            protected override void Animate()
            {
                if (opening && delayCount < interval)
                {
                    if (++delayCount >= interval)
                    {
                        delayCount = 0;

                        if (tileSelection.X < sheetDimensions.X - 1)
                            tileSelection.X++;
                    }
                }
                else if (!opening && delayCount < interval)
                {
                    if (++delayCount >= interval)
                    {
                        delayCount = 0;

                        if (tileSelection.X > 0)
                            tileSelection.X--;
                    }
                }

                SetFrame();
            }
        }

        private class DragonHead : SpriteSheet
        {
            public enum State { Hiding, Emerging, Blinking, Idle, OpeningMouth, ClosingMouth, Receding }

            private readonly Dragon _parent;

            private State currentState;

            public int TileSelectionX { get { return tileSelection.X; } }//
            public int TileSelectionY { get { return tileSelection.Y; } }//
            public bool Active { get; set; }
            public State CurrentState
            {
                get { return currentState; }
                set
                {
                    delayCount = 0;

                    switch (value)
                    {
                        case State.Hiding:
                            currentState = value;
                            tileSelection.X = 0;
                            tileSelection.Y = 0;
                            break;
                        case State.Emerging:
                            currentState = value;
                            tileSelection.X = 0;
                            tileSelection.Y = 2;
                            break;
                        case State.Blinking:
                        case State.Idle:
                            currentState = value;
                            tileSelection.X = 0;
                            tileSelection.Y = 0;
                            break;
                        case State.OpeningMouth:
                            currentState = value;
                            tileSelection.X = 0;
                            tileSelection.Y = 1;
                            break;
                        case State.ClosingMouth:
                            currentState = value;
                            tileSelection.X = sheetDimensions.X - 1;
                            tileSelection.Y = 1;
                            break;
                        case State.Receding:
                            currentState = value;
                            tileSelection.X = sheetDimensions.X - 1;
                            tileSelection.Y = 2;
                            break;
                    }
                }
            }

            public DragonHead(Texture2D image, int x, int y, Dragon parent)
                : base(
                      new SpriteInfo(image, x, y, (int)Game1.Layers.Actor),
                      new CollisionInfo(null, null),
                      new AnimationInfo(6, 3, 3))
            {
                _parent = parent;
                tileSelection.X = 0;
                tileSelection.Y = 2;
            }

            protected override void Animate()
            {
                if (Active)
                {
                    if (_parent.Flashing)

                        render = false;

                    else if (++delayCount >= interval)
                    {
                        delayCount = 0;

                        switch (CurrentState)
                        {
                            case State.Hiding:
                                render = false;
                                break;
                            case State.Emerging:
                                render = true;
                                if (++tileSelection.X >= sheetDimensions.X)
                                    CurrentState = State.Blinking;
                                break;
                            case State.Blinking:
                                render = true;
                                if (++tileSelection.X >= sheetDimensions.X)
                                    CurrentState = State.Idle;
                                break;
                            case State.Idle:
                                render = true;
                                break;
                            case State.OpeningMouth:
                                render = true;
                                if (tileSelection.X < sheetDimensions.X - 1)
                                    tileSelection.X++;
                                break;
                            case State.ClosingMouth:
                                render = true;
                                if (tileSelection.X > 0)
                                    tileSelection.X--;
                                break;
                            case State.Receding:
                                render = true;
                                if (tileSelection.X > 0)
                                    tileSelection.X--;
                                else currentState = State.Hiding;
                                break;
                        }
                    }
                }
                else render = false;

                SetFrame();
            }
        }

        private class DragonFire : SpriteSheet, IProjectile
        {
            private static readonly Rectangle[] _dragonFlameHitBox =
            {
                new Rectangle(2, 2, Game1.TILE_SIZE - 4, Game1.TILE_SIZE - 4)
            };

            private readonly IGameObject _parent;

            public bool ContactWithWall { get; set; }
            public bool ContactWithPlayer { get; set; }
            public IGameObject Parent { get { return _parent; } }

            public DragonFire(int x, int y, Vector2 velocity, IGameObject parent)
            : base(
                  new SpriteInfo(
                      Images.DRAGON_FIRE, 
                      x - (Game1.TILE_SIZE / 2), 
                      y - (Game1.TILE_SIZE / 2), 
                      (int)Game1.Layers.Actor),
                  new CollisionInfo(_dragonFlameHitBox, null),
                  new AnimationInfo(4, 1, 2))
            {
                _parent = parent;
                this.velocity = velocity;
            }
        }

        private enum Regions { Top = 0, Right, Bottom, Left, EnumSize }

        private const int INITIAL_VITALITY = 6;
        private const int NUM_FLAMES = 8;
        private const int BREATH_SPAN = 10;
        private const uint FLASH_DURATION = 30;
        private const uint INITIAL_ATTACK_CYCLE_DURATION = 300;
        private const uint ATTACK_CYCLE_DECREMENT = 30;
        private const float OPEN_GATE_MARKER = 1.00f;
        private const float EMERGE_MARKER = 0.95f;
        private const float OPEN_MOUTH_MARKER = 0.80f;
        private const float BREATHE_FIRE_MARKER = 0.725f;
        private const float WITHDRAW_FIRE_MARKER = 0.425f;
        private const float CLOSE_MOUTH_MARKER = 0.40f;
        private const float RECEDE_MARKER = 0.35f;
        private const float CLOSE_GATE_MARKER = 0.30f;
        private const float FIRE_INTERVAL = (BREATHE_FIRE_MARKER - WITHDRAW_FIRE_MARKER) / NUM_FLAMES;

        private static readonly Rectangle[] _dragonHitBox =
        {
            new Rectangle(
                (Game1.playfield.X / 2) - (Game1.TILE_SIZE / 2), 
                2, 
                Game1.TILE_SIZE,
                (3 * Game1.TILE_SIZE)),
            new Rectangle(
                Game1.playfield.X - (5 * (Game1.TILE_SIZE / 2)),
                (Game1.playfield.Y / 2) - (Game1.TILE_SIZE / 2),
                5 * (Game1.TILE_SIZE / 2),
                Game1.TILE_SIZE),
            new Rectangle(
                (Game1.playfield.X / 2) - (Game1.TILE_SIZE / 2), 
                Game1.playfield.Y - (3 * Game1.TILE_SIZE) - 2, 
                Game1.TILE_SIZE,
                (3 * Game1.TILE_SIZE)),
            new Rectangle(
                0,
                (Game1.playfield.Y / 2) - (Game1.TILE_SIZE / 2),
                5 * (Game1.TILE_SIZE / 2),
                Game1.TILE_SIZE),
            new Rectangle(0, 0, 0, 0)
        };
        private static readonly SpritePlus _appearingForm = new SpritePlus(Images.APPEARING_DRAGON, 0, 0);

        private readonly int _startX;
        private readonly int _startY;
        private readonly DragonGate[] _gate;
        private readonly DragonHead[] _head;
        private readonly Blade[] _blade;

        private bool struck;
        private int vitality;
        private uint flashCount;
        private uint attackCycleDuration;
        private uint attackCycleCount;
        private Regions currentRegion;
        private IProjectile[] chamber;

        private uint markerOpenGate;
        private uint markerEmerge;
        private uint markerOpenMouth;
        private uint markerBreatheFire;
        private uint markerWithdrawFire;
        private uint markerCloseMouth;
        private uint markerRecede;
        private uint markerCloseGate;
        private uint[] markerFire;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public int Vitality { get { return vitality; } }
        public bool Flashing { get { return flashCount % 2 != 0; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public Rectangle HitBoxAssault
        {
            get
            {
                if (attackCycleCount < (markerRecede - markerCloseGate) / 2 + markerCloseGate)

                    return GetHitBox((int)Regions.EnumSize);

                return GetHitBox((int)currentRegion);
            }
        }
        public Point Mouth
        {
            get
            {
                DragonHead head = _head[(int)currentRegion];

                switch (currentRegion)
                {
                    case Regions.Top:
                        return new Point(head.Center.X, head.Bottom - 1);
                    case Regions.Right:
                        return new Point(head.Left, head.Center.Y);
                    case Regions.Bottom:
                        return new Point(head.Center.X, head.Top);
                    case Regions.Left:
                        return new Point(head.Right - 1, head.Center.Y);
                }

                return new Point(head.Center.X, head.Bottom - 1);
            }
        }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.AddRange(_gate);
                attachment.AddRange(_head);
                attachment.AddRange(_blade);

                return attachment;
            }
        }

        public Dragon()
            : base(
                  new SpriteInfo(Images.BLANK, 0, 0, (int)Game1.Layers.Floor),
                  new CollisionInfo(_dragonHitBox, null))
        { 
            _startX = X;
            _startY = Y;
            _gate = new DragonGate[4]
            {
                new DragonGate(Images.DRAGON_GATE_TOP, Game1.playfield.X / 2 - (3 * (Game1.TILE_SIZE / 2)), 0),
                new DragonGate(Images.DRAGON_GATE_RIGHT, Game1.playfield.X - (2 * Game1.TILE_SIZE), Game1.playfield.Y / 2 - (3 * (Game1.TILE_SIZE / 2))),
                new DragonGate(Images.DRAGON_GATE_BOTTOM, Game1.playfield.X / 2 - (3 * (Game1.TILE_SIZE / 2)), Game1.playfield.Y - (2 * Game1.TILE_SIZE)),
                new DragonGate(Images.DRAGON_GATE_LEFT, 0, Game1.playfield.Y / 2 - (3 * (Game1.TILE_SIZE / 2)))
            };
            _head = new DragonHead[4]
            {
                new DragonHead(Images.DRAGON_FROM_TOP, Game1.playfield.X / 2 - Game1.TILE_SIZE, 2, this),
                new DragonHead(Images.DRAGON_FROM_RIGHT, Game1.playfield.X - (3 * Game1.TILE_SIZE), Game1.playfield.Y / 2 - Game1.TILE_SIZE, this),
                new DragonHead(Images.DRAGON_FROM_BOTTOM, Game1.playfield.X / 2 - Game1.TILE_SIZE, Game1.playfield.Y - (3 * Game1.TILE_SIZE), this),
                new DragonHead(Images.DRAGON_FROM_LEFT, 0, Game1.playfield.Y / 2 - Game1.TILE_SIZE, this)
            };
            _blade = new Blade[4]
            {
                new Blade(2, 2, new Point(0, 0)),
                new Blade(12, 2, new Point(0, 0)),
                new Blade(2, 12, new Point(0, 0)),
                new Blade(12, 12, new Point(0, 0))
            };
            struck = false;
            vitality = INITIAL_VITALITY;
            attackCycleDuration = INITIAL_ATTACK_CYCLE_DURATION;
            attackCycleCount = attackCycleDuration;
            markerFire = new uint[NUM_FLAMES];

            CalculateMarkers();

            currentRegion = (Regions)Game1.rand.Next((int)Regions.EnumSize);

            for (int i = 0; i < (int)Regions.EnumSize; i++)
            {
                _head[i].Active = (i == (int)currentRegion) ? true : false;
                _head[i].Render = false;
            }
        }

        public void EmptyChamber() { chamber = null; }

        public void Reset()
        {
            x = _startX;
            y = _startY;
            vitality = INITIAL_VITALITY;
            attackCycleDuration = INITIAL_ATTACK_CYCLE_DURATION;
            attackCycleCount = attackCycleDuration;
        }

        public void Strike()
        {
            if (--vitality == 0)
                remove = true;

            struck = true;
            flashCount = FLASH_DURATION;
        }

        private void Fire()
        {
            int displacement = Game1.rand.Next(BREATH_SPAN);
            int negation = Game1.rand.Next(2) == 0 ? 1 : -1;           
            Point mouth = Mouth;

            chamber = new IProjectile[1];

            if (displacement < 4)
            {
                if (currentRegion == Regions.Top)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(0.0f, 4.0f), this);
                else if (currentRegion == Regions.Right)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(-4.0f, 0.0f), this);
                else if (currentRegion == Regions.Bottom)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(0.0f, -4.0f), this);
                else if (currentRegion == Regions.Left)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(4.0f, 0.0f), this);
            }
            else if (displacement < 7)
            {
                if (currentRegion == Regions.Top)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(1.0f * negation, 3.0f), this);
                else if (currentRegion == Regions.Right)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(-3.0f, 1.0f * negation), this);
                else if (currentRegion == Regions.Bottom)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(1.0f * negation, -3.0f), this);
                else if (currentRegion == Regions.Left)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(3.0f, 1.0f * negation), this);
            }
            else if (displacement < 9)
            {
                if (currentRegion == Regions.Top)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(2.0f * negation, 2.0f), this);
                else if (currentRegion == Regions.Right)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(-2.0f, 2.0f * negation), this);
                else if (currentRegion == Regions.Bottom)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(2.0f * negation, -2.0f), this);
                else if (currentRegion == Regions.Left)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(2.0f, 2.0f * negation), this);
            }
            else
            {
                if (currentRegion == Regions.Top)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(3.0f * negation, 1.0f), this);
                else if (currentRegion == Regions.Right)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(-1.0f, 3.0f * negation), this);
                else if (currentRegion == Regions.Bottom)
                    chamber[0] = new DragonFire(mouth.X + (displacement * negation), mouth.Y, new Vector2(3.0f * negation, -1.0f), this);
                else if (currentRegion == Regions.Left)
                    chamber[0] = new DragonFire(mouth.X, mouth.Y + (displacement * negation), new Vector2(1.0f, 3.0f * negation), this);
            }

            //Cancel if the dragon has been defeated.
            if (remove)
                chamber = null;
        }

        private void CalculateMarkers()
        {
            markerOpenGate = (uint)(OPEN_GATE_MARKER * attackCycleDuration);
            markerEmerge = (uint)(EMERGE_MARKER * attackCycleDuration);
            markerOpenMouth = (uint)(OPEN_MOUTH_MARKER * attackCycleDuration);
            markerBreatheFire = (uint)(BREATHE_FIRE_MARKER * attackCycleDuration);
            markerWithdrawFire = (uint)(WITHDRAW_FIRE_MARKER * attackCycleDuration);
            markerCloseMouth = (uint)(CLOSE_MOUTH_MARKER * attackCycleDuration);
            markerRecede = (uint)(RECEDE_MARKER * attackCycleDuration);
            markerCloseGate = (uint)(CLOSE_GATE_MARKER * attackCycleDuration);

            float fireInterval = (BREATHE_FIRE_MARKER - WITHDRAW_FIRE_MARKER) / NUM_FLAMES;
            for (int i = 0; i < markerFire.Length; i++)
                markerFire[i] = markerBreatheFire - ((uint)i * (uint)(fireInterval * attackCycleDuration));

            System.Diagnostics.Debug.WriteLine(//
                "fire interval: " + fireInterval + "\n" +//
                "marker -- breathe fire: " + markerBreatheFire + "\n" +//
                "marker -- withdraw fire: " + markerWithdrawFire);//
            for (int i = 0; i < markerFire.Length; i++)//
                System.Diagnostics.Debug.WriteLine("marker -- fire " + i + ": " + markerFire[i]);//

            width = Game1.playfield.X;
            height = Game1.playfield.Y;
        }

        public override void Update()
        {
            //System.Diagnostics.Debug.WriteLine(//
            //    "CurrentState: " + _head[(int)currentRegion].CurrentState + "\n" +
            //    "attackCycleCount: " + attackCycleCount + "\n" +
            //    "TileSelectionX: " + _head[(int)currentRegion].TileSelectionX + "\n" +
            //    "TileSelectionY: " + _head[(int)currentRegion].TileSelectionY + "\n");//

            if (attackCycleCount == markerOpenGate)
            {
                _gate[(int)currentRegion].Open();
            }
            else if (attackCycleCount == markerEmerge)
            {
                _head[(int)currentRegion].CurrentState = DragonHead.State.Emerging;
            }
            else if (attackCycleCount == markerOpenMouth)
            {
                _head[(int)currentRegion].CurrentState = DragonHead.State.OpeningMouth;
            }
            else if (attackCycleCount <= markerBreatheFire && attackCycleCount > markerWithdrawFire)
            {
                foreach (uint i in markerFire)
                    if (attackCycleCount == i)
                        Fire();
            }
            else if (attackCycleCount == markerCloseMouth)
            {
                _head[(int)currentRegion].CurrentState = DragonHead.State.ClosingMouth;
            }
            else if (attackCycleCount == markerRecede)
            {
                _head[(int)currentRegion].CurrentState = DragonHead.State.Receding;
            }
            else if (attackCycleCount == markerCloseGate)
            {
                _gate[(int)currentRegion].Close();
            }

            if (--attackCycleCount == 0)
            {
                if (struck)
                {
                    attackCycleDuration -= ATTACK_CYCLE_DECREMENT;
                    CalculateMarkers();
                    struck = false;
                }
                attackCycleCount = attackCycleDuration;

                currentRegion = (Regions)Game1.rand.Next((int)Regions.EnumSize);

                for (int i = 0; i < (int)Regions.EnumSize; i++)
                    _head[i].Active = (i == (int)currentRegion) ? true : false;
            }

            if (flashCount > 0)
                flashCount--;

            if (flashCount > 0 && flashCount % 2 == 0)
                render = false;
            else render = true;

            base.Update();
        }
    }
}
