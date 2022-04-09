using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Terrain : TileSheet
    {
        private readonly bool _animated;

        public Terrain(Texture2D image, int x, int y)
            : base(
                  new SpriteInfo(image, x, y, layer: (int)Game1.Layers.Terrain),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(null, null),
                  new AnimationInfo(1, 1, 0))
        { _animated = false; }

        public Terrain(Texture2D image, int x, int y, int numFrames, uint interval)
            : base(
                  new SpriteInfo(image, x, y, layer: (int)Game1.Layers.Terrain),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(null, null),
                  new AnimationInfo(numFrames, 1, interval))
        { _animated = true; }

        protected override void Animate() { if (_animated) base.Animate(); }
    }

    public class Breakable : Terrain
    {
        public Breakable(Texture2D image, int x, int y)
            : base(image, x, y) { }

        public Breakable(Texture2D image, int x, int y, int numFrames, uint interval)
            : base(image, x, y, numFrames, interval) { }
    }

    public class BreakablePrize : Breakable
    {
        public BreakablePrize(Texture2D image, int x, int y)
            : base(image, x, y) { }

        public BreakablePrize(Texture2D image, int x, int y, int numFrames, uint interval)
            : base(image, x, y, numFrames, interval) { }
    }

    public class Passable : Terrain
    {
        public Passable(Texture2D image, int x, int y)
            : base(image, x, y) { layer = (int)Game1.Layers.Passable; }
    }

    public class WallPrimary : Terrain
    {
        public WallPrimary(Texture2D image, int x, int y)
            : base(image, x, y) { }
    }

    public class WallSecondary : Terrain
    {
        public WallSecondary(Texture2D image, int x, int y)
            : base(image, x, y) { }
    }

    public class BreakableWallPrimary : Breakable
    {
        public BreakableWallPrimary(Texture2D image, int x, int y)
            : base(image, x, y) { }
    }

    public class BreakableWallSeconary : Breakable
    {
        public BreakableWallSeconary(Texture2D image, int x, int y)
            : base(image, x, y) { }
    }

    public class Water : Terrain
    {
        public Water(Texture2D image, int x, int y)
            : base(image, x, y) { }

        public Water(Texture2D image, int x, int y, int numFrames)
            : base(image, x, y, numFrames, 9) { }
    }

    public class Barrel : Breakable
    {
        public Barrel(int x, int y)
            : base(Images.BARREL, x, y) { }
    }

    public class Drum : Breakable
    {
        public Drum(int x, int y)
            : base(Images.DRUM, x, y) { }
    }

    public class Orb : Breakable
    {
        public Orb(int x, int y)
            : base(Images.ORB, x, y, 30, 3) { }
    }

    public class Casket : Breakable
    {
        public Casket(int x, int y, bool topHalf = true)
            : base(topHalf ? Images.CASKET_TOP : Images.CASKET_BOTTOM, x, y) { }
    }

    public class InvisibleWall : Terrain
    {
        public InvisibleWall(int x, int y)
            : base(Images.BLANK, x, y)
        { layer = (int)Game1.Layers.Floor; }
    }

    public class Plaque : Terrain
    {
        public Plaque(Texture2D image, int x, int y)
            : base(image, x, y) { }
    }

    public class WorldMap : Terrain
    {
        public WorldMap(int x, int y)
            : base(Images.MAP, x, y)  { }
    }
}
