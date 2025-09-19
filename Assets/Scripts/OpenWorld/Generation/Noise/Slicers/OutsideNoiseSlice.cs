namespace Scripts.OpenWorld.Generation.Noise.Slicers
{
    public class OutsideNoiseSlice : NoiseSlice
    {
        public override bool IsWithinBounds(float value) => minBound >= value || maxBound <= value;
    }
}