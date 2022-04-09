namespace DynamiteDexter
{
    public class Fireworks : Sprite, IExpires
    { 
        private const uint RELEASE_INTERVAL = 10;

        private uint count;
        private Explosion release;

        public bool TimeUp { get { return count == 0; } }
        public Explosion Release { get { return release; } }

        public Fireworks(IGameObject source, uint duration)
            : base(Images.DRUM, source.Center.X - (Game1.TILE_SIZE / 2), source.Center.Y - (Game1.TILE_SIZE / 2))
        {
            count = duration;
            render = false;
        }

        public override void Update()
        {
            if (--count == 0)
                remove = true;

            if (count % RELEASE_INTERVAL == 0)
                release = new Explosion(
                    Game1.rand.Next(Rect.Left - (Rect.Width / 2), Rect.Left + Rect.Width + (Rect.Width / 2)),
                    Game1.rand.Next(Rect.Top - (Rect.Height / 2), Rect.Top + Rect.Height + (Rect.Height / 2)));
            else release = null;

            base.Update();
        }
    }
}
