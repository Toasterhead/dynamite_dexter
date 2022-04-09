using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Raccoon : Pedestrian, IHostile, IHunts, ISeeks, IHasAttachments, IOutlined
    {
        private enum Phases { Hide, Seek, Abscond }

        private const int SEEKING_SPEED = 3;
        private const int ABSCONDING_SPEED = 2;
        private const int THRESHOLD = 4;
        private const uint MINIMUM_TRAVEL_DISTANCE = 3 * Game1.TILE_SIZE;
        private const double DETECTION_RADIUS = 4 * Game1.TILE_SIZE;

        public const int DOLLARS_TO_STEAL = 15;

        private static readonly Rectangle[] _raccoonHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly Sprite _loot;
        private readonly Passable _hidingPlace;
        private readonly OutlineMask _outlineMask;

        private int dollarsStolen;
        private uint travelCount = 0;
        private Phases currentPhase;
        private Point currentWaypoint;
        private Stack<Point> path;
        private Graph graph;

        public bool DynamicGraph { get { return true; } }
        public uint PathUpdateInterval { get { return 15; } }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public int DollarsStolen { get { return dollarsStolen; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_loot);
                attachments.Add(_hidingPlace);

                return attachments;
            }
        }
        public bool HandsFull { get { return currentPhase == Phases.Abscond; } }

        public Raccoon(int x, int y, IGameObject target, Passable hidingPlace)
		    : base(
			    new SpriteInfo(Images.RACCOON, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Actor),
			    new CollisionInfo(_raccoonHitBox, null),
			    new AnimationInfo(4, 4, 4),
			    speed: SEEKING_SPEED)
	    {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _loot = new Sprite(Images.LOOT_BAG, _startX, _startY - Game1.TILE_SIZE, (int)Game1.Layers.Actor);
            _hidingPlace = hidingPlace;
            _outlineMask = new OutlineMask(Images.RACCOON_OUTLINE, this);
            _loot.Render = false;
            dollarsStolen = 0;
            travelCount = 0;
            currentPhase = Phases.Hide;
            graph = null;
            currentWaypoint = new Point(x, y);
        }

        public void AcquireGraph(Graph graph) { this.graph = graph; }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Reset()
        {
            x = _startX;
            y = _startY;
            currentWaypoint = new Point((int)x / Game1.TILE_SIZE, (int)y / Game1.TILE_SIZE);
            UpdatePath();
            _loot.Reposition(_startX, _startY - Game1.TILE_SIZE);
            _loot.Render = false;
            Neutral();
            travelCount = 0;
            dollarsStolen = 0;
            currentPhase = Phases.Hide;
        }

        public void UpdatePath()
        {
            int gridX = Center.X / Game1.TILE_SIZE;
            int gridY = Center.Y / Game1.TILE_SIZE;
            int vertex = BinarySearch(gridX, gridY);

            int targetGridX = _target.Center.X / Game1.TILE_SIZE;
            int targetGridY = _target.Center.Y / Game1.TILE_SIZE;
            int targetVertex = BinarySearch(targetGridX, targetGridY);

            if (vertex == -1 || targetVertex == -1)
            {
                path = null;
                return;
            }

            graph.Path(vertex);
            path = graph.ShortestPathTo(targetVertex);
        }

        private int BinarySearch(int keyX, int keyY)
        {
            int lowerBound = 0;
            int upperbound = graph.Size - 1;
            int current;

            while (true)
            {
                current = (lowerBound + upperbound) / 2;

                Point vertexData = graph.GetVertexData(current);

                if (vertexData.X == keyX && vertexData.Y == keyY)

                    return current;

                else if (lowerBound > upperbound)

                    return -1;

                else
                {
                    int compressedCurrent = vertexData.X + (vertexData.Y * Game1.GRID_SIZE_X);
                    int compressedKey = keyX + (keyY * Game1.GRID_SIZE_X);

                    if (compressedCurrent < compressedKey)
                        lowerBound = current + 1;
                    else upperbound = current - 1;
                }
            }
        }

        public void FillHands(int amount)
        {
            dollarsStolen = amount;
            currentPhase = Phases.Abscond;
            velocity.Normalize();
            velocity *= ABSCONDING_SPEED;
            Game1.PlaySound(Sounds.NAB);
        }

        protected override void Animate()
        {
            const int CARRYING_FRAME = 2;

            if (currentPhase == Phases.Abscond)
            {
                _loot.Render = true;

                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= sheetDimensions.X)
                        tileSelection.X = CARRYING_FRAME;

                    delayCount = 0;
                }
            }
            else if (currentPhase == Phases.Seek)
            {
                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= CARRYING_FRAME)
                        tileSelection.X = 0;

                    delayCount = 0;
                }
            }

            if (velocity.X < 0.0f)
                tileSelection.Y = 0;
            else if (velocity.X > 0.0f)
                tileSelection.Y = 1;
            else if (velocity.Y < 0.0f)
                tileSelection.Y = 2;
            else if (velocity.Y > 0.0f)
                tileSelection.Y = 3;

            SetFrame();
        }

        public override void Update()
        {
            if (currentPhase == Phases.Hide)
            {
                //Where 'c' is the hypotenuse of a right triangle...
                double a = x - _target.X;
                double b = y - _target.Y;
                double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                if (c < DETECTION_RADIUS)
                    currentPhase = Phases.Seek;
            }
            else if (currentPhase == Phases.Seek)
            {
                if (path != null)
                {
                    const int HALF_TILE = Game1.TILE_SIZE / 2;

                    if (Center.X > currentWaypoint.X * Game1.TILE_SIZE + HALF_TILE - THRESHOLD &&
                        Center.X < currentWaypoint.X * Game1.TILE_SIZE + HALF_TILE + THRESHOLD)

                        x = (currentWaypoint.X * Game1.TILE_SIZE + HALF_TILE) - (Width / 2);

                    if (Center.Y > currentWaypoint.Y * Game1.TILE_SIZE + HALF_TILE - THRESHOLD &&
                        Center.Y < currentWaypoint.Y * Game1.TILE_SIZE + HALF_TILE + THRESHOLD)

                        y = (currentWaypoint.Y * Game1.TILE_SIZE + HALF_TILE) - (Height / 2);

                    if (Center.X < currentWaypoint.X * Game1.TILE_SIZE + HALF_TILE)
                        MoveRight();
                    else if (Center.X > currentWaypoint.X * Game1.TILE_SIZE + HALF_TILE)
                        MoveLeft();
                    else if (Center.Y < currentWaypoint.Y * Game1.TILE_SIZE + HALF_TILE)
                        MoveDown();
                    else if (Center.Y > currentWaypoint.Y * Game1.TILE_SIZE + HALF_TILE)
                        MoveUp();
                    else if (path.Count > 0)
                        currentWaypoint = path.Pop();
                    else path = null;
                }
                else Neutral();
            }
            else //if (currentPhase == Phases.Abscond)
            {
                if (x < -Game1.TILE_SIZE || x >= Game1.playfield.X || y < -Game1.TILE_SIZE)

                    velocity = new Vector2(0.0f, 0.0f);

                else if (y >= Game1.playfield.Y)

                    velocity = new Vector2(0.0f, ABSCONDING_SPEED);

                else if (++travelCount >= MINIMUM_TRAVEL_DISTANCE)
                {
                    if (Game1.rand.Next(30) == 0)
                    {
                        int negationCoefficient = Game1.rand.Next(2) == 0 ? 1 : -1;

                        if (velocity.X == 0.0f)
                        {
                            velocity.X = negationCoefficient * ABSCONDING_SPEED;
                            velocity.Y = 0.0f;
                        }
                        else
                        {
                            velocity.X = 0.0f;
                            velocity.Y = negationCoefficient * ABSCONDING_SPEED;
                        }

                        travelCount = 0;
                    }
                }

                if (ContactWithTerrain)
                    velocity = -velocity;
            }

            _loot.Reposition(X, Y - Game1.TILE_SIZE);

            base.Update();
        }
    }
}
