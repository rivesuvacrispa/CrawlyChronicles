using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Effects.DamageText
{
    public readonly struct DamageTextArguments
    {
        public readonly DamageInstance damageInstance;
        public readonly Vector3 position;

        public DamageTextArguments(Vector3 position, DamageInstance damageInstance)
        {
            this.position = position;
            this.damageInstance = damageInstance;
        }
    }
}