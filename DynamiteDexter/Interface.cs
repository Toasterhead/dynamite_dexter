using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public interface IProjectile
    {
        bool ContactWithWall { get; set; }
        bool ContactWithPlayer { get; set; }
        IGameObject Parent { get; }
    }

    public interface IResets
    {
        void Reset();
    }

    public interface IHostile : IResets
    {
        int StartX { get; }
        int StartY { get; }
    }

    public interface IHasAttachments
    {
        List<IGameObject> Attachments { get; }
    }

    public interface IBoss : IHostile
    {
        bool Flashing { get; }
        bool Remove { get; }
        int Vitality { get; }
        void Strike();
        Rectangle HitBoxAssault { get; }
        Sprite AppearingForm { get; }
    }

    public interface IHunts
    {
        IGameObject Target { get; }
    }

    public interface ISeeks : IHunts
    {
        void UpdatePath();
        void AcquireGraph(Graph graph);
        bool DynamicGraph { get; }
        uint PathUpdateInterval { get; }
    }

    public interface INavigates
    {
        void AcquireSet(List<IGameObject> spriteSet);
    }

    public interface IFires
    {
        IProjectile[] Chamber { get; }
        void EmptyChamber();
    }

    public interface IOutlined
    {
        OutlineMask TheOutlineMask { get; }
        void SyncOutline();
    }

    public interface IExpires
    {
        bool TimeUp { get; }
    }

    public interface IPickup { }
    public interface IVulnerable { }
    public interface IDestructive { }
}
