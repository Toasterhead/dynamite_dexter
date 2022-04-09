using System;

namespace DynamiteDexter
{
    public class SkullWall : Terrain, IHunts
    {
        const int FRAMES_IN_ROTATION = 6;
        const int FRAME_NARROW_GAZE_UP = FRAMES_IN_ROTATION;
        const int FRAME_NARROW_GAZE_DOWN = FRAMES_IN_ROTATION + 1;

        private readonly IGameObject _target;

        public IGameObject Target { get { return _target; } }

        public SkullWall(int x, int y, IGameObject target)
            : base(Images.WALL_SKULL, x, y, 8, 0) { _target = target; }

        protected override void Animate()
        {
            if (_target != null)
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

                double angleToFrame = (FRAMES_IN_ROTATION * theta) / (2 * Math.PI);
                tileSelection.X = (int)(angleToFrame + 0.5); //Round to nearest integer.

                if (tileSelection.X >= FRAMES_IN_ROTATION) tileSelection.X = 0;

                if ((tileSelection.X == 1 || tileSelection.X == 4) &&
                    Math.Abs(relativeY) < 2 * Game1.TILE_SIZE &&
                    Math.Abs(relativeX) < (Game1.TILE_SIZE / 2))

                    tileSelection.X = tileSelection.X == 1 ? FRAME_NARROW_GAZE_UP : FRAME_NARROW_GAZE_DOWN;
            }

            SetFrame();
        }
    }
}
