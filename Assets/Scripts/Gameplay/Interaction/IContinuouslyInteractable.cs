namespace Gameplay.Interaction
{
    public interface IContinuouslyInteractable : IInteractable
    {
        public void OnInteractionStart();
        public void OnInteractionStop();
        public float InteractionTime { get; }
    }
}