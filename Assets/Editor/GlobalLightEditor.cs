using Environment;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (GlobalLight))]
public class GlobalLightEditor : Editor {

    public override void OnInspectorGUI() {
        GlobalLight manager = target as GlobalLight;
        if (manager == null) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Set day")) manager.SetInstantly(false);
        if (GUILayout.Button ("Set night")) manager.SetInstantly(true);
    }
}