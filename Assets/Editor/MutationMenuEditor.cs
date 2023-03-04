using Environment;
using Scripts.Gameplay.Bosses.Terrorwing;
using UI;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Terrorwing))]
public class MutationMenuEditor : Editor {

    public override void OnInspectorGUI() {
        Terrorwing manager = target as Terrorwing;
        if (manager == null) return;

        DrawDefaultInspector();
        
        if(GUILayout.Button("SetState")) manager.ForcePattern(manager.debug_PatternToSet);
    }
}