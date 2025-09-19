using Environment;
using Gameplay.Bosses.Terrorwing;
using UI;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Terrorwing))]
public class TerrorwingEditor : Editor {

    public override void OnInspectorGUI() {
        Terrorwing manager = target as Terrorwing;
        if (manager == null) return;

        DrawDefaultInspector();
        
        if(GUILayout.Button("SetState")) manager.ForcePattern(manager.debug_PatternToSet);
        if(GUILayout.Button("Die")) manager.Die(manager.debug_DieFromPlayer);
    }
}