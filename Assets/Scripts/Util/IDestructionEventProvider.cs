namespace Util
{
    public interface IDestructionEventProvider
    {
        public delegate void DestructionProviderEvent();
        public event DestructionProviderEvent OnProviderDestroy;
    }
}