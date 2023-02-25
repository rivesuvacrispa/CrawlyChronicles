using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WTFDebugger : MonoBehaviour
    {
        [SerializeField] private Text text;

        private static Text Text;

        private void Awake() => Text = text;

        public static void SetText(string txt) => Text.text = txt;
    }
}