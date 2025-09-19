using Gameplay.Mutations;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BasicAbility), true), CanEditMultipleObjects]
public class AbilityEditor : Editor
{
    private SerializedProperty levelProperty;
    private int previous = -1;
    
    private void OnEnable()
    {
        levelProperty = serializedObject.FindProperty("level");
    }

    public override void OnInspectorGUI() {
        BasicAbility manager = target as BasicAbility;
        if (manager == null) return;
        
        int current = levelProperty.intValue;
        if (previous != current)
        {
            previous = current;
            manager.OnLevelChanged(current);
        }

        DrawDefaultInspector();

        if (manager is ActiveAbility activeAbility &&
            GUILayout.Button("Activate"))
        {
            activeAbility.Activate();
        }
    }
}