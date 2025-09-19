using DelaunatorSharp;
using Gameplay.Abilities;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (SpriteShapeSplineController), true), CanEditMultipleObjects]
public class SpriteShapeSplineControllerEditor : Editor
{
    public override void OnInspectorGUI() {
        SpriteShapeSplineController manager = target as SpriteShapeSplineController;

        DrawDefaultInspector();

        if (GUILayout.Button("Update"))
        {
            manager.UpdateFromEditor();
        }
    }
}