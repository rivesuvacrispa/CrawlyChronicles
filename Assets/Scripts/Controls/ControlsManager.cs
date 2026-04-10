using System.Collections.Generic;
using System.Linq;
using UI.Elements;
using UI.Menus;
using UnityEngine;

namespace Controls
{
    public class ControlsManager : MonoBehaviour
    {
        private readonly KeyCode[] keyCodes = new[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Q,
            KeyCode.R,
            KeyCode.F,
            KeyCode.C,
            KeyCode.Z,
            KeyCode.X,
            KeyCode.T,
            KeyCode.G,
            KeyCode.V,
        };

        private static readonly Stack<KeyCode> KeyChain = new();

        private void Awake()
        {
            ResetKeyChain();
            ActiveAbilityButton.OnKeyReserved += OnKeyReserved;
            ActiveAbilityButton.OnKeyUnreserved += OnKeyUnreserved;
            MainMenu.OnAfterReset += AfterReset;
        }
        
        private void OnDestroy()
        {
            ActiveAbilityButton.OnKeyReserved -= OnKeyReserved;
            ActiveAbilityButton.OnKeyUnreserved -= OnKeyUnreserved;
            MainMenu.OnAfterReset -= AfterReset;
        }

        private void OnKeyUnreserved(KeyCode keycode)
        {
            print($"Unreserved key {keycode}");

            if (!KeyChain.Contains(keycode))
                KeyChain.Push(keycode);
        }

        private void OnKeyReserved(KeyCode keycode)
        {
            print($"Reserved key {keycode}");
        }
        
        private void AfterReset()
        {
            ResetKeyChain();
        }

        private void ResetKeyChain()
        {
            KeyChain.Clear();
            foreach (KeyCode key in keyCodes.Reverse())
            {
                KeyChain.Push(key);
            }
        }

        public static bool TryGetFreeKeyCode(out KeyCode keyCode)
        {
            return KeyChain.TryPop(out keyCode);
        }
    }
}