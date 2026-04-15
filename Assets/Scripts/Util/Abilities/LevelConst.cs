namespace Util.Abilities
{
    [System.Serializable]
    public class LevelConst : LevelField<float, float>
    {
        public LevelConst(float v) : base(v)
        {
        }

        public override float AtLvl(int lvl)
        {
            return value;
        }
    }
}