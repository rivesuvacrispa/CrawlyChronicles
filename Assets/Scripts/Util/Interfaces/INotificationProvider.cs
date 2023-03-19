namespace Util.Interfaces
{
    public interface INotificationProvider : ITransformProvider
    {
        public delegate void NotificationProviderEvent();
        public event NotificationProviderEvent OnDataUpdate;
        public string NotificationText { get; }
    }
}