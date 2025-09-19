using System;
using Pathfinding;
using UnityEngine;

namespace Gameplay.AI
{
    public class CallbackableAIPath : AIPath
    {
        private Action callback;
        
        public Action Callback
        {
            get => callback;
            set
            {
                Debug.Log($"Ai Path [{GetHashCode()}] callback set to {value}");
                callback = value;
            }
        }

        public override void OnTargetReached()
        {
            Callback?.Invoke();
        }
    }
}