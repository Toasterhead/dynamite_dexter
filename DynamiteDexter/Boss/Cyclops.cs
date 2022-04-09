using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Cyclops : SpritePlus, IBoss, IHunts, IHasAttachments
    {
        private class CyclopsEye : SpriteSheet
        {
            public CyclopsEye(int x, int y)
                : base(
                      new SpriteInfo(
                          Images.CYCLOPS_EYEBALL,
                          x - (Game1.TILE_SIZE / 2),
                          y - (Game1.TILE_SIZE / 2),
                          (int)Game1.Layers.Overhead),
                      new CollisionInfo(null, null),
                      new AnimationInfo(8, 1, 0))
            { }

            public void DetermineFrame(IGameObject target)
            {
                if (target != null)
                {
                    //Note - Results are in radians.

                    double relativeX = target.Center.X - Center.X;
                    double relativeY = target.Center.Y - Center.Y;

                    relativeY = -relativeY; //Account for inverted y-axis.

                    double theta;

                    if (relativeX < 0.0)
                        theta = Math.PI + Math.Atan(relativeY / relativeX); //Determine angle from [x, y] displacement.
                    else theta = Math.Atan(relativeY / relativeX);

                    theta = theta - (2 * Math.PI) * Math.Floor(theta / (2 * Math.PI)); //Reduce to coterminal.

                    double angleToFrame = (SheetDimensions.X * theta) / (2 * Math.PI);
                    tileSelection.X = (int)(angleToFrame + 0.5); //Round to nearest integer.

                    if (tileSelection.X >= sheetDimensions.X) tileSelection.X = 0;
                }
            }

            protected override void Animate() { SetFrame(); }
        }

        private const int START_X = 3 * Game1.TILE_SIZE;
        private const int START_Y = 2 * Game1.TILE_SIZE;
        private const int SPEED = 2;
        private const int BOUNDARY_HORIZONTAL = 1 * Game1.TILE_SIZE;
        private const int BOUNDARY_VERTICAL = 2 * Game1.TILE_SIZE;
        private const int INITIAL_VITALITY = 3;
        private const uint FLASH_DURATION = 30;

        private static readonly Rectangle[] _cyclopsHitBox =
        {
            new Rectangle(3, 3, Images.CYCLOPS.Width - 6, Images.CYCLOPS.Height - 3)
        };
        private static readonly Sprite _appearingForm = new Sprite(
            Images.APPEARING_CYCLOPS, 
            START_X, 
            START_Y, 
            (int)Game1.Layers.Actor);

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly CyclopsEye _eye;

        private int vitality;
        private uint flashCount;

        public int StartX { get { return START_X; } }
        public int StartY { get { return START_Y; } }
        public int Vitality { get { return vitality; } }
        public uint FlashCount { get { return flashCount; } }
        public bool Flashing { get { return flashCount % 2 != 0; } }
        public Rectangle HitBoxAssault { get { return Rect; } }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public IGameObject Target { get { return _target; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_eye as SpriteSheet);

                return attachments;
            }
        }

        public Cyclops(IGameObjectPlus target)
            : base(
                  new SpriteInfo(Images.CYCLOPS, START_X, START_Y, (int)Game1.Layers.Actor),
                  new CollisionInfo(_cyclopsHitBox, null))
        {
            _startX = START_X;
            _startY = START_Y;
            _target = target;
            _eye = new CyclopsEye(_startX + (Width / 2), _startY + (Height / 2));
            velocity = new Vector2(SPEED, SPEED);
            vitality = INITIAL_VITALITY;
            flashCount = 0;
        }

        public void Strike()
        {
            if (--vitality == 0)
                remove = true;

            flashCount = FLASH_DURATION;
        }

        public void Reset()
        {
            x = START_X;
            y = START_Y;
            flashCount = 0;
        }

        public override void Update()
        {
            if (Left < BOUNDARY_HORIZONTAL || Right >= Game1.playfield.X - BOUNDARY_HORIZONTAL)
                velocity.X = -velocity.X;
            if (Top < BOUNDARY_VERTICAL || Bottom >= Game1.playfield.Y - BOUNDARY_VERTICAL)
                velocity.Y = -velocity.Y;

            if (flashCount > 0)
                flashCount--;

            if (flashCount > 0 && flashCount % 2 == 0)
            {
                render = false;
                _eye.Render = false;
            }
            else
            {
                render = true;
                _eye.Render = true;
            }

            base.Update();

            _eye.Reposition(Center.X - (_eye.Width / 2), Center.Y - (_eye.Height / 2));
            _eye.DetermineFrame(_target);
        }
    }
}
