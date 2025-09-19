namespace Scripts.OpenWorld.Generation.Noise.Layers
{
    public class AdditiveNoiseLayer : NoiseLayer
    {
        protected override float CombineNoise(float left, float right) => left + right;
    }
}