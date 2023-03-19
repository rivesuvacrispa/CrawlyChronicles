using Gameplay.Enemies;

namespace Util.Interfaces
{
    public interface IWallCollisionListener
    {
        public EntityWallCollider EntityWallCollider { get; set; }
        public void OnWallCollisionEnter();
        public void OnWallCollisionExit();
    }
}