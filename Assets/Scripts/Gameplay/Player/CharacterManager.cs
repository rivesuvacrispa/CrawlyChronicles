using Scriptable;
using UnityEngine;

namespace Gameplay.Player
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private Character[] allCharacters;
        [SerializeField, Header("Default Character")] public Character currentCharacter;
        
        
#if UNITY_EDITOR
        [SerializeField, Header("Debug")] public Character debug_CharacterToSet;
        
#endif

        public delegate void CharacterChangeEvent(Character selected);
        public static event CharacterChangeEvent OnCharacterSelected;
        public static Character CurrentCharacter { get; private set; }
        
        

        private void Start()
        {
            SelectCharacter(currentCharacter);
        }

        public void SelectCharacter(Character character)
        {
            currentCharacter = character;
            CurrentCharacter = character;
            OnCharacterSelected?.Invoke(character);
        }
    }
}