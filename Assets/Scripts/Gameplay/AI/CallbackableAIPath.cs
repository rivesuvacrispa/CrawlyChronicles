using System;
using Pathfinding;

namespace Gameplay.AI
{
    public class CallbackableAIPath : AIPath
    {
        public Action Callback { get; set; }

        public override void OnTargetReached()
        {
            Callback?.Invoke();
        }
    }
}