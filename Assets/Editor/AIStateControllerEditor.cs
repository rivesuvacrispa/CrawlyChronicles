using Gameplay.AI;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (AIStateController))]
public class AIStateControllerEditor : Editor {

    public override void OnInspectorGUI() {
        AIStateController manager = target as AIStateController;
        if (manager == null) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Update state")) manager.SetState(manager.debug_AIState);
    }
}