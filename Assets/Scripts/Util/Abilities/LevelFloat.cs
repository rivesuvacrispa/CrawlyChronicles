using Gameplay.Mutations;
using UnityEngine;

namespace Util.Abilities
{
    [System.Serializable]
    public class LevelFloat : LevelField<Vector2, float>
    {
        public override float AtLvl(int lvl)
        {
            return BasicAbility.LerpLevel(value.x, value.y, lvl);
        }

        public LevelFloat(Vector2 v) : base(v)
        {
        }

        public LevelFloat(float a, float b) : base(new Vector2(a, b)){ }
    }
}