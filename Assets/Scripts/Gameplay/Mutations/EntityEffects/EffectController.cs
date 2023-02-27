using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Abilities.EntityEffects
{
    [RequireComponent(typeof(Enemy))]
    public class EffectController : MonoBehaviour
    {
        private Enemy enemy;
        private GameObject effectsGO;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            effectsGO = new GameObject("Effects");
            effectsGO.transform.SetParent(transform);
        }

        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            EntityEffect effect = effectsGO.TryGetComponent(out effect) ? 
                effect : 
                effectsGO.AddComponent<T>().Init(enemy);
            effect.Refresh(data);
        }

        public void ClearAll()
        {
            foreach (EntityEffect effect in effectsGO.GetComponents<EntityEffect>()) 
                effect.Cancel();
        }
    }
}