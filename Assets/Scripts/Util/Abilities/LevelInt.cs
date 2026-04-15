using Gameplay.Mutations;
using UnityEngine;

namespace Util.Abilities
{
    [System.Serializable]
    public class LevelInt : LevelField<Vector2Int, int>
    {
        public override int AtLvl(int lvl)
        {
            return BasicAbility.LerpLevel(value.x, value.y, lvl);
        }
        
        public int AtLvlFloor(int lvl)
        {
            return BasicAbility.LerpLevelFloor(value.x, value.y, lvl);
        }


        public LevelInt(Vector2Int v) : base(v)
        {
        }
        
        public LevelInt(int a, int b) : base(new Vector2Int(a, b)){ }
    }
}