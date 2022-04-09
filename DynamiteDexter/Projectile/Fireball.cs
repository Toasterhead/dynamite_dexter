using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Fireball : SpriteSheet, IProjectile, IOutlined
    {
        private const int SPEED = 4;

        private static readonly Rectangle[] _fireballHitBox =
        {
            new Rectangle(3, 7, 10, 8)
        };

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public bool ContactWithWall { get; set; }
        public bool ContactWithPlayer { get; set; }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }

        public Fireball(int x, int y, bool downward, IGameObject parent)
            : base(
                  new SpriteInfo(
                      Images.FIREBALL,
                      x - (Game1.TILE_SIZE / 2),
                      y - (Game1.TILE_SIZE / 2),
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_fireballHitBox, null),
                  new AnimationInfo(3, 1, 2))
        {
            _parent = parent;
            _outlineMask = new OutlineMask(downward ? Images.FIREBALL_OUTLINE : Images.FIREBALL_OUTLINE, this);

            if (!downward)
            {
                orientation = SpriteEffects.FlipVertically;
                velocity.Y = -SPEED;
            }
            else velocity.Y = SPEED;
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }
    }
}