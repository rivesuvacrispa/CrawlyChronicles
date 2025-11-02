using Gameplay;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Player;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BreedingManager))]
public class BreedingManagerEditor : Editor {

    public override void OnInspectorGUI() {
        BreedingManager manager = target as BreedingManager;
        if (manager == null) return;

        DrawDefaultInspector();

        if (GUILayout.Button ("Lay eggs")) manager
            .LayEggs(PlayerPhysicsBody.Position, 
                new TrioGene(999, 999, 999), 
                AbilityController.GetMutationData());
    }
}