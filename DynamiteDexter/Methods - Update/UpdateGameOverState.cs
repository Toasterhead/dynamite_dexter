using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateGameOverState()
        {
            if (universalTimer - universalTimeStamp > 304)
                SubUpdateGameOver();

            universalTimer++;
        }
    }
}
