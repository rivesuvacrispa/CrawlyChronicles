using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Selectable/Basic")]
    public class SelectableObject : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private new string name;

        public Sprite Sprite => sprite;
        public string Name => name;
    }
}