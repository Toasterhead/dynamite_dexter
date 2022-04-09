using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace DynamiteDexter
{
    public class Pickup : TileSheet
    {
        public readonly SoundEffectInstance Sound;

        public Pickup(Texture2D image, int x, int y, int frames, SoundEffectInstance sound)
            : base(
                  new SpriteInfo(image, x, y, layer: (int)Game1.Layers.Floor),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(null, null),
                  new AnimationInfo(frames, 1, 9))
        { Sound = sound;}
    }

    public class Treasure : Pickup
    {
        public readonly int Value;

        public Treasure(Texture2D image, int x, int y, int frames, int value, SoundEffectInstance sound)
            : base(image, x, y, frames, sound) { Value = value; }
    }

    public class Coins : Treasure
    {
        public Coins(int x, int y)
            : base(Images.COINS, x, y, 2, 1, Sounds.TREASURE_COINS) { }
    }

    public class MoneyBag : Treasure
    {
        public MoneyBag(int x, int y)
            : base(Images.MONEY_BAG, x, y, 1, 5, Sounds.TREASURE_MONEY_BAG) { }
    }

    public class Chalice : Treasure
    {
        public Chalice(int x, int y)
            : base(Images.CHALICE, x, y, 2, 20, Sounds.TREASURE_CHALICE) { }
    }

    public class Diamond : Treasure
    {
        public Diamond(int x, int y)
            : base(Images.DIAMOND, x, y, 2, 10, Sounds.TREASURE_DIAMOND) { }
    }

    public class LootBag : Treasure
    {
        public LootBag(int x, int y, int amount)
            : base(Images.MONEY_BAG, x, y, 1, amount, Sounds.TREASURE_MONEY_BAG) { }
    }

    public class ExtraLife : Pickup
    {
        public ExtraLife(int x, int y)
            : base(Images.EXTRA_LIFE, x, y, 1, Sounds.EXTRA_LIFE) { }
    }

    public class DynamiteStock : Pickup
    {
        public readonly int Value;

        public DynamiteStock(Texture2D image, int x, int y, int value, SoundEffectInstance sound)
            : base(image, x, y, 1, sound) { Value = value; }
    }

    public class DynamiteSingleStick : DynamiteStock
    {
        public DynamiteSingleStick(int x, int y)
            : base(Images.DYNAMITE_STICK, x, y, 1, Sounds.DYNAMITE_STICK) { }
    }

    public class DynamitePack : DynamiteStock
    {
        public DynamitePack(int x, int y)
            : base(Images.DYNAMITE_PACK, x, y, 5, Sounds.DYNAMITE_PACK) { }
    }

    public class Key : Pickup
    {
        public Key(int x, int y)
            : base(Images.KEY, x, y, 1, Sounds.KEY) { }
    }
}
