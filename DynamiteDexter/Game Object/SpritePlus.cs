using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class SpritePlus : Sprite, IGameObjectPlus
    {
        #region Immutable Data
        protected readonly Texture2D[] imageSet = null;
        protected readonly Rectangle[] hitBox = null;
        protected readonly Point[] collisionPoint = null;
        #endregion

        #region Protected Data
        protected Vector2 velocity = new Vector2(0.0f, 0.0f);
        protected float previousX = 0.0f;
        protected float previousY = 0.0f;
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

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public float PreviousX { get { return previousX; } }
        public float PreviousY { get { return previousY; } }
        public Point Previous { get { return new Point((int)previousX, (int)previousY); } }
        public float DisplacementX { get { return (int)x - (int)previousX; } }
        public float DisplacementY { get { return (int)y - (int)previousY; } }
        public Point Displacement { get { return new Point((int)x - (int)previousX, (int)y - (int)previousY); } }

        public Color TheColor { get { return color; } }
        public SpriteEffects Orientation { get { return orientation; } }
        public float Alpha
        {
            /*
                Note: 
                
                'color.A' appears to operate on a scale from 0.0f to 1.0f in the Color 
                constructor, but from 0.0f to 256.0f upon being read as a property.
            */

            get { return color.A / 256.0f; }
            set { color = new Color(color.R, color.G, color.B, value); }
        }

        public int NumHitBoxes
        {
            get
            {
                if (hitBox == null)
                    return 0;

                return hitBox.Length;
            }
        }
        public int NumCollisionPoints
        {
            get
            {
                if (collisionPoint == null)
                    return 0;

                return collisionPoint.Length;
            }
        }
        #endregion

        #region Constructor

        public SpritePlus(Texture2D image, int x, int y, int layer = 0)
            : base(image, x, y, layer)
        { }

        public SpritePlus(Texture2D[] imageSet, int x, int y, int layer = 0)
            : base(imageSet[0], x, y, layer)
        { }

        public SpritePlus(Texture2D image, int x, int y, Rectangle[] hitBox, int layer = 0)
            : base(image, x, y, layer)
        { this.hitBox = hitBox; }

        public SpritePlus(Texture2D image, int x, int y, Point[] collisionPoint, int layer = 0)
            : base(image, x, y, layer)
        { this.collisionPoint = collisionPoint; }

        public SpritePlus(
            int x,
            int y,
            Texture2D[] imageSet,
            Rectangle[] hitBox,
            Point[] collisionPoint,
            int layer = 0)
            : base(imageSet[0], x, y, layer)
        {
            this.imageSet = imageSet;
            this.hitBox = hitBox;
            this.collisionPoint = collisionPoint;
        }

        public SpritePlus(SpriteInfo spriteInfo, SpriteExtraInfo spriteExtraInfo, CollisionInfo collisionInfo)
            : base(spriteInfo.image, spriteInfo.x, spriteInfo.y, spriteInfo.layer)
        {
            imageSet = spriteExtraInfo.imageSet;
            hitBox = collisionInfo.hitBox;
            collisionPoint = collisionInfo.collisionPoint;
            color = spriteExtraInfo.color;
            orientation = spriteExtraInfo.orientation;
            Alpha = spriteExtraInfo.alpha;
        }

        public SpritePlus(SpriteInfo spriteInfo, CollisionInfo collisionInfo)
            : base(spriteInfo.image, spriteInfo.x, spriteInfo.y, spriteInfo.layer)
        {
            imageSet = null;
            hitBox = collisionInfo.hitBox;
            collisionPoint = collisionInfo.collisionPoint;
            color = Color.White;
            orientation = SpriteEffects.None;
            Alpha = 1.0f;
        }

        #endregion

        #region Scaling Methods

        /// <summary>
        /// Re-sizes the sprite by adding or subtracting from its dimensions.
        /// Returns 'false' and performs no operations if new dimensions would 
        /// be less than zero.
        /// 
        /// Setting 'fromCenter' to 'true' causes sprite to change size about
        /// the center rather than the upper-left corner.
        /// </summary>
        public bool Scale(int x, int y, bool fromCenter = false)
        {
            if (x < 0 && -x >= width) return false;
            if (y < 0 && -y >= height) return false;

            if (fromCenter)
            {
                Point centerTemp = new Point(Center.X, Center.Y);

                width += x;
                height += y;

                Center = centerTemp;
            }
            else
            {
                width += x;
                height += y;
            }

            return true;
        }

        /// <summary>
        /// Re-sizes the sprite by setting new dimensions.
        /// Returns 'false' and performs no operations if new dimensions are
        /// less than zero.
        /// 
        /// Setting 'fromCenter' to 'true' causes sprite to change size about
        /// the center rather than the upper-left corner.
        /// </summary>
        public bool ReScale(int width, int height, bool fromCenter = false)
        {
            if (width < 0) return false;
            if (height < 0) return false;

            if (fromCenter)
            {
                Point centerTemp = new Point(Center.X, Center.Y);

                this.width = width;
                this.height = height;

                Center = centerTemp;
            }
            else
            {
                this.width = width;
                this.height = height;
            }

            return true;
        }

        #endregion

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

        public int HitBoxTop(int i) { return hitBox[i].Top; } //Untested.

        public int HitBoxRight(int i) { return hitBox[i].Right; } //Untested.

        public int HitBoxBottom(int i) { return hitBox[i].Bottom; } //Untested.

        public int HitBoxLeft(int i) { return hitBox[i].Left; } //Untested.

        public void SetHitBoxTop(int i, int newValue, bool absolute = false)
        {
            if (absolute)
            {
                int displacementY = newValue - GetHitBox(i).Top;
                y += displacementY;
            }
            else hitBox[i].Y = newValue; //Untested.
        }

        public void SetHitBoxRight(int i, int newValue, bool absolute = false)
        {
            if (absolute)
            {
                int displacementX = newValue - GetHitBox(i).Right;
                x += displacementX;
            }
            else hitBox[i].X = newValue - hitBox[i].Width; //Untested.
        }

        public void SetHitBoxBottom(int i, int newValue, bool absolute = false)
        {
            if (absolute)
            {
                int displacementY = newValue - GetHitBox(i).Bottom;
                y += displacementY;
            }
            else hitBox[i].Y = newValue - hitBox[i].Height; //Untested.
        }

        public void SetHitBoxLeft(int i, int newValue, bool absolute = false)
        {
            if (absolute)
            {
                int displacementX = newValue - GetHitBox(i).Left;
                x += displacementX;
            }
            else hitBox[i].X = newValue; //Untested.
        }

        #endregion

        #region Collision Point Methods

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

        public void SetCollisionPoint(int i, Point newValue, bool absolute = false)
        {
            if (absolute)
            {
                int displacementX = newValue.X - (collisionPoint[i].X + X);
                int displacementY = newValue.Y - (collisionPoint[i].Y + Y);

                x += displacementX;
                y += displacementY;
            }
            else collisionPoint[i] = newValue;
        }

        #endregion

        public void SetImage(int i) { image = imageSet[i]; }

        public override void Update()
        {
            UpdatePosition();
        }

        protected virtual void UpdatePosition()
        {
            previousX = x;
            previousY = y;

            x += velocity.X;
            y += velocity.Y;
        }
    }
}

