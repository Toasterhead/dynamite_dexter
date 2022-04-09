using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Bullet : SpriteSheet, IProjectile, IOutlined
    {
        private static readonly Rectangle[] _bulletHitBox =
        {
            new Rectangle(8, 8, 8, 8)
        };

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public bool ContactWithWall { get; set; }
        public bool ContactWithPlayer { get; set; }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }

        public Bullet(int x, int y, Vector2 velocity, IGameObject parent, bool alternateImage = false)
            : base(
                  new SpriteInfo(
                      alternateImage ? Images.ENERGY_BALL : Images.BULLET, 
                      x - (Game1.TILE_SIZE / 2), 
                      y - (Game1.TILE_SIZE / 2), 
                      (int)Game1.Layers.Actor),
                  new CollisionInfo(_bulletHitBox, null),
                  new AnimationInfo(4, 1, 1))
        {
            _parent = parent;
            _outlineMask = new OutlineMask(Images.BULLET_OUTLINE, this);
            this.velocity = velocity;
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }
    }
}
