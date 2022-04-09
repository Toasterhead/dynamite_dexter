using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Angel : SpriteSheet, IFires, IHasAttachments
    {
        private class SympathyStick : GravityArc, IProjectile
        {
            private readonly IGameObject _parent;

            public bool ContactWithWall { get { return false; } set { } }
            public bool ContactWithPlayer { get { return false; } set { } }
            public IGameObject Parent { get { return _parent; } }

            public SympathyStick(int x, int y, IGameObject parent)
                : base(
                      new Sprite(
                      Images.SYMPATHY_STICK,
                      x - (Game1.TILE_SIZE / 2),
                      y - (Game1.TILE_SIZE / 2),
                      (int)Game1.Layers.Overhead),
                  x,
                  y,
                  1,
                  -10)
            { _parent = parent; }

            public override IGameObject GetImpact()
            {
                return new DynamiteSingleStick((int)(x / Game1.TILE_SIZE + 0.5f), (int)(y / Game1.TILE_SIZE + 0.5f));
            }
        }

        private const int SPEED = 2;

        private readonly int _horizontalDropOff;
        private readonly Sprite _dynamiteStick;
        private readonly Sprite _shadow;

        private bool carryingDynamite;
        private IProjectile[] chamber;

        public IProjectile[] Chamber { get { return chamber; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_dynamiteStick);
                attachments.Add(_shadow);

                return attachments;
            }
        }

        public Angel(int horizontalDropOff, int verticalPosition)
            : base(
                  new SpriteInfo(
                      Images.ANGEL, 
                      Game1.playfield.X + (3 * Game1.TILE_SIZE), 
                      verticalPosition, 
                      (int)Game1.Layers.Overhead),
                  new CollisionInfo(null, null),
                  new AnimationInfo(2, 1, 5))
        {
            _horizontalDropOff = horizontalDropOff;
            _dynamiteStick = new Sprite(Images.SYMPATHY_STICK, X - Game1.TILE_SIZE, Y + Game1.TILE_SIZE, (int)Game1.Layers.Overhead);
            _shadow = new Sprite(Images.CAST_SHADOW, Center.X - (Game1.TILE_SIZE / 2), Bottom - 1, (int)Game1.Layers.Shadow);
            carryingDynamite = true;
            velocity = new Vector2(-SPEED, 0.0f);
        }

        public void EmptyChamber() { chamber = null; }

        public override void Update()
        {
            if (carryingDynamite && X < _horizontalDropOff)
            {
                carryingDynamite = false;
                _dynamiteStick.Render = false;
                chamber = new IProjectile[1];
                chamber[0] = new SympathyStick(X - Game1.TILE_SIZE, Y + Game1.TILE_SIZE, this);
            }

            base.Update();

            _dynamiteStick.Reposition(Left - Game1.TILE_SIZE, Top);
            _shadow.Reposition(Center.X - (Game1.TILE_SIZE / 2), Bottom - 1);
        }
    }
}
