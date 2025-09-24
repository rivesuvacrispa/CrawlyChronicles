using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Enemies;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Food.VenusFlyTrap
{
    public class VenusFlyTrapHand : MonoBehaviour
    {
        [SerializeField] private VenusFlyTrap flyTrap;
        [SerializeField] private Sprite openedSprite;
        [FormerlySerializedAs("catchedSprite")] 
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private new Collider2D collider;

        private void Awake() => collider = GetComponent<Collider2D>();

        private void Start()
        {
            Close();
            OpenTrapTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private void Close()
        {
            spriteRenderer.sprite = closedSprite;
            collider.enabled = false;
        }

        private void Open()
        {
            spriteRenderer.sprite = openedSprite;
            collider.enabled = true;
        }

        private async UniTaskVoid OpenTrapTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
            Open();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            bool caught = false;
            if (col.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Die();
                caught = true;
            }
            else if (col.gameObject.TryGetComponent(out PlayerManager player))
            {
                player.Die(false);
                caught = true;
            }
            
            if (caught)
            {
                Close();
                flyTrap.Catch();
            }          
        }
    }
}