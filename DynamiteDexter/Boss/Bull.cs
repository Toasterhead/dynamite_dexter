using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Bull : Pedestrian, IBoss, IHunts, IFires, IHasAttachments //Under construction.
    {
        private class Dust : SpritePlus
        {
            public Dust(int x, int y)
                : base(
                      new SpriteInfo(Images.DUST, x, y, (int)Game1.Layers.Actor),
                      new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                      new CollisionInfo(null, null)) { }

            public void AdjustOrientation(bool facingLeft)
            {
                orientation = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
        }

        private const int START_X = 9 * Game1.TILE_SIZE;
        private const int START_Y = (Game1.GRID_SIZE_Y / 2) * Game1.TILE_SIZE - (Game1.TILE_SIZE / 2);
        private const int VERTICAL_CHARGING_DISPLACEMENT = 4;
        private const int PACING_FRAME_INTERVAL = 9;
        private const int CHARGING_FRAME_INTERVAL = 2;
        private const int SPEED_PACING = 1;
        private const int SPEED_BOUNCING = 2;
        private const int SPEED_CHARGING = 4;
        private const int INITIAL_VITALITY = 3;
        private const uint FLASH_DURATION = 30;
        private const uint MOVEMENT_DURATION = 30;
        private const uint WIND_UP_DURATION = 30;
        private const uint THROW_INTERVAL = 40;
        private const uint AIMING_MARK = THROW_INTERVAL / 2;
        private const uint RELEASE_MARK = THROW_INTERVAL / 4;

        //Gravity Arc Constants
        private const int INITIAL_T = -7;
        private const float COEFFICIENT = 0.25f;
        private const float Y_INTERCEPT = 50.0f;

        private static readonly Point _relativeBombPosition = new Point(Game1.TILE_SIZE / 4, -Game1.TILE_SIZE);
        private static readonly Point _relativeDustPosition = new Point(2 * Game1.TILE_SIZE, Game1.TILE_SIZE);
        private static readonly Rectangle[] _bullHitBox =
        {
            new Rectangle(5, 1, 2 * Game1.TILE_SIZE - 10, 2 * Game1.TILE_SIZE - 2)
        };
        private static readonly Sprite _appearingForm = new Sprite(Images.APPEARING_BULL, START_X, START_Y, (int)Game1.Layers.Actor);

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly SpriteSheet _overheadBomb;
        private readonly Dust _dust;
        private readonly Sprite _shadow;

        private bool facingLeft;
        private bool bouncing;
        private int t;
        private int vitality;
        private uint flashCount;
        private uint movementCount;
        private uint throwCount;
        private uint? chargeCount;
        private IProjectile[] chamber;

        public bool Flashing { get { return flashCount % 2 != 0; } }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public int Vitality { get { return vitality; } }
        public uint FlashCount { get { return flashCount; } }
        public IGameObject Target { get { return _target; } }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public override Rectangle HitBoxAssault
        {
            get
            {
                if (bouncing)

                    return new Rectangle(0, 0, 0, 0);

                return base.HitBoxAssault;
            }
        }
        public override Rectangle HitBoxTerrain
        {
            get
            {
                if (bouncing)

                    return new Rectangle(-100, -100, 0, 0);

                return base.HitBoxTerrain;
            }
        }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_overheadBomb);
                attachments.Add(_dust);
                attachments.Add(_shadow);

                return attachments;
            }
        }

        public Bull(IGameObject target)
            : base(
                  new SpriteInfo(Images.BULL, START_X, START_Y, (int)Game1.Layers.Actor),
                  new CollisionInfo(_bullHitBox, null),
                  new AnimationInfo(3, 6, PACING_FRAME_INTERVAL),
                  speed: SPEED_PACING)
        {
            _startX = START_X;
            _startY = START_Y;
            _target = target;
            _overheadBomb = new SpriteSheet(
                new SpriteInfo(Images.BOMB, _startX + _relativeBombPosition.X, _startY + _relativeBombPosition.Y, (int)Game1.Layers.Actor),
                new CollisionInfo(null, null),
                new AnimationInfo(2, 1, 2));
            _dust = new Dust(_startX + _relativeDustPosition.X, _startY + _relativeDustPosition.Y);              
            _shadow = new Sprite(Images.CAST_SHADOW, Center.X - (Game1.TILE_SIZE / 2), _startY + (5 * (Game1.TILE_SIZE / 4)), (int)Game1.Layers.Shadow);
            facingLeft = true;
            bouncing = false;
            vitality = INITIAL_VITALITY;
            movementCount = MOVEMENT_DURATION;
            throwCount = THROW_INTERVAL;

            _overheadBomb.Render = false;
            _dust.Render = false;
            _shadow.Render = false;
        }

        public void EmptyChamber() { chamber = null; }

        public override void Reset()
        {
            x = _startX;
            y = _startY;
            facingLeft = true;
            vitality = INITIAL_VITALITY;
        }

        public void Strike()
        {
            if (--vitality == 0)
                remove = true;

            flashCount = FLASH_DURATION;
        }

        protected override void Animate()
        {
            _overheadBomb.Render = false;
            _dust.Render = false;
            _shadow.Render = false;

            if (bouncing)
            {
                tileSelection.X = 0;
                tileSelection.Y = facingLeft ? 1 : 4;

                _shadow.Render = true;
                _shadow.Reposition(Center.X - (Game1.TILE_SIZE / 2), _startY + (5 * (Game1.TILE_SIZE / 4)));
            }
            else if (chargeCount == null)
            {
                if (throwCount < RELEASE_MARK)
                    tileSelection.X = 2;
                else if (throwCount < AIMING_MARK)
                {
                    tileSelection.X = 1;

                    int bombPositionX =  
                        facingLeft ? 
                        (int)x + _relativeBombPosition.X : 
                        (int)x + Width - _relativeBombPosition.X - _overheadBomb.Width;

                    _overheadBomb.Render = true;
                    _overheadBomb.Reposition(bombPositionX, (int)y + _relativeBombPosition.Y);
                }
                else tileSelection.X = 0;

                if (++delayCount >= interval)
                {
                    delayCount = 0;

                    if (facingLeft)
                    { 
                        if (tileSelection.Y == 0)
                            tileSelection.Y = 1;
                        else tileSelection.Y = 0;
                    }
                    else
                    {
                        if (tileSelection.Y == 3)
                            tileSelection.Y = 4;
                        else tileSelection.Y = 3;
                    }
                }
            }
            else
            {
                if (delayCount++ >= CHARGING_FRAME_INTERVAL)
                {
                    delayCount = 0;

                    if (facingLeft)
                        tileSelection.Y = 2;
                    else tileSelection.Y = 5;

                    if (tileSelection.X == 0)
                        tileSelection.X = 1;
                    else tileSelection.X = 0;

                    if (Game1.rand.Next(2) == 0)
                    {
                        int dustPositionX = 
                            facingLeft ? 
                            (int)x + _relativeDustPosition.X : 
                            (int)x + Width - _relativeDustPosition.X - _dust.Width;
                        _dust.AdjustOrientation(facingLeft);

                        _dust.Render = true;
                        _dust.Reposition(dustPositionX, (int)y + _relativeDustPosition.Y);
                    }
                }
            }

            SetFrame();
        }

        private float F(int t)
        {
            return COEFFICIENT * (float)Math.Pow(t, 2) + Y_INTERCEPT;
        }

        public override void Update()
        {
            if (bouncing)
            {
                velocity = new Vector2(facingLeft ? SPEED_BOUNCING : -SPEED_BOUNCING, F(t) - F(t - 1));
                t++;

                if (y >= _startY)
                {
                    bouncing = false;
                    facingLeft = !facingLeft;
                    Neutral();
                    y = _startY;
                }
            }
            else if (chargeCount == null)
            {
                if (--movementCount == 0)
                {
                    movementCount = MOVEMENT_DURATION;

                    if (velocity.X < 0.0f)
                        MoveRight();
                    else if (velocity.X > 0.0f)
                        MoveLeft();
                    else if (facingLeft)
                        MoveRight();
                    else MoveLeft();
                }

                if (--throwCount == 0)
                    throwCount = THROW_INTERVAL;
                else if (throwCount == RELEASE_MARK)
                {
                    chamber = new IProjectile[1];
                    chamber[0] =
                        facingLeft ?
                        new Bomb(Left, Top + Game1.TILE_SIZE, false, -12, this) :
                        new Bomb(Right - 1, Top + Game1.TILE_SIZE, true, -12, this);
                }
            }
            else
            {
                if (chargeCount > 0)
                    chargeCount--;
                else velocity = facingLeft ? new Vector2(-SPEED_CHARGING, 0.0f) : new Vector2(SPEED_CHARGING, 0.0f);

                if (ContactWithTerrain)
                {
                    chargeCount = null;
                    bouncing = true;
                    t = INITIAL_T;
                    y -= VERTICAL_CHARGING_DISPLACEMENT;
                }
            }

            if (chargeCount == null && 
                _target.Center.Y >= Top && 
                _target.Center.Y < Bottom && 
                Game1.rand.Next(100) == 0)
            {
                chargeCount = WIND_UP_DURATION;
                movementCount = MOVEMENT_DURATION;
                throwCount = THROW_INTERVAL;
                velocity = new Vector2(0.0f, 0.0f);
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
