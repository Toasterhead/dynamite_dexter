using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class TilePlus : Tile, IGameObjectPlus
    {
        #region Immutable Data
        protected readonly Texture2D[] imageSet = null;
        protected readonly Rectangle[] hitBox = null;
        protected readonly Point[] collisionPoint = null;
        #endregion

        #region Protected Data
        protected Vector2 velocity = new Vector2(0.0f, 0.0f);
        protected Point previous = new Point(0, 0);
        protected Color color = Color.White;
        protected SpriteEffects orientation = SpriteEffects.None;
        #endregion

        #region Public Accessors
        public Texture2D[] ImageSet { get { return imageSet; } }
        public Rectangle[] HitBox
        {
            get
            {
                if (hitBox == null)
                    return null;

                Rectangle[] absoluteHitBox = new Rectangle[hitBox.Length];
                for (int i = 0; i < hitBox.Length; i++)
                    absoluteHitBox[i] = new Rectangle(X + hitBox[i].X, Y + hitBox[i].Y, hitBox[i].Width, hitBox[i].Height);

                return absoluteHitBox;
            }
        }
        public Point[] CollisionPoint
        {
            get
            {
                if (collisionPoint == null)
                    return null;

                Point[] absoluteCollisionPoint = new Point[collisionPoint.Length];
                for (int i = 0; i < collisionPoint.Length; i++)
                    absoluteCollisionPoint[i] = new Point(X + collisionPoint[i].X, Y + collisionPoint[i].Y);

                return absoluteCollisionPoint;
            }
        }
        public Point GridPrevious
        {
            get
            {
                return new Point(
                    previous.X != 0 ? previous.X / (int)Tile.TileSize : 0,
                    previous.Y != 0 ? previous.Y / (int)Tile.TileSize : 0);
            }
        }

        public Vector2 Velocity { get { return velocity; } }
        public Point Previous { get { return previous; } }
        public Color TheColor { get { return color; } }
        public SpriteEffects Orientation { get { return orientation; } }

        public float Alpha
        {
            get { return color.A; }
            set { color = new Color(color.R, color.G, color.B, value); }
        }

        public int NumHitBoxes { get { return hitBox.Length; } }
        public int NumCollisionPoints { get { return collisionPoint.Length; } }
        #endregion

        public TilePlus(Texture2D image, int gridX, int gridY, int layer = 0)
            : base(image, gridX, gridY, layer)
        { }

        public TilePlus(Texture2D[] imageSet, int gridX, int gridY, int layer = 0)
            : base(imageSet[0], gridX, gridY, layer)
        { }

        public TilePlus(Texture2D image, int gridX, int gridY, Rectangle[] hitBox, int layer = 0)
            : base(image, gridX, gridY, layer)
        { this.hitBox = hitBox; }

        public TilePlus(Texture2D image, int gridX, int gridY, Point[] collisionPoint, int layer = 0)
            : base(image, gridX, gridY, layer)
        { this.collisionPoint = collisionPoint; }

        public TilePlus(
            int gridX,
            int gridY,
            Texture2D[] imageSet,
            Rectangle[] hitBox,
            Point[] collisionPoint,
            int layer = 0)
            : base(imageSet[0], gridX, gridY, layer)
        {
            this.imageSet = imageSet;
            this.hitBox = hitBox;
            this.collisionPoint = collisionPoint;
        }

        public TilePlus(SpriteInfo spriteInfo, SpriteExtraInfo spriteExtraInfo, CollisionInfo collisionInfo) //Untested.
            : base(spriteInfo.image, spriteInfo.x, spriteInfo.y, spriteInfo.layer)
        {
            imageSet = spriteExtraInfo.imageSet;
            hitBox = collisionInfo.hitBox;
            collisionPoint = collisionInfo.collisionPoint;
            color = spriteExtraInfo.color;
            orientation = spriteExtraInfo.orientation;
            Alpha = spriteExtraInfo.alpha;
        }

        private void SetPrevious()
        {
            previous.X = rect.X;
            previous.Y = rect.Y;
        }

        #region Hit-box Methods

        /// <summary>
        /// Retrieves a copy of the indicated hit-box. The resulting coordinates 
        /// can be toggled between absolute or relative to those of the parent 
        /// sprite. 
        /// 
        /// For individual hit-boxes, this method is more efficient than the 
        /// HitBox accessor as it doesn't create an entirely new array per 
        /// invocation.
        /// </summary>
        public Rectangle GetHitBox(int i, bool absolute = true)
        {
            if (absolute)
                return new Rectangle(X + hitBox[i].X, Y + hitBox[i].Y, hitBox[i].Width, hitBox[i].Height);

            //'hitBox' is read-only. Is a copy necessary? Test later...
            return new Rectangle(hitBox[i].X, hitBox[i].Y, hitBox[i].Width, hitBox[i].Height);
        }

        public int HitBoxTop(int i) { return hitBox[i].Top; }

        public int HitBoxRight(int i) { return hitBox[i].Right; }

        public int HitBoxBottom(int i) { return hitBox[i].Bottom; }

        public int HitBoxLeft(int i) { return hitBox[i].Left; }

        public void SetHitBoxTop(int i, int newValue, bool absolute = false) { hitBox[i].Y = newValue; }

        public void SetHitBoxRight(int i, int newValue, bool absolute = false) { hitBox[i].X = newValue - hitBox[i].Width; }

        public void SetHitBoxBottom(int i, int newValue, bool absolute = false) { hitBox[i].Y = newValue - hitBox[i].Height; }

        public void SetHitBoxLeft(int i, int newValue, bool absolute = false) { hitBox[i].X = newValue; }

        #endregion

        /// <summary>
        /// Retrieves a copy of the indicated collision point. The resulting 
        /// coordinates can be toggled between absolute or relative to those 
        /// of the parent sprite.
        /// 
        /// For individual collision points, this method is more efficient than 
        /// the CollisionPoint accessor as it doesn't create an entirely new 
        /// array per invocation.
        /// </summary>
        public Point GetCollisionPoint(int i, bool absolute = true)
        {
            if (absolute)
                return new Point(X + collisionPoint[i].X, Y + collisionPoint[i].Y);

            //'collisionPoint' is read-only. Is a copy necessary? Test later...
            return new Point(collisionPoint[i].X, collisionPoint[i].Y);
        }
    }
}
