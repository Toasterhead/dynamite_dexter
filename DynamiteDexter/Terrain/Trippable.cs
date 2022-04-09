using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
    
namespace DynamiteDexter
{
    public abstract class Trippable : Terrain
    {
        protected readonly Rectangle _tripZone;

        private bool tripped;

        public bool Tripped { get { return tripped; } }
        public Rectangle TripZone { get { return _tripZone; } }

        public Trippable(Texture2D image, int x, int y, int numFrames, uint interval, Rectangle tripZoneRelative)
            : base(image, x, y, numFrames, interval)
        {
            _tripZone = new Rectangle(X + tripZoneRelative.X, Y + tripZoneRelative.Y, tripZoneRelative.Width, tripZoneRelative.Height);
            tripped = false;
        }

        public void Trip() { tripped = true; }

        public virtual IGameObject[] GetItem(IGameObject target) { return null; }
    }
}
