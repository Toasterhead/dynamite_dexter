using System;
using System.Collections.Generic;

namespace DynamiteDexter
{
    public class Cannon : Terrain, IFires, IHasAttachments
    {
        private const int NUM_QUARTERS = 4;

        private readonly bool _leftward;
        private readonly Game1.SyncStates _syncState;

        private IProjectile[] chamber;

        public readonly Sprite Tip;

        public IProjectile[] Chamber { get { return chamber; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(Tip);

                return attachment;
            }
        }

        public Cannon(int x, int y, bool leftward, Game1.SyncStates syncState)
            : base(leftward ? Images.CANNON_LEFTWARD_BULK : Images.CANNON_RIGHTWARD_BULK, x, y)
        {
            _leftward = leftward;
            _syncState = syncState;
            Tip = new Sprite(new SpriteInfo(
                leftward ? Images.CANNON_LEFTWARD_TIP : Images.CANNON_RIGHTWARD_TIP,
                leftward ? x * Game1.TILE_SIZE - Width : x * Game1.TILE_SIZE,
                y * Game1.TILE_SIZE - Height,
                layer: (int)Game1.Layers.Terrain));
        }

        public void EmptyChamber() { chamber = null; }

        public override void Update()
        {
            bool willFire = false;
            int count = Game1.universalTimer % (NUM_QUARTERS * Game1.SYNC_QUARTER);

            if (_syncState == Game1.SyncStates.FirstAndThirdQuarter)

                willFire = count == 0 * Game1.SYNC_QUARTER || count == 2 * Game1.SYNC_QUARTER;

            else if (_syncState == Game1.SyncStates.SecondAndFourthQuarter)

                willFire = count == 1 * Game1.SYNC_QUARTER || count == 3 * Game1.SYNC_QUARTER;

            else willFire = count == (int)_syncState * Game1.SYNC_QUARTER;

            if (willFire)
            {
                chamber = new IProjectile[1];
                chamber[0] = new Bomb(_leftward ? Left - 4 : Right + 3, Center.Y, !_leftward, -12, this);
            }
        }
    }
}
