using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class PillBox : Terrain, IFires
    {
        private const int INTERVAL = 4 * Game1.SYNC_QUARTER;
        private const float PROJECTILE_SPEED = 4;
        private const float SINE_45 = 0.7071f;

        private IProjectile[] chamber;

        public IProjectile[] Chamber { get { return chamber; } }

        public PillBox(int x, int y)
            : base(Images.PILL_BOX, x, y, 2, 0)
        { }

        public void EmptyChamber() { chamber = null; }

        protected override void Animate()
        {
            if (Game1.universalTimer % INTERVAL > 2 * Game1.SYNC_QUARTER)
                tileSelection.X = 1;
            else tileSelection.X = 0;

            SetFrame();
        }

        public override void Update()
        {
            if (Game1.universalTimer % INTERVAL == 1 * Game1.SYNC_QUARTER)
            {
                chamber = new IProjectile[4];
                chamber[0] = new Bullet(Left, Center.Y, new Vector2(-PROJECTILE_SPEED, 0.0f), this);
                chamber[1] = new Bullet(Center.X, Top, new Vector2(0.0f, -PROJECTILE_SPEED), this);
                chamber[2] = new Bullet(Right, Center.Y, new Vector2(PROJECTILE_SPEED, 0.0f), this);
                chamber[3] = new Bullet(Center.X, Bottom, new Vector2(0.0f, PROJECTILE_SPEED), this);
            }
            else if (Game1.universalTimer % INTERVAL == 3 * Game1.SYNC_QUARTER)
            {
                chamber = new IProjectile[4];
                chamber[0] = new Bullet(Left, Top, new Vector2(SINE_45 * -PROJECTILE_SPEED, SINE_45 * -PROJECTILE_SPEED), this);
                chamber[1] = new Bullet(Right, Top, new Vector2(SINE_45 * PROJECTILE_SPEED, SINE_45 * -PROJECTILE_SPEED), this);
                chamber[2] = new Bullet(Left, Bottom, new Vector2(SINE_45 * -PROJECTILE_SPEED, SINE_45 * PROJECTILE_SPEED), this);
                chamber[3] = new Bullet(Right, Bottom, new Vector2(SINE_45 * PROJECTILE_SPEED, SINE_45 * PROJECTILE_SPEED), this);
            }

            base.Update();
        }
    }
}
