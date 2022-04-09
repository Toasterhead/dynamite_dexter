using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Door : Terrain
    {
        private Door otherHalf;

        public Door OtherHalf { get { return otherHalf; } }

        public Door(Texture2D image, int x, int y)
            : base(image, x, y)
        { otherHalf = null; }

        public void LinkTo(Door otherHalf) { this.otherHalf = otherHalf; }
    }
}
