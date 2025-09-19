using Definitions;
using UnityEngine;

namespace Gameplay.Bosses.AntColony
{
    [RequireComponent(typeof(Collider2D))]
    public class AntQueenWallTrigger : MonoBehaviour
    {
        [SerializeField] private AntQueen antQueen;

        private new Collider2D collider;

        public void SetEnabled(bool isEnabled) => collider.enabled = isEnabled;
        
        private void Awake()
        {
            enabled = false;
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer.Equals(GlobalDefinitions.DefaultLayerMask)) 
                antQueen.TouchingWalls = true;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if(col.gameObject.layer.Equals(GlobalDefinitions.DefaultLayerMask))
                antQueen.TouchingWalls = false;
        }
    }
}