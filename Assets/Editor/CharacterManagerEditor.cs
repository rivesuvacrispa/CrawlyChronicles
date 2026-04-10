using Gameplay.Player;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (CharacterManager), true)]
public class CharacterManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        CharacterManager manager = target as CharacterManager;
        if (manager == null) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Set Character")) manager.SelectCharacter(manager.debug_CharacterToSet);
    }
}
