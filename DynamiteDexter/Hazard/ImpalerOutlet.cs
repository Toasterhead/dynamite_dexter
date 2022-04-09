using System.Collections.Generic;

namespace DynamiteDexter
{
    public class ImpalerOutlet : Terrain, IHasAttachments
    {
        Impaler impaler;
        Terrain rear;

        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(impaler);
                attachment.Add(rear);

                return attachment;
            }
        }

        public ImpalerOutlet(int x, int y, bool upward, Game1.SyncStates syncState)
            : base(
                  upward ? Images.ARROW_LAUNCHER_UPWARD_TOP : Images.ARROW_LAUNCHER_DOWNWARD_BOTTOM, x, y)
        {
            if (upward)
            {
                rear = new Terrain(Images.ARROW_LAUNCHER_UPWARD_BOTTOM, x, y + 1);
                impaler = new Impaler(this, upward, syncState);
            }
            else
            {
                rear = new Terrain(Images.ARROW_LAUNCHER_DOWNWARD_TOP, x, y - 1);
                impaler = new Impaler(this, upward, syncState);
            }
        }
    }
}
