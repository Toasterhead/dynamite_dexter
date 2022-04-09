using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class SpriteSheet : SpritePlus, IGameObjectAnimated
    {
        protected Rectangle sourceRect;

        protected Point sheetDimensions; //Measured in tiles.
        protected Point tileDimensions; //Measured in pixels.
        protected Point tileSelection;

        /*
            Frame-count mode: 
        
            'delayCount' increases until equal to 'interval' and frame is updated 
            before setting 'delayCount' back to zero. Timing is independent of other 
            sprite sheets. Is active when 'timeSync == false'.

            (1 frame per second at 'interval = 60'.)

            Time-sync mode: 
        
            Checks if timestamp from clock is divisible by 'interval' and updates
            frame accordingly. Timing is in sync with other sprite sheets. Is active 
            when 'timeSync == true'.

            (1 frame per second at 'interval = 1000'.)
        */

        protected uint interval;
        protected bool timeSync;
        protected uint delayCount;

        public Rectangle SourceRect { get { return sourceRect; } } //Return deep copy?
        public Point SheetDimensions { get { return sheetDimensions; } }

        public SpriteSheet(
            Texture2D image,
            int x,
            int y,
            int sheetX,
            int sheetY,
            int layer = 0,
            uint interval = 10,
            bool timeSync = false)
            : base(image, x, y, layer)
        {
            sheetDimensions = new Point(sheetX, sheetY);

            if (sheetDimensions.X <= 0 || sheetDimensions.Y <= 0)
                throw new Exception("Error - Sprite sheet dimensions must be greater than zero.");

            tileDimensions = new Point(image.Width / sheetDimensions.X, image.Height / sheetDimensions.Y);
            tileSelection = new Point(0, 0);

            this.x = x;
            this.y = y;
            width = image.Width / sheetDimensions.X;
            height = image.Height / sheetDimensions.Y;

            sourceRect = new Rectangle(
                tileSelection.X * tileDimensions.X,
                tileSelection.Y * tileDimensions.Y,
                tileDimensions.X,
                tileDimensions.Y);

            this.interval = interval;
            delayCount = 0;

            this.timeSync = timeSync;
        }

        public SpriteSheet(
            SpriteInfo spriteInfo,
            SpriteExtraInfo spriteExtraInfo,
            CollisionInfo collisionInfo,
            AnimationInfo animationInfo)
            : base(spriteInfo, spriteExtraInfo, collisionInfo)
        {
            sheetDimensions = new Point(animationInfo.sheetX, animationInfo.sheetY);

            if (sheetDimensions.X <= 0 || sheetDimensions.Y <= 0)
                throw new Exception("Error - Sprite sheet dimensions must be greater than zero.");

            tileDimensions = new Point(image.Width / sheetDimensions.X, image.Height / sheetDimensions.Y);
            tileSelection = new Point(0, 0);

            width = image.Width / sheetDimensions.X;
            height = image.Height / sheetDimensions.Y;

            sourceRect = new Rectangle(
                tileSelection.X * tileDimensions.X,
                tileSelection.Y * tileDimensions.Y,
                tileDimensions.X,
                tileDimensions.Y);

            interval = animationInfo.interval;
            delayCount = 0;

            timeSync = animationInfo.timeSync;
        }

        public SpriteSheet(
            SpriteInfo spriteInfo,
            CollisionInfo collisionInfo,
            AnimationInfo animationInfo)
            : base(spriteInfo, collisionInfo)
        {
            sheetDimensions = new Point(animationInfo.sheetX, animationInfo.sheetY);

            if (sheetDimensions.X <= 0 || sheetDimensions.Y <= 0)
                throw new Exception("Error - Sprite sheet dimensions must be greater than zero.");

            tileDimensions = new Point(image.Width / sheetDimensions.X, image.Height / sheetDimensions.Y);
            tileSelection = new Point(0, 0);

            width = image.Width / sheetDimensions.X;
            height = image.Height / sheetDimensions.Y;

            sourceRect = new Rectangle(
                tileSelection.X * tileDimensions.X,
                tileSelection.Y * tileDimensions.Y,
                tileDimensions.X,
                tileDimensions.Y);

            interval = animationInfo.interval;
            delayCount = 0;

            timeSync = animationInfo.timeSync;
        }

        protected virtual void Animate()
        {
            if (!timeSync)
            {
                //Frame-count Mode

                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= sheetDimensions.X)
                    {
                        tileSelection.X = 0;
                        if (++tileSelection.Y >= sheetDimensions.Y) tileSelection.Y = 0;
                    }
                    SetFrame();
                    delayCount = 0;
                }
            }
            else
            {
                //Time-sync Mode

                long milliseconds = TimeElapsed() % interval;
                long totalFrames = sheetDimensions.X * sheetDimensions.Y;
                long frame = (milliseconds * totalFrames) / interval;
                tileSelection.Y = (int)(frame / sheetDimensions.X);
                tileSelection.X = (int)(frame % sheetDimensions.X);

                SetFrame();
            }
        }

        protected long TimeElapsed()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        protected void SetFrame()
        {
            sourceRect = new Rectangle(
                tileSelection.X * tileDimensions.X,
                tileSelection.Y * tileDimensions.Y,
                tileDimensions.X,
                tileDimensions.Y);
        }

        public override void Update()
        {
            UpdatePosition();
            Animate();
        }
    }
}
