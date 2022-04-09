using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public struct RenderInfo
    {
        public readonly Texture2D image;
        public readonly Rectangle rect;
        public readonly Color color;

        public RenderInfo(Texture2D image, Rectangle rect, Color color)
        {
            this.image = image;
            this.rect = rect;
            this.color = color;
        }
    }

    public struct CollisionInfo
    {
        public readonly Rectangle[] hitBox;
        public readonly Point[] collisionPoint;

        public CollisionInfo(Rectangle[] hitBox, Point[] collisionPoint)
        {
            this.hitBox = hitBox;
            this.collisionPoint = collisionPoint;
        }
    }

    public struct AnimationInfo
    {
        public readonly int sheetX;
        public readonly int sheetY;
        public readonly uint interval;
        public readonly bool timeSync;

        public AnimationInfo(int sheetX, int sheetY, uint interval, bool timeSync = false)
        {
            this.sheetX = sheetX;
            this.sheetY = sheetY;
            this.interval = interval;
            this.timeSync = timeSync;
        }
    }

    public struct SpriteInfo
    {
        public readonly Texture2D image;
        public readonly int x;
        public readonly int y;
        public readonly int layer;

        public SpriteInfo(Texture2D image, int x, int y, int layer = 0)
        {
            this.image = image;
            this.x = x;
            this.y = y;
            this.layer = layer;
        }
    }

    public struct SpriteExtraInfo
    {
        public readonly Texture2D[] imageSet;
        public readonly Color color;
        public readonly SpriteEffects orientation;
        public readonly float alpha;

        public SpriteExtraInfo(Texture2D[] imageSet, Color color, SpriteEffects orientation, float alpha = 1.0f)
        {
            this.imageSet = imageSet;
            this.color = color;
            this.orientation = orientation;
            this.alpha = alpha;
        }

        public SpriteExtraInfo(float alpha = 1.0f)
        {
            imageSet = null;
            color = Color.White;
            orientation = SpriteEffects.None;
            this.alpha = alpha;
        }
    }
}

