using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class OutlineMask : SpriteSheet
    {
        public OutlineMask(Texture2D image, SpriteSheet parent)
            : base(
                  new SpriteInfo(image, parent.X - 1, parent.Y - 1, layer: (int)Game1.Layers.Outline),
                  new CollisionInfo(null, null),
                  new AnimationInfo(parent.SheetDimensions.X, parent.SheetDimensions.Y, 0))
        {
            if (!(image.Width / sheetDimensions.X == (parent.Image.Width / sheetDimensions.X) + 2) &&
                !(image.Height / sheetDimensions.Y == (parent.Image.Height / sheetDimensions.Y) + 2))

                throw new System.Exception("Error - Outline mask must have dimensions that are two pixels greater than parent object.");
        }

        public OutlineMask(Texture2D image, Sprite parent)
            : base(
                  new SpriteInfo(image, parent.X - 1, parent.Y - 1, layer: (int)Game1.Layers.Outline),
                  new CollisionInfo(null, null),
                  new AnimationInfo(1, 1, 0))
        {
            if (!(image.Width == parent.Image.Width + 2 && image.Height == parent.Image.Height + 2))
                throw new System.Exception("Error - Outline mask must have dimensions that are two pixels greater than parent object.");
        }

        public void Sync(float parentX, float parentY, int frameX, int frameY)
        {
            x = parentX - 1;
            y = parentY - 1;
            tileSelection.X = frameX;
            tileSelection.Y = frameY;
        }

        protected override void Animate() { SetFrame(); }
    }
}
