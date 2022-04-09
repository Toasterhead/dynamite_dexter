using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Zombie : Pedestrian, IHostile, IHunts, ISeeks, IOutlined
    {
        private const int SPEED = 1;
        private const int THRESHOLD = 2;

        private static readonly Rectangle[] _zombieHitBox =
        {
            new Rectangle(2, 2, Game1.TILE_SIZE - 4, Game1.TILE_SIZE - 4)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly OutlineMask _outlineMask;

        private Point currentWaypoint;
        private Stack<Point> path;
        private Graph graph;  

        public bool DynamicGraph { get { return true; } }
        public uint PathUpdateInterval { get { return 30; } }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Zombie(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(
                      Images.ZOMBIE,
                      x * Game1.TILE_SIZE,
                      y * Game1.TILE_SIZE,
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_zombieHitBox, null),
                  new AnimationInfo(2, 4, 5),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _outlineMask = new OutlineMask(Images.ZOMBIE_OUTLINE, this);
            graph = null;
            currentWaypoint = new Point(x, y);
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public void AcquireGraph(Graph graph) { this.graph = graph; }

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

        public override void Reset()
        {
            Game1.PlaySound(Sounds.ZOMBIE);
            currentWaypoint = new Point((int)x / Game1.TILE_SIZE, (int)y / Game1.TILE_SIZE);
            UpdatePath();
        }

        public override void Update()
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

            base.Update();
        }
    }
}
