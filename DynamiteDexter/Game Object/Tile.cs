using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Tile : IGameObject
    {
        #region Protected Data
        protected Texture2D image;
        protected Rectangle rect;
        protected bool render = true;
        protected bool remove = false;
        protected int layer;
        #endregion

        protected int gridX;
        protected int gridY;

        public static uint TileSize = 50;

        #region Public Accessors
        public Texture2D Image { get { return image; } }
        public Rectangle Rect
        {
            //Returns copy of 'rect' to prevent original from being modified.
            get { return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height); }
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

        public int GridX
        {
            get { return gridX; }
            set
            {
                gridX = value;
                rect.X = value * (int)TileSize;
            }
        }
        public int GridY
        {
            get { return gridY; }
            set
            {
                gridY = value;
                rect.Y = value * (int)TileSize;
            }
        }
        public int GridWidth { get { return 1; } }
        public int GridHeight { get { return 1; } }

        public int X { get { return rect.X; } }
        public int Y { get { return rect.Y; } }
        public int Z { get { return layer; } }
        public int Width { get { return rect.Width; } }
        public int Height { get { return rect.Height; } }

        public int Left { get { return rect.X; } }
        public int Right { get { return rect.X + rect.Width; } }
        public int Top { get { return rect.Y; } }
        public int Bottom { get { return rect.Y + rect.Height; } }
        public Point TopLeft { get { return new Point(rect.X, rect.Y); } }
        public Point TopRight { get { return new Point(rect.X + rect.Width, rect.Y); } }
        public Point BottomLeft { get { return new Point(rect.X, rect.Y + rect.Height); } }
        public Point BottomRight { get { return new Point(rect.X + rect.Width, rect.Y + rect.Height); } }
        public Point Center { get { return new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2)); } }
        #endregion

        #region Contructors

        public Tile(Texture2D image, int gridX, int gridY, int layer = 0)
        {
            this.image = image;
            this.layer = layer;
            this.gridX = gridX;
            this.gridY = gridY;
            rect = new Rectangle(gridX * (int)TileSize, gridY * (int)TileSize, (int)TileSize, (int)TileSize);
        }

        public Tile(SpriteInfo spriteInfo) //Untested.
        {
            image = spriteInfo.image;
            layer = spriteInfo.layer;
            gridX = spriteInfo.x;
            gridY = spriteInfo.y;
            rect = new Rectangle(gridX * (int)TileSize, gridY * (int)TileSize, (int)TileSize, (int)TileSize);
        }

        #endregion

        /// <summary>
        /// Moves the tile by the given number of spaces.
        /// </summary>
        public void Translate(int x, int y)
        {
            GridX += x;
            GridY += y;
        }

        /// <summary>
        /// Moves the tile to the given location on the grid.
        /// </summary>
        public void Reposition(int x, int y)
        {
            GridX = x;
            GridY = y;
        }

        public virtual void Update()
        {
            //
        }
    }
}
