using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class BoulderOutlet : Terrain, IHasAttachments
    {
        private readonly Boulder _boulder;
        private readonly Terrain[] _pieces;

        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(_boulder);
                attachment.AddRange(_pieces);

                return attachment;
            }
        }

        public BoulderOutlet(int x, int y, Rectangle tripZone, int[] path, IGameObject target)
            : base(Images.BOULDER_RELEASE_UPPER_LEFT, x, y)
        {
            _boulder = new Boulder(x, y, tripZone, path, target);
            _pieces = new Terrain[3];
            _pieces[0] = new Terrain(Images.BOULDER_RELEASE_UPPER_RIGHT, x + 1, y);
            _pieces[1] = new Terrain(Images.BOULDER_RELEASE_LOWER_LEFT, x, y + 1);
            _pieces[2] = new Terrain(Images.BOULDER_RELEASE_LOWER_RIGHT, x + 1, y + 1);
        }
    }
}
