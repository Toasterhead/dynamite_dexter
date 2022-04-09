using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Blade : Pedestrian, IHostile
    {
        private enum Phases { Rightward = 0, Downward, Leftward, Upward }
        private enum Species {
            Fixed,
            Rectangular,
            Reflected,
            ReflectedX,
            ReflectedY,
            Horizontal,
            Vertical,
            HorizontalReflected,
            VerticalReflected }

        private readonly int _startX;
        private readonly int _startY;
        private readonly int _stopRightward;
        private readonly int _stopDownward;
        private readonly int _stopLeftward;
        private readonly int _stopUpward;
        private readonly Species _species;

        private static Rectangle[] _bladeHitBox =
        {
            new Rectangle(2, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private Phases currentPhase;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }

        public Blade(int x, int y, Point path, bool highSpeed = false)
            : base(
                  new SpriteInfo(Images.BLADE, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_bladeHitBox, null),
                  new AnimationInfo(3, 1, 3),
                  speed: highSpeed ? 4 : 2)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;

            path = new Point(path.X * Game1.TILE_SIZE, path.Y * Game1.TILE_SIZE);

            if (path.X == 0 && path.Y == 0)
                _species = Species.Fixed;
            else if (path.X == 0 && path.Y < 0)
            {
                _species = Species.VerticalReflected;
                currentPhase = Phases.Upward;
                _stopDownward = _startY;
                _stopUpward = _startY + path.Y;
            }
            else if (path.X == 0)
            {
                _species = Species.Vertical;
                currentPhase = Phases.Downward;
                _stopDownward = _startY + path.Y;
                _stopUpward = _startY;
            }
            else if (path.Y == 0 && path.X < 0)
            {
                _species = Species.HorizontalReflected;
                currentPhase = Phases.Leftward;
                _stopRightward = _startX;
                _stopLeftward = _startX + path.X;
            }
            else if (path.Y == 0)
            {
                _species = Species.Horizontal;
                currentPhase = Phases.Rightward;
                _stopRightward = _startX + path.X;
                _stopLeftward = _startX;
            }
            else if (path.X < 0 && path.Y < 0)
            {
                _species = Species.Reflected;
                currentPhase = Phases.Leftward;
                _stopRightward = _startX;
                _stopDownward = _startY;
                _stopLeftward = _startX + path.X;
                _stopUpward = _startY + path.Y;
            }
            else if (path.X < 0)
            {
                _species = Species.ReflectedX;
                currentPhase = Phases.Leftward;
                _stopRightward = _startX;
                _stopDownward = _startY + path.Y;
                _stopLeftward = _startX + path.X;
                _stopUpward = _startY;
            }
            else if (path.Y < 0)
            {
                _species = Species.ReflectedY;
                currentPhase = Phases.Upward;
                _stopRightward = _startX + path.X;
                _stopDownward = _startY;
                _stopLeftward = _startX;
                _stopUpward = _startY + path.Y;
            }
            else
            {
                _species = Species.Rectangular;
                currentPhase = Phases.Rightward;
                _stopRightward = _startX + path.X;
                _stopDownward = _startY + path.Y;
                _stopLeftward = _startX;
                _stopUpward = _startY;
            }
        }

        private void AdvancePhase()
        {
            const int NUM_PHASES = 4;

            if (_species == Species.Rectangular || _species == Species.Reflected || _species == Species.ReflectedY)
                currentPhase = (Phases)((int)(currentPhase + 1) % NUM_PHASES);
            else if (_species == Species.ReflectedX)
                currentPhase = currentPhase == 0 ? (Phases)(NUM_PHASES - 1) : (Phases)((int)(currentPhase - 1) % NUM_PHASES);
            else if (
                _species == Species.Horizontal || _species == Species.HorizontalReflected ||
                _species == Species.Vertical || _species == Species.VerticalReflected)

                currentPhase = (Phases)((int)(currentPhase + 2) % NUM_PHASES);
        }

        public override void Reset()
        {
            switch (_species)
            {
                case Species.Fixed: break;
                case Species.VerticalReflected: currentPhase = Phases.Upward;
                    break;
                case Species.Vertical: currentPhase = Phases.Downward;
                    break;
                case Species.HorizontalReflected: currentPhase = Phases.Leftward;
                    break;
                case Species.Horizontal: currentPhase = Phases.Rightward;
                    break;
                case Species.Reflected: currentPhase = Phases.Leftward;
                    break;
                case Species.ReflectedX: currentPhase = Phases.Leftward;
                    break;
                case Species.ReflectedY: currentPhase = Phases.Upward;
                    break;
                case Species.Rectangular: currentPhase = Phases.Rightward;
                    break;
                default: throw new System.Exception("Error - Invalid blade type selection.");
            }

        }

        protected override void Animate()
        {
            if (++delayCount == interval)
            {
                if (++tileSelection.X == sheetDimensions.X)
                    tileSelection.X = 0;

                delayCount = 0;
            }

            SetFrame();
        }

        public override void Update()
        {
            if (_species != Species.Fixed)
            {
                if (currentPhase == Phases.Rightward)
                {
                    MoveRight();

                    if (x + velocity.X >= _stopRightward)
                        AdvancePhase();
                }
                else if (currentPhase == Phases.Downward)
                {
                    MoveDown();

                    if (y + velocity.Y >= _stopDownward)
                        AdvancePhase();
                }
                else if (currentPhase == Phases.Leftward)
                {
                    MoveLeft();

                    if (x + velocity.X <= _stopLeftward)
                        AdvancePhase();
                }
                else //if (currentPhase == Phases.Upward)
                {
                    MoveUp();

                    if (y + velocity.Y <= _stopUpward)
                        AdvancePhase();
                }
            }

            base.Update();
        }
    }
}
