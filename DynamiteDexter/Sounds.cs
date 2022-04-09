using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public static class Sounds
    {
        public struct SoundCharacteristics
        {
            public uint priority;
            public bool interrupts;

            public SoundCharacteristics(uint priority, bool interrupts)
            {
                this.priority = priority;
                this.interrupts = interrupts;
            }
        }

        public static SoundEffectInstance ALTERED_PATH;
        public static SoundEffectInstance COUNTDOWN;
        public static SoundEffectInstance DENY;
        public static SoundEffectInstance DYNAMITE_PACK;
        public static SoundEffectInstance DYNAMITE_STICK;
        public static SoundEffectInstance EXPLOSION;
        public static SoundEffectInstance EXTRA_LIFE;
        public static SoundEffectInstance KEY;
        public static SoundEffectInstance NAB;
        public static SoundEffectInstance OPEN_DOOR;
        public static SoundEffectInstance PAUSE;
        public static SoundEffectInstance ROBOT;
        public static SoundEffectInstance SLAY_BOSS;
        public static SoundEffectInstance SNAKE;
        public static SoundEffectInstance STRUCK;
        public static SoundEffectInstance THUMP;
        public static SoundEffectInstance TREASURE_CHALICE;
        public static SoundEffectInstance TREASURE_COINS;
        public static SoundEffectInstance TREASURE_DIAMOND;
        public static SoundEffectInstance TREASURE_MONEY_BAG;
        public static SoundEffectInstance ZOMBIE;

        public static SoundEffectInstance DUPLICATE_COUNTDOWN;
        public static SoundEffectInstance DUPLICATE_DENY;
        public static SoundEffectInstance DUPLICATE_DYNAMITE_PACK;
        public static SoundEffectInstance DUPLICATE_DYNAMITE_STICK;
        public static SoundEffectInstance DUPLICATE_EXPLOSION;
        public static SoundEffectInstance DUPLICATE_TREASURE_CHALICE;
        public static SoundEffectInstance DUPLICATE_TREASURE_COINS;
        public static SoundEffectInstance DUPLICATE_TREASURE_DIAMOND;
        public static SoundEffectInstance DUPLICATE_TREASURE_MONEY_BAG;

        public static class Music
        {
            public static Song BOSS_APPEARS;
            public static Song BOSS_THEME_A;
            public static Song BOSS_THEME_B;
            public static Song DEPARTURE;
            public static Song FANFARE;
            public static Song FINAL_BOSS;
            public static Song GAME_OVER;
            public static Song HOUSE;
            public static Song INSCRIPTION;
            public static Song TITLE;
            public static Song VICTORY_LULLABY;
            public static Song VICTORY_MARCH;
            public static Song VICTORY_MARCH_OUTRO;
        }

        public static SoundCharacteristics GetCharacteristics(SoundEffectInstance sound)
        {
            if (sound == ROBOT || sound == ZOMBIE)
                return new SoundCharacteristics(0, false);
            else if (sound == TREASURE_COINS || sound == DYNAMITE_STICK)
                return new SoundCharacteristics(1, true);
            else if (sound == TREASURE_MONEY_BAG || sound == DYNAMITE_PACK)
                return new SoundCharacteristics(2, true);
            else if (sound == TREASURE_DIAMOND || sound == KEY)
                return new SoundCharacteristics(3, true);
            else if (sound == OPEN_DOOR)
                return new SoundCharacteristics(4, false);
            else if (sound == SNAKE || sound == NAB)
                return new SoundCharacteristics(5, true);
            else if (sound == TREASURE_CHALICE)
                return new SoundCharacteristics(6, true);
            else if (sound == EXTRA_LIFE)
                return new SoundCharacteristics(7, true);
            else if (sound == ALTERED_PATH)
                return new SoundCharacteristics(8, false);
            else if (sound == SLAY_BOSS)
                return new SoundCharacteristics(9, true);
            else if (sound == STRUCK || sound == PAUSE)
                return new SoundCharacteristics(uint.MaxValue, true);

            return new SoundCharacteristics(0, true);
        }

        public static SoundEffectInstance RetrieveDuplicate(SoundEffectInstance sound)
        {
            if (sound == COUNTDOWN)
                return DUPLICATE_COUNTDOWN;
            else if (sound == DENY)
                return DUPLICATE_DENY;
            else if (sound == DYNAMITE_PACK)
                return DUPLICATE_DYNAMITE_PACK;
            else if (sound == DYNAMITE_STICK)
                return DUPLICATE_DYNAMITE_STICK;
            else if (sound == EXPLOSION)
                return DUPLICATE_EXPLOSION;
            else if (sound == TREASURE_CHALICE)
                return DUPLICATE_TREASURE_CHALICE;
            else if (sound == TREASURE_COINS)
                return DUPLICATE_TREASURE_COINS;
            else if (sound == TREASURE_DIAMOND)
                return DUPLICATE_TREASURE_DIAMOND;
            else if (sound == TREASURE_MONEY_BAG)
                return DUPLICATE_TREASURE_MONEY_BAG;

            return null;
        }
    }
}
