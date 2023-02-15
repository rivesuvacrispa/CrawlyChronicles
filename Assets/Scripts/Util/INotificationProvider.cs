using UnityEngine;

namespace Util
{
    public interface INotificationProvider
    {
        public delegate void NotificationProviderEvent();
        public event NotificationProviderEvent OnDataUpdate;
        public event NotificationProviderEvent OnProviderDestroy;
        public Vector2 Position { get; }
        public string NotificationText { get; }
    }
}