using Environment;
using UI;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MutationMenu))]
public class MutationMenuEditor : Editor {

    public override void OnInspectorGUI() {
        MutationMenu manager = target as MutationMenu;
        if (manager == null) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Add mutation to egg")) 
            manager.HatchingEgg.MutationData.Add(manager.debug_MutationToAdd, manager.debug_MutationLevelToAdd);

        if(GUILayout.Button("Roll")) manager.Refresh();
        if(GUILayout.Button("Clear"))
        {
            manager.ClearAll();
            manager.ResetEgg();
        }
        if(GUILayout.Button("Save"))
        {
            manager.Save();
        }
    }
}