using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Arrow : SpritePlus, IProjectile, IOutlined
    {
        private const int SPEED = 6;

        private static readonly Rectangle[] _arrowHitBox =
        {
            new Rectangle(6, 1, 5, Game1.TILE_SIZE - 2)
        };

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }
        public bool ContactWithWall { get; set; }
        public bool ContactWithPlayer { get; set; }

        public Arrow(int x, int y, bool upward, IGameObject parent)
            : base(
                  new SpriteInfo(
                      Images.ARROW, 
                      x - (Images.ARROW.Width / 2), 
                      y - (Images.ARROW.Height / 2), 
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_arrowHitBox, null))
        {
            _parent = parent;
            _outlineMask = new OutlineMask(upward ? Images.ARROW_OUTLINE_UP : Images.ARROW_OUTLINE_DOWN, this);

            if (!upward)
            {
                orientation = SpriteEffects.FlipVertically;
                velocity.Y = SPEED;
            }
            else velocity.Y = -SPEED;
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, 0, 0); }
    }
}
