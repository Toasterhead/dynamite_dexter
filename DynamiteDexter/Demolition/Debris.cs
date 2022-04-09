﻿namespace DynamiteDexter
{
    public class Debris : SpriteSheet, IExpires
    {
        private const int START_TIME = 24;

        private int count;

        public bool TimeUp { get { return count <= 0; } }
        public bool Strike { get { return count == START_TIME - 1; } }
        public int Count { get { return count; } }

        public Debris(IGameObject source)
            : base(
                  new SpriteInfo(
                      Images.DEBRIS,
                      source.Center.X - (Images.EXPLOSION.Height / 2),
                      source.Center.Y - (Images.EXPLOSION.Height / 2),
                      layer: (int)Game1.Layers.Passable),
                  new CollisionInfo(null, null),
                  new AnimationInfo(8, 1, 0))
        { count = START_TIME; }

        protected override void Animate()
        {
            tileSelection.X = (START_TIME - count) / 2;
            SetFrame();
        }

        public override void Update()
        {
            count--;
            base.Update();
        }
    }
}
