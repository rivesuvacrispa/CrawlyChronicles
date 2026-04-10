using Definitions;
using Environment;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Player;
using Hitboxes;
using UI.Menus;
using UnityEditor;
using UnityEngine;
using Util.Interfaces;

[CustomEditor (typeof (PlayerManager))]
public class PlayerManagerEditor : Editor, IDamageSource 
{

    public override void OnInspectorGUI() {
        if (target is not PlayerManager manager) return;

        DrawDefaultInspector();

        if (GUILayout.Button("Kill"))
        {
            manager.Die(false);
        }
        
        if (GUILayout.Button("Damage"))
        {
            ((IDamageable)manager).Damage(new DamageSource(this), 1f);
        }

        if (GUILayout.Button("Mutate"))
        {
            MutationManager.Instance.MutateFromPlayer();
        }

        if (GUILayout.Button("Add Genes"))
        {
            Vector3 pos = manager.transform.position;
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Aggressive, 100);
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Neutral, 100);
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Defensive, 100);
        }

        if (GUILayout.Button("Lay Eggs"))
        {
            BreedingManager.Instance.LayEggs(PlayerPhysicsBody.Position, 
                new TrioGene(999, 999, 999), 
                AbilityController.GetMutationData());
        }
    }
}