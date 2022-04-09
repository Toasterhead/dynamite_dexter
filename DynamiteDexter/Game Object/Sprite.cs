using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Sprite : IGameObject
    {
        #region Protected Data
        protected Texture2D image;
        protected float x, y;
        protected int width, height;
        protected bool render = true;
        protected bool remove = false;
        protected int layer;
        #endregion

        #region Public Accessors
        public Texture2D Image { get { return image; } }
        public Rectangle Rect
        {
            get { return new Rectangle((int)x, (int)y, width, height); }
        }
        public bool Render
        {
            get { return render; }
            set { render = value; }
        }
        public bool Remove
        {
            get { return remove; }
            set { remove = value; }
        }
        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public int X { get { return (int)x; } }
        public int Y { get { return (int)y; } }
        public int Z { get { return layer; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public float Xf { get { return x; } }
        public float Yf { get { return y; } }

        public int Left
        {
            get { return (int)x; }
            set { x = value; }
        }
        public int Right
        {
            get { return (int)x + width; }
            set { x = value - width; }
        }
        public int Top
        {
            get { return (int)y; }
            set { y = value; }
        }
        public int Bottom
        {
            get { return (int)y + height; }
            set { y = value - height; }
        }
        public Point TopLeft
        {
            get
            {
                return new Point((int)x, (int)y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }
        public Point TopRight
        {
            get
            {
                return new Point((int)x + width, (int)y);
            }
            set
            {
                x = value.X - width;
                y = value.Y;
            }
        }
        public Point BottomLeft
        {
            get
            {
                return new Point((int)x, (int)y + height);
            }
            set
            {
                x = value.X;
                y = value.Y - height;
            }
        }
        public Point BottomRight
        {
            get
            {
                return new Point((int)x + width, (int)y + height);
            }
            set
            {
                x = value.X - width;
                y = value.Y - height;
            }
        }
        public Point Center
        {
            get
            {
                return new Point((int)x + (width / 2), (int)y + (height / 2));
            }
            set
            {
                x = value.X - (width / 2);
                y = value.Y - (height / 2);
            }
        }
        #endregion

        #region Contructors

        public Sprite(Texture2D image, int x, int y, int layer = 0)
        {
            this.image = image;
            this.layer = layer;
            this.x = x;
            this.y = y;
            width = image.Width;
            height = image.Height;
        }

        public Sprite(SpriteInfo spriteInfo)
        {
            image = spriteInfo.image;
            layer = spriteInfo.layer;
            x = spriteInfo.x;
            y = spriteInfo.y;
            width = spriteInfo.image.Width;
            height = spriteInfo.image.Height;
        }

        #endregion

        /// <summary>
        /// Moves the sprite by the given amount.
        /// </summary>
        public virtual void Translate(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        /// <summary>
        /// Moves the sprite to the given location.
        /// </summary>
        public virtual void Reposition(int x, int y, bool fromCenter)
        {
            this.x = x;
            this.y = y;

            if (fromCenter)
            {
                this.x -= width / 2;
                this.y -= height / 2;
            }
        }

        public virtual void Reposition(int x, int y) { Reposition(x, y, fromCenter: false); }

        public virtual void Update()
        {
            //
        }
    }
}

