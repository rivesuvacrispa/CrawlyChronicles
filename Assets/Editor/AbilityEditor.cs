using Gameplay.Abilities;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Ability), true), CanEditMultipleObjects]
public class AbilityEditor : Editor
{
    private SerializedProperty levelProperty;
    private int previous = -1;
    
    private void OnEnable()
    {
        levelProperty = serializedObject.FindProperty("level");
    }

    public override void OnInspectorGUI() {
        Ability manager = target as Ability;
        if (manager == null) return;
        
        int current = levelProperty.intValue;
        if (previous != current)
        {
            previous = current;
            manager.OnLevelChanged(current);
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Activate"))
        {
            manager.Activate();
        }
    }
}