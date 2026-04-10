namespace Util
{
    public class FloatWrapper
    {
        public float Value { get; private set; }
        
        public FloatWrapper(float v)
        {
            Value = v;
        }

        public void Add(float v)
        {
            Value += v;
        }

        public void Sub(float v)
        {
            Value -= v;
        }
    }
}