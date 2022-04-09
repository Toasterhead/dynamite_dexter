using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public interface IGameObject
    {
        Texture2D Image { get; }
        Rectangle Rect { get; }
        bool Render { get; set; }
        bool Remove { get; set; }
        int Layer { get; set; }

        int X { get; }
        int Y { get; }
        int Z { get; }
        int Width { get; }
        int Height { get; }

        int Left { get; }
        int Right { get; }
        int Top { get; }
        int Bottom { get; }
        Point TopLeft { get; }
        Point TopRight { get; }
        Point BottomLeft { get; }
        Point BottomRight { get; }
        Point Center { get; }

        void Translate(int x, int y);
        void Reposition(int x, int y);

        void Update();
    }

    public interface IGameObjectPlus : IGameObject
    {
        Texture2D[] ImageSet { get; }
        Rectangle[] HitBox { get; }
        Point[] CollisionPoint { get; }

        Color TheColor { get; }
        SpriteEffects Orientation { get; }
        float Alpha { get; set; }

        int NumHitBoxes { get; }
        int NumCollisionPoints { get; }

        Rectangle GetHitBox(int i, bool absolute = true);
        Point GetCollisionPoint(int i, bool absolute = true);

        int HitBoxTop(int i);
        int HitBoxRight(int i);
        int HitBoxBottom(int i);
        int HitBoxLeft(int i);

        void SetHitBoxTop(int i, int newValue, bool absolute = false);
        void SetHitBoxRight(int i, int newValue, bool absolute = false);
        void SetHitBoxBottom(int i, int newValue, bool absolute = false);
        void SetHitBoxLeft(int i, int newValue, bool absolute = false);
    }

    public interface IGameObjectAnimated : IGameObjectPlus
    {
        Rectangle SourceRect { get; }
        Point SheetDimensions { get; }
    }

}