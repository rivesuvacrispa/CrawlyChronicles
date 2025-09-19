namespace OpenWorld.Generation.Noise.Layers
{
    public class SubstructiveNoiseLayer : NoiseLayer
    {
        protected override float CombineNoise(float left, float right) => left - right;
    }
}