using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Pedestrian : SpriteSheet
    {
        private enum AnimationColumn { Leftward = 0, Rightward, Upward, Downward }

        private const float ANIMATION_THRESHOLD = 0.01f;

        private static readonly Point playfield = Game1.playfield;

        protected int speed;

        public virtual Rectangle HitBoxTerrain { get { return GetHitBox(0); } }
        public virtual Rectangle HitBoxAssault { get { return GetHitBox(0); } } //Change later?
        public bool ContactWithTerrain { get; set; }

        public Pedestrian(SpriteInfo spriteInfo, CollisionInfo collisionInfo, AnimationInfo animationInfo, int speed)
            : base(spriteInfo, collisionInfo, animationInfo)
        {
            this.speed = speed;
            ContactWithTerrain = false;
        }

        public virtual void MoveUp()
        {
            velocity.X = 0.0f;
            velocity.Y = -speed;
        }
        public virtual void MoveDown()
        {
            velocity.X = 0.0f;
            velocity.Y = speed;
        }
        public virtual void MoveLeft()
        {
            velocity.X = -speed;
            velocity.Y = 0.0f;
        }
        public virtual void MoveRight()
        {
            velocity.X = speed;
            velocity.Y = 0.0f;
        }
        public virtual void Neutral()
        {
            velocity.X = 0.0f;
            velocity.Y = 0.0f;
        }
        public virtual void Reverse()
        {
            velocity.X = -velocity.X;
            velocity.Y = -velocity.Y;
        }

        protected override void Animate()
        {
            if (velocity.X < -ANIMATION_THRESHOLD)
            {
                tileSelection.Y = (int)AnimationColumn.Leftward;
                SubAnimate();
            }
            else if (velocity.X > ANIMATION_THRESHOLD)
            {
                tileSelection.Y = (int)AnimationColumn.Rightward;
                SubAnimate();
            }
            else if (velocity.Y < -ANIMATION_THRESHOLD)
            {
                tileSelection.Y = (int)AnimationColumn.Upward;
                SubAnimate();
            }
            else if (velocity.Y > ANIMATION_THRESHOLD)
            {
                tileSelection.Y = (int)AnimationColumn.Downward;
                SubAnimate();
            }
            else tileSelection.X = 0;

            SetFrame();
        }

        protected virtual void SubAnimate()
        {
            if (++delayCount == interval)
            {
                tileSelection.X = tileSelection.X == 0 ? 1 : 0;
                delayCount = 0;
            }
        }

        protected virtual void BoundsCheck()
        {
            if (x >= playfield.X - Width)
            {
                x = playfield.X - Width - 1;
                Reverse();
            }
            else if (x < 0)
            {
                x = 0;
                Reverse();
            }
            else if (y >= playfield.Y - Height)
            {
                y = playfield.Y - Height - 1;
                Reverse();
            }
            else if (y < 0)
            {
                y = 0;
                Reverse();
            }
            else if (ContactWithTerrain)
                Reverse();
        }

        public virtual void Reset() { } //To ensure compliance with IHotile interface.

        public override void Update()
        {
            if (ContactWithTerrain)
                ContactWithTerrain = false;

            base.Update();
        }
    }
}
