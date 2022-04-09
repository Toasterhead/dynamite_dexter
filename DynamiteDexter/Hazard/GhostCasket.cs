using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class GhostCasket : Trippable
    {
        private static readonly Rectangle _tripZoneRel = new Rectangle(
            -2 * Game1.TILE_SIZE, 
            -2 * Game1.TILE_SIZE, 
            5 * Game1.TILE_SIZE, 
            6 * Game1.TILE_SIZE);

        public GhostCasket(int x, int y)
            : base(Images.CASKET_TOP, x, y, 1, 0, _tripZoneRel) { }

        public override IGameObject[] GetItem(IGameObject target)
        {
            IGameObject[] item = new IGameObject[1];
            item[0] = new Ghost(GridX, GridY, target);

            return item;
        }
    }
}
