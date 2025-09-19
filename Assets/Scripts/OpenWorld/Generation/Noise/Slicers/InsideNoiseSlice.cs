namespace OpenWorld.Generation.Noise.Slicers
{
    public class InsideNoiseSlice : NoiseSlice
    {
        public override bool IsWithinBounds(float value) => minBound <= value && maxBound >= value;
    }
}