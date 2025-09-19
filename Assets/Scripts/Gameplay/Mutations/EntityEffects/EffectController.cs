using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.EntityEffects
{
    public class EffectController : MonoBehaviour
    {
        private IImpactEffectAffectable target;
        private GameObject effectsGO;

        private void Awake()
        {
            target = GetComponent<IImpactEffectAffectable>();
            effectsGO = new GameObject("Effects");
            effectsGO.transform.SetParent(transform);
        }

        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            EntityEffect effect = effectsGO.TryGetComponent(out effect) ? 
                effect : 
                effectsGO.AddComponent<T>().SetTarget(target);
            effect.Refresh(data);
        }

        public void ClearAll()
        {
            foreach (EntityEffect effect in effectsGO.GetComponents<EntityEffect>()) 
                effect.Cancel();
        }
    }
}