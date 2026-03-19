using Gameplay.Effects.BroodSpider;

namespace Gameplay.Mutations.EntityEffects.BroodInfection
{
    public class BroodInfectionEffectData : EntityEffectData
    {
        public float BaseSpawnChance { get; }
        public float BaseSpiderAmount { get; }
        public BroodSpiderArguments SpiderArguments { get; }

        public BroodInfectionEffectData(
            int duration,
            float baseSpawnChance,
            float baseSpiderAmount,
            BroodSpiderArguments args
        ) : base(duration)
        {
            BaseSpawnChance = baseSpawnChance;
            BaseSpiderAmount = baseSpiderAmount;
            SpiderArguments = args;
        }
    }
}