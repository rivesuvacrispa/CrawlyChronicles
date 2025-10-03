using Gameplay.AI;
using Gameplay.Map;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteSwarmerStateController : AIStateController
    {
        public bool CanPlayFlightAnimation { get; private set; } = true;
        
        protected override void SetEnter()
        {
            if (!TryGetRandomPointAround(MapManager.MapCenter.position, 40, out Vector3 point))
            {
                CanPlayFlightAnimation = false;
                base.SetEnter();
                return;
            }
            
            transform.position = point;
            
            SetEtherial(true);
            SetDefaultReachDistance();
            DisableLocator();
            DoNotRepath();
            enemy.OnMapEntered();
        }
    }
}