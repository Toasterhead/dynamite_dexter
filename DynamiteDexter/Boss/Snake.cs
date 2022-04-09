using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Snake : SpriteSheet, IBoss, INavigates, ISeeks, IHasAttachments 
    {
        public enum Directions { Left = 0, Up, Right, Down, EnumSize }

        private const int START_X = 0;
        private const int START_Y = 0;
        private const int SPEED = 2;
        private const int LENGTH = 12;
        private const int OUTER_BOUND = 2;
        private const int INITIAL_VITALITY = 3;
        private const uint FLASH_DURATION = 30;
        private const uint TILE_INTERVAL = Game1.TILE_SIZE / SPEED;
        private const uint PATH_UPDATE_INTERVAL = 5 * TILE_INTERVAL;

        private static readonly Point playfield = Game1.playfield;
        private static readonly Rectangle[] _dummyHitBox =
        {
            new Rectangle(0, 0, 0, 0)
        };
        private static readonly Sprite _appearingForm = new Sprite(
            Images.APPEARING_SNAKE, 
            START_X, 
            START_Y, 
            (int)Game1.Layers.Actor);

        private readonly int _startX;
        private readonly int _startY;
        private readonly SnakeTail _tail;

        private int targetVertex;
        private int segmentTileSelection;
        private int vitality;
        private uint count;
        private uint flashCount;
        private Graph graph;
        private Directions direction;
        private SnakeSegment[] segment;
        private Stack<Point> path;
        private List<IGameObject> spriteSet;

        public bool DynamicGraph { get { return true; } }
        public bool Flashing { get { return flashCount % 2 != 0; } }
        public uint PathUpdateInterval { get { return PATH_UPDATE_INTERVAL; } }
        public int Vitality { get { return vitality; } }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public Rectangle HitBoxAssault { get { return new Rectangle(0, 0, 0, 0); } }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public IGameObject Target { get { return null; } }
        public SnakeSegment[] Segment { get { return segment; } }
        public SnakeTail Tail { get { return _tail; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_tail);
                attachments.AddRange(segment);

                return attachments;
            }
        }

        public Snake()
            : base(
                  new SpriteInfo(Images.SNAKE_HEAD, 0, 0, (int)Game1.Layers.Actor),
                  new CollisionInfo(_dummyHitBox, null),
                  new AnimationInfo(4, 1, 0))
        {
            _startX = (int)x;
            _startY = (int)y;
            _tail = new SnakeTail(this);

            segment = new SnakeSegment[LENGTH];
            segment[0] = new SnakeSegment(2, 2, null);
            segment[1] = new SnakeSegment(3, 2, segment[0]);
            segment[2] = new SnakeSegment(4, 2, segment[1]);
            segment[3] = new SnakeSegment(5, 2, segment[2]);
            segment[4] = new SnakeSegment(7, 2, segment[3]);
            segment[5] = new SnakeSegment(8, 2, segment[4]);
            segment[6] = new SnakeSegment(9, 2, segment[5]);
            segment[7] = new SnakeSegment(10, 2, segment[6]);
            segment[8] = new SnakeSegment(11, 2, segment[7]);
            segment[9] = new SnakeSegment(11, 3, segment[8]);
            segment[10] = new SnakeSegment(11, 4, segment[9]);
            segment[11] = new SnakeSegment(11, 5, segment[10]);

            segmentTileSelection = 0;
            vitality = INITIAL_VITALITY;
            flashCount = 0;
        }

        public void AcquireSet(List<IGameObject> spriteSet) { this.spriteSet = spriteSet; }

        public void AcquireGraph(Graph graph) { this.graph = graph; }

        public bool MoveLeft()
        {
            if (segment[0].GridX - 1 >= 0)
                segment[0].MoveTo(segment[0].GridX - 1, segment[0].GridY);
            else return false;

            direction = Directions.Left;

            return true;
        }

        public bool MoveRight()
        {
            if (segment[0].GridX + 1 < Game1.GRID_SIZE_X)
                segment[0].MoveTo(segment[0].GridX + 1, segment[0].GridY);
            else return false;

            direction = Directions.Right;

            return true;
        }

        public bool MoveUp()
        {
            if (segment[0].GridY - 1 >= 0)
                segment[0].MoveTo(segment[0].GridX, segment[0].GridY - 1);
            else return false;

            direction = Directions.Up;

            return true;
        }

        public bool MoveDown()
        {
            if (segment[0].GridY + 1 < Game1.GRID_SIZE_Y)
                segment[0].MoveTo(segment[0].GridX, segment[0].GridY + 1);
            else return false;

            direction = Directions.Down;

            return true;
        }

        public void Strike()
        {
            if (--vitality == 0)
                remove = true;

            flashCount = FLASH_DURATION;
        }

        public void Reset()
        {
            count = 0;

            for (int i = 0; i < LENGTH; i++)
                segment[i].Reposition(LENGTH - i, 11);
        }

        public void UpdatePath()
        {
            int targetX;
            int targetY;

            do
            {
                do
                {
                    targetX = Game1.rand.Next(OUTER_BOUND, Game1.GRID_SIZE_X - OUTER_BOUND);
                    targetY = Game1.rand.Next(OUTER_BOUND, Game1.GRID_SIZE_Y - OUTER_BOUND);
                }
                while (
                targetX >= segment[0].GridX - 5 && 
                targetX < segment[0].GridX + 5 &&
                targetY >= segment[0].GridY - 5 && 
                targetY < segment[0].GridY + 5);

                targetVertex = BinarySearch(targetX, targetY);
            }
            while (targetVertex == -1);

            int vertex = BinarySearch(segment[0].GridX, segment[0].GridY);

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

        protected override void Animate()
        {
            switch (direction)
            {
                case Directions.Left: tileSelection.X = 0;
                    break;
                case Directions.Up: tileSelection.X = 1;
                    break;
                case Directions.Right: tileSelection.X = 2;
                    break;
                case Directions.Down: tileSelection.X = 3;
                    break;
                default: throw new Exception("Error - Invalid direction selected.");
            }

            if (++segmentTileSelection >= TILE_INTERVAL)
                segmentTileSelection = 0;

            foreach (SnakeSegment i in segment)
                i.SetFrameSelection(segmentTileSelection, Flashing);

            if (Flashing)
                render = false;
            else render = true;

            SetFrame();
        }

        public override void Update()
        {
            SnakeSegment first = segment[0];
            SnakeSegment last = segment[LENGTH - 1];
            SnakeSegment nextToLast = segment[LENGTH - 2];

            //Move to entire snake to next position in path...

            if (path != null && path.Count > 0 && ++count == TILE_INTERVAL)
            {
                Point nextMove = path.Pop();

                if (nextMove.X == first.GridX - 1 && nextMove.Y == first.GridY)
                    MoveLeft();
                else if (nextMove.X == first.GridX + 1 && nextMove.Y == first.GridY)
                    MoveRight();
                else if (nextMove.X == first.GridX && nextMove.Y == first.GridY - 1)
                    MoveUp();
                else if (nextMove.X == segment[0].GridX && nextMove.Y == segment[0].GridY + 1)
                    MoveDown();

                count = 0;
            }

            //Determime direction of tail...

            if (last.GridX == nextToLast.GridX - 1)
                _tail.Direction = Directions.Right;
            else if (last.GridX == nextToLast.GridX + 1)
                _tail.Direction = Directions.Left;
            else if (last.GridY == nextToLast.GridY - 1)
                _tail.Direction = Directions.Down;
            else if (last.GridY == nextToLast.GridY + 1)
                _tail.Direction = Directions.Up;

            //Determine displacemnt of head...

            int distance = (int)count * SPEED;
            int headDisplacementX = 0;
            int headDisplacementY = 0;

            switch (direction)
            {
                case Directions.Left: headDisplacementX = -distance;
                    break;
                case Directions.Up: headDisplacementY = -distance;
                    break;
                case Directions.Right: headDisplacementX = distance;
                    break;
                case Directions.Down: headDisplacementY = distance;
                    break;
                default: throw new Exception("Error - Invalid direction selected.");
            }

            //Determine displacement of tail...

            int tailDisplacementX = -Game1.TILE_SIZE;
            int tailDisplacementY = -Game1.TILE_SIZE;

            switch (_tail.Direction)
            {
                case Directions.Left: tailDisplacementX += -distance + Game1.TILE_SIZE;
                    break;
                case Directions.Up: tailDisplacementY += -distance + Game1.TILE_SIZE;
                    break;
                case Directions.Right: tailDisplacementX += distance - Game1.TILE_SIZE;
                    break;
                case Directions.Down: tailDisplacementY += distance - Game1.TILE_SIZE;
                    break;
                default: throw new Exception("Error - Invalid direction selected.");
            }

            //Position head and tail...

            Reposition(first.X + headDisplacementX, first.Y + headDisplacementY);
            _tail.Reposition(last.X + tailDisplacementX, last.Y + tailDisplacementY);

            //Count down flash effect, if applicable...

            if (flashCount > 0)
                flashCount--;

            base.Update();
        }
    }

    public class SnakeSegment : TileSheet
    {
        private bool reverse;
        private SnakeSegment linkTowardTail;
        private SnakeSegment linkTowardHead;

        private static readonly Rectangle[] _snakeSegmentHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        public SnakeSegment(int x, int y, SnakeSegment linkTowardHead)
            : base(
                  new SpriteInfo(Images.SNAKE_SEGMENT, x, y, (int)Game1.Layers.Terrain),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(_snakeSegmentHitBox, null),
                  new AnimationInfo(8, 6, 0))
        {
            reverse = false;

            this.linkTowardHead = linkTowardHead;

            if (linkTowardHead != null)
                this.linkTowardHead.SetLinkTowardTail(this);
        }

        public void SetLinkTowardTail(SnakeSegment linkTowardTail) { this.linkTowardTail = linkTowardTail; }

        public void MoveTo(int x, int y)
        {
            int prevX = GridX;
            int prevY = GridY;

            GridX = x;
            GridY = y;

            if (linkTowardTail != null)
                linkTowardTail.MoveTo(prevX, prevY);
        }

        public void SetFrameSelection(int tileSelectionX, bool flashing)
        {
            if (reverse)
                tileSelection.X = sheetDimensions.X - 1 - tileSelectionX;
            else tileSelection.X = tileSelectionX;

            if (flashing)
                render = false;
            else render = true;
        }

        protected override void Animate()
        {
            reverse = false;

            //Set vertical frame selection... (direction) 

            if (linkTowardHead == null)
            {
                if (linkTowardTail.GridX == GridX - 1)

                    tileSelection.Y = 0;

                else if (linkTowardTail.GridX == GridX + 1)
                {
                    tileSelection.Y = 0;
                    reverse = true;
                }
                else if (linkTowardTail.GridY == GridY - 1)

                    tileSelection.Y = 1;

                else
                {
                    tileSelection.Y = 1;
                    reverse = true;
                }
            }
            else if (linkTowardTail == null)
            {
                if (linkTowardHead.GridX == GridX - 1)
                {
                    tileSelection.Y = 0;
                    reverse = true;
                }

                else if (linkTowardHead.GridX == GridX + 1)

                    tileSelection.Y = 0;

                else if (linkTowardHead.GridY == GridY - 1)
                {
                    tileSelection.Y = 1;
                    reverse = true;
                }
                else tileSelection.Y = 1;
            }
            else if (linkTowardHead.GridX == GridX - 1 && linkTowardTail.GridX == GridX + 1)
            {
                tileSelection.Y = 0;
                reverse = true;
            }
            else if (linkTowardTail.GridX == GridX - 1 && linkTowardHead.GridX == GridX + 1)
            {
                tileSelection.Y = 0;
            }
            else if (linkTowardHead.GridY == GridY - 1 && linkTowardTail.GridY == GridY + 1)
            {
                tileSelection.Y = 1;
                reverse = true;
            }
            else if (linkTowardTail.GridY == GridY - 1 && linkTowardHead.GridY == GridY + 1)
            {
                tileSelection.Y = 1;
            }
            else if ((linkTowardHead.GridX == GridX - 1 && linkTowardTail.GridY == GridY - 1) ||
                     (linkTowardTail.GridX == GridX - 1 && linkTowardHead.GridY == GridY - 1))

                tileSelection.Y = 2;

            else if ((linkTowardHead.GridX == GridX - 1 && linkTowardTail.GridY == GridY + 1) ||
                     (linkTowardTail.GridX == GridX - 1 && linkTowardHead.GridY == GridY + 1))

                tileSelection.Y = 3;

            else if ((linkTowardHead.GridX == GridX + 1 && linkTowardTail.GridY == GridY - 1) ||
                     (linkTowardTail.GridX == GridX + 1 && linkTowardHead.GridY == GridY - 1))

                tileSelection.Y = 4;

            else tileSelection.Y = 5;

            //Update horizontal frame selection...

            SetFrame();
        }
    }

    public class SnakeTail : SpriteSheet
    {
        public Snake.Directions Direction { get; set; }

        public readonly Snake Parent;

        private static readonly Rectangle[] _snakeTailHitbox =
        {
            new Rectangle(Game1.TILE_SIZE, Game1.TILE_SIZE, Game1.TILE_SIZE, Game1.TILE_SIZE)
        };

        public SnakeTail(Snake parent)
            : base(
                  new SpriteInfo(Images.SNAKE_TAIL, 0, 0, (int)Game1.Layers.Actor),
                  new CollisionInfo(_snakeTailHitbox, null),
                  new AnimationInfo(4, 1, 0))
        { Parent = parent; }

        protected override void Animate()
        {
            switch (Direction)
            {
                case Snake.Directions.Left: tileSelection.X = 2;
                    break;
                case Snake.Directions.Up: tileSelection.X = 3;
                    break;
                case Snake.Directions.Right: tileSelection.X = 0;
                    break;
                case Snake.Directions.Down: tileSelection.X = 1;
                    break;
            }

            if (Parent.Flashing)
                render = false;
            else render = true;

            SetFrame();
        }
    }
}