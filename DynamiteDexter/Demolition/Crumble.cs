using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Crumble : SpriteSheet, IExpires
    {
        private const int START_TIME = 6;

        private int count;

        public bool TimeUp { get { return count <= 0; } }
        public bool Strike { get { return count == START_TIME - 1; } }
        public int Count { get { return count; } }

        public Crumble(IGameObject source)
            : base(
                  new SpriteInfo(Images.CRUMBLE, source.X, source.Y, layer: (int)Game1.Layers.Terrain),
                  new CollisionInfo(null, null),
                  new AnimationInfo(6, 1, 0))
        { count = START_TIME; }

        protected override void Animate()
        {
            tileSelection.X = START_TIME - count;
            SetFrame();
        }

        public override void Update()
        {
            count--;
            base.Update();
        }
    }
}
