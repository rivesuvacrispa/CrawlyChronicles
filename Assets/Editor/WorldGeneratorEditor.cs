using Gameplay.Abilities;
using Scripts.OpenWorld.Generation;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldGenerator manager = target as WorldGenerator;
        if (manager == null) return;
        DrawDefaultInspector();

        if (GUILayout.Button("Clear")) manager.Clear();
        if (GUILayout.Button("Generate")) manager.Generate();
        if (GUILayout.Button("Update ShadowCaster")) manager.UpdateShadowCaster();
    }
}