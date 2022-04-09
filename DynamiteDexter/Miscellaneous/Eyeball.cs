using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Eyeball : Breakable, IHunts, IFires
    {
        const float PROJECTILE_SPEED = 2.0f;
        const float DIAGONAL_RATIO = 0.7071f; //Adjacent divided by hypotenuse at 45 degrees.

        private bool fires;
        private int? fixedPos;
        private IGameObject _target;
        private IProjectile[] chamber;

        public IGameObject Target { get { return _target; } }
        public IProjectile[] Chamber { get { return chamber; } }

        public Eyeball(int x, int y, IGameObject target, bool fires = false, int? fixedPos = null)
            : base(Images.EYEBALL, x, y, 8, 0)
        {
            _target = target;
            this.fires = fires;
            this.fixedPos = fixedPos;
        }

        public void EmptyChamber() { chamber = null; }

        protected override void Animate()
        {
            if (fixedPos != null && !fires)

                tileSelection.X = (int)fixedPos;

            else if (_target != null)
            {
                //Note - Results are in radians.

                double relativeX = _target.Center.X - Center.X;
                double relativeY = _target.Center.Y - Center.Y;

                relativeY = -relativeY; //Account for inverted y-axis.

                double theta;

                if (relativeX < 0.0)
                    theta = Math.PI + Math.Atan(relativeY / relativeX); //Determine angle from [x, y] displacement.
                else theta = Math.Atan(relativeY / relativeX);

                theta = theta - (2 * Math.PI) * Math.Floor(theta / (2 * Math.PI)); //Reduce to coterminal.

                double angleToFrame = (SheetDimensions.X * theta) / (2 * Math.PI);
                tileSelection.X = (int)(angleToFrame + 0.5); //Round to nearest integer.

                if (tileSelection.X >= sheetDimensions.X) tileSelection.X = 0;
            }

            SetFrame();
        }

        public override void Update()
        {
            if (fires && _target != null && Game1.rand.Next(240) == 0)
            {
                float projectileVelocityX = 0.0f;
                float projectileVelocityY = 0.0f;

                switch (tileSelection.X)
                {
                    case 0:
                        projectileVelocityX = PROJECTILE_SPEED;
                        projectileVelocityY = 0.0f;
                        break;
                    case 1:
                        projectileVelocityX = PROJECTILE_SPEED * DIAGONAL_RATIO;
                        projectileVelocityY = PROJECTILE_SPEED * DIAGONAL_RATIO * -1;
                        break;
                    case 2:
                        projectileVelocityX = 0.0f;
                        projectileVelocityY = -PROJECTILE_SPEED;
                        break;
                    case 3:
                        projectileVelocityX = PROJECTILE_SPEED * DIAGONAL_RATIO * -1;
                        projectileVelocityY = PROJECTILE_SPEED * DIAGONAL_RATIO * -1;
                        break;
                    case 4:
                        projectileVelocityX = -PROJECTILE_SPEED;
                        projectileVelocityY = 0.0f;
                        break;
                    case 5:
                        projectileVelocityX = PROJECTILE_SPEED * DIAGONAL_RATIO * -1;
                        projectileVelocityY = PROJECTILE_SPEED * DIAGONAL_RATIO;
                        break;
                    case 6:
                        projectileVelocityX = 0.0f;
                        projectileVelocityY = PROJECTILE_SPEED;
                        break;
                    case 7:
                        projectileVelocityX = PROJECTILE_SPEED * DIAGONAL_RATIO;
                        projectileVelocityY = PROJECTILE_SPEED * DIAGONAL_RATIO;
                        break;
                }

                chamber = new IProjectile[1];
                chamber[0] = new Bullet(Center.X, Center.Y, new Vector2(projectileVelocityX, projectileVelocityY), this);
            }

            base.Update();
        }
    }
}
