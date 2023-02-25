namespace Util
{
    public static class SoundUtility
    {
        public static float GetRandomPitchTwoSided(float bound)
            => 1 + UnityEngine.Random.value * bound * 2 - bound;
        
        public static float GetRandomPitchLower(float bound)
            => 1 - UnityEngine.Random.value * bound;
        
        public static float GetRandomPitchHigher(float bound)
            => 1 + UnityEngine.Random.value * bound;
    }
}