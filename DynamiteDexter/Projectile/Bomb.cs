namespace DynamiteDexter
{
    public class Bomb : GravityArc, IProjectile, IOutlined
    {
        private const int SPEED = 3;

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public bool ContactWithWall { get { return false; } set { } }
        public bool ContactWithPlayer { get { return false; } set { } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }

        public Bomb(int x, int y, bool rightward, int initialT, IGameObject parent)
            : base(
                  new SpriteSheet(
                      Images.BOMB, 
                      x - (Game1.TILE_SIZE / 2), 
                      y - (Game1.TILE_SIZE / 2), 
                      2, 
                      1, 
                      (int)Game1.Layers.Overhead, 2),
                  x, 
                  y, 
                  rightward ? SPEED : -SPEED, initialT)
        {
            _parent = parent;
            _outlineMask = new OutlineMask(Images.BOMB_OUTLINE, Subject as SpriteSheet);
        }

        public void SyncOutline()
        {
            SpriteSheet subjectSheet = Subject as SpriteSheet;

            int tileSelectionX = subjectSheet.SourceRect.X / subjectSheet.Width;

            _outlineMask.Sync(Subject.X, Subject.Y, tileSelectionX, 0);
        }

        public override IGameObject GetImpact() { return new Explosion(this); }
    }
}
