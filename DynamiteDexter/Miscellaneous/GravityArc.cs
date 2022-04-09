using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class GravityArc : SpritePlus
    {
        private const float Y_INTERCEPT = 50.0f;
        private const float COEFFICIENT = 0.25f;

        private readonly Sprite _subject;

        private int t;

        public Sprite Subject { get { return _subject; } }

        public GravityArc(Sprite subject, int x, int y, int horizontalVelocity, int tInitial)
            : base(
                  new SpriteInfo(
                      Images.CAST_SHADOW,
                      x - (Game1.TILE_SIZE / 2),
                      y - (Game1.TILE_SIZE / 2),
                      layer: (int)Game1.Layers.Shadow),
                  new CollisionInfo(null, null))
        {
            t = tInitial;
            _subject = subject;
            _subject.Layer = (int)Game1.Layers.Overhead;
            velocity = new Vector2(horizontalVelocity, 0.0f);
        }

        private float F(int t) { return -COEFFICIENT * (float)Math.Pow(t, 2) + Y_INTERCEPT; }

        public virtual IGameObject GetImpact() { return null; }

        public override void Update()
        {
            _subject.Reposition(Center.X - (_subject.Width / 2), (int)(y - F(t++)));

            if (t >= 0 && _subject.Bottom > Center.Y)
                remove = true;

            base.Update();
        }
    }
}
