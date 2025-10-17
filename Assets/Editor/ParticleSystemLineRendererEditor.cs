using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Player;
using UnityEditor;
using UnityEngine;
using Util.Particles;

[CustomEditor (typeof (ParticleSystemLineRenderer))]
public class ParticleSystemLineRendererEditor : Editor {

    public override void OnInspectorGUI() {
        if (target is not ParticleSystemLineRenderer manager) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Update")) manager.UpdateMesh();
    }
}
