using System.Collections.Generic;

namespace DynamiteDexter
{
    public class Room
    {
        public  enum Mode { Standard, House, Darkened, Ominous, Boss, Skip }

        private readonly Mode _mode;
        private readonly string _plaqueText;

        private List<IGameObject> spriteSet;

        public List<IGameObject> SpriteSet { get { return spriteSet; } }
        public Mode TheMode { get { return _mode; } }
        public string PlaqueText
        {
            get
            {
                string s = "";

                foreach (char ch in _plaqueText)
                    s += ch;

                return s;
            }
        }

        public Room(List<IGameObject> spriteSet, string plaqueText, Mode mode)
        {
            this.spriteSet = spriteSet;
            _plaqueText = plaqueText;
            _mode = mode;
        }

        public void RemoveGameObject(IGameObject gObj) //Necessary? Consider deleting.
        {
            if (gObj is IHostile || gObj is Pickup || gObj is Breakable)
                spriteSet.Remove(gObj);
        }

        public void SwapLayout(List<Terrain> alternateLayout)
        {
            if (alternateLayout != null)
            { 
                List<Terrain> removalSet = new List<Terrain>();

                foreach (IGameObject i in spriteSet)

                    if (i is Terrain)
                        removalSet.Add(i as Terrain);

                foreach (Terrain i in removalSet)
                    spriteSet.Remove(i as IGameObject);

                foreach (Terrain i in alternateLayout)
                    spriteSet.Add(i);
            }
        }
    }
}
