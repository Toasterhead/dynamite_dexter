using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class SurpriseFlame : GravityArc, IProjectile, IHasAttachments
    {
        private const int SPEED = 2;

        private readonly IGameObject _parent;

        public bool ContactWithWall { get { return false; } set { } }
        public bool ContactWithPlayer { get { return false; } set { } }
        public IGameObject Parent { get { return _parent; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(Subject);

                return attachments;
            }
        }

        public SurpriseFlame(int x, int y, int initialT, IGameObject target, IGameObject parent)
            : base(
                  new SpriteSheet(Images.LIVE_FLAME, x, y, 4, 2, (int)Game1.Layers.Overhead, 2),
                  x, 
                  y, 
                  SPEED, initialT)
        {
            _parent = parent;
            Subject.Reposition(X, Y);
        }

        public override IGameObject GetImpact() { return new LiveFlame(X, Y - (Game1.TILE_SIZE / 4)); }
    }
}
