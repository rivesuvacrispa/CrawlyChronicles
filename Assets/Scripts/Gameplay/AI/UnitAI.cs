using Pathfinding;
using UnityEngine;

namespace Gameplay.AI
{
    [RequireComponent(typeof(CallbackableAIPath)),
     RequireComponent(typeof(AIDestinationSetter)),
     RequireComponent(typeof(Seeker))]
    public class UnitAI : MonoBehaviour
    {
        private CallbackableAIPath aiPath;
        private AIDestinationSetter destinationSetter;
        private Seeker seeker;

        private void Awake()
        {
            seeker = GetComponent<Seeker>();
            aiPath = GetComponent<CallbackableAIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            destinationSetter.enabled = false;
            aiPath.enabled = false;
        }

        public void SetDestination(Vector3 pos)
        {
            seeker.StartPath(transform.position, pos, p => aiPath.SetPath(p));
        }
    }
}