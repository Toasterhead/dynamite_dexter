using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Flag : SpriteSheet, IHasAttachments
    {
        private const int NUM_FLAGS = 9;

        public readonly int Number;
        public readonly Tile PoleBase;

        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(PoleBase);

                return attachment;
            }
        }

        public Flag(int x, int y, int number)
            : base(
                  new SpriteInfo(Images.FLAG, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Floor),
                  new CollisionInfo(null, null),
                  new AnimationInfo(NUM_FLAGS + 1, 1, 0))
        {
            Number = number < Game1.NUM_FLAGS ? number : 0;
            PoleBase = new Tile(
                Images.FLAG_POLE_BOTTOM, 
                Left / Game1.TILE_SIZE, 
                Bottom / Game1.TILE_SIZE, 
                (int)Game1.Layers.Floor);
        }

        protected override void Animate()
        {
            if (Game1.flagStatus[Number].raised)
                tileSelection.X = Number;
            else tileSelection.X = NUM_FLAGS;

            SetFrame();
        }
    }

    public class FlagStatus
    {
        public bool raised;
        public Point mapLocation;
        public Point startLocation;

        public FlagStatus()
        {
            raised = false;
            mapLocation = new Point(0, 0);
            startLocation = new Point(0, 0);
        }
    }
}
