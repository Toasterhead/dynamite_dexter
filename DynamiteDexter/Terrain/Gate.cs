namespace DynamiteDexter
{
    public class Gate : Terrain, IHunts, IResets
    {
        private readonly bool _horizontal;
        private readonly IGameObject _target;

        private bool closed;
        private int trippedNone;
        private int? trippedNegative;
        private int? trippedPositive;

        public Gate Link { get; set; }
        public IGameObject Target { get { return _target; } }
        public bool Closed { get { return closed; } }

        public Gate(int x, int y, bool horizontal, IGameObject target)
            : base(horizontal ? Images.GATE_HORIZONTAL : Images.GATE_VERTICAL, x, y, 6, 2)
        {
            _target = target;
            _horizontal = horizontal;
            trippedNone = 0;
            trippedNegative = null;
            trippedPositive = null;
            render = false;
        }

        public void Reset()
        {
            trippedNone = 0;
            trippedNegative = null;
            trippedPositive = null;
        }

        public void HardReset()
        {
            closed = false;
            render = false;
            tileSelection.X = 0;
            delayCount = 0;
            Reset();
        }

        protected override void Animate()
        {
            if (closed && tileSelection.X < sheetDimensions.X - 1)
            {
                render = true;

                if (++delayCount >= interval)
                {
                    tileSelection.X++;
                    delayCount = 0;
                }
            }

            SetFrame();
        }

        public override void Update()
        {
            if (!closed)
            {
                int centerTarget = _horizontal ? _target.Center.Y : _target.Center.X;
                int centerThis = _horizontal ? Center.Y : Center.X;

                if (_target.Rect.Intersects(Rect))
                {
                    if (centerTarget >= centerThis && trippedPositive == null)
                        trippedPositive = Game1.universalTimer;
                    else if (centerTarget < centerThis && trippedNegative == null)
                        trippedNegative = Game1.universalTimer;
                }
                else if (trippedNegative != null && trippedPositive != null)
                {
                    if ((centerTarget >= centerThis && trippedPositive > trippedNegative && trippedNegative > trippedNone) ||
                        (centerTarget < centerThis && trippedNegative > trippedPositive && trippedPositive > trippedNone))

                        closed = true;

                    else Reset();
                }
                else
                {
                    trippedNone = Game1.universalTimer;
                    trippedNegative = null;
                    trippedPositive = null;
                }
            }

            base.Update();
        }
    }
}
