namespace Scripts.OpenWorld.Generation.Noise.Layers
{
    public class MultitiveNoiseLayer : NoiseLayer
    {
        protected override float CombineNoise(float left, float right) => left * right;
    }
}