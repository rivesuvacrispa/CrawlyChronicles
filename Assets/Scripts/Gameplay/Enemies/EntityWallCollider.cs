using Definitions;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    public class EntityWallCollider : MonoBehaviour
    {
        [SerializeField] private Component listenerComponent;

        private IWallCollisionListener listener;

        private void Awake()
        {
            if (listenerComponent is IWallCollisionListener l)
                listener = l;
            else
            {
                Debug.LogError($"Component {listenerComponent.name} is not IWallCollisionListener");
                enabled = false;
                return;
            }
            listener.EntityWallCollider = this;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(col.collider.gameObject.layer.Equals(GlobalDefinitions.DefaultLayerMask))
                listener.OnWallCollisionEnter();
        }
        
        private void OnCollisionExit2D(Collision2D col)
        {
            if(col.collider.gameObject.layer.Equals(GlobalDefinitions.DefaultLayerMask))
                listener.OnWallCollisionExit();
        }
    }
}