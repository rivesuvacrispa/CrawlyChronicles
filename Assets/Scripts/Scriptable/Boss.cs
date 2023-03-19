using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Boss")]
    public class Boss : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private string encounterTitle;
        [SerializeField] private int genesReward;
        [SerializeField] private BasicMutation mutationReward;

        public string Name => name;
        public int GenesReward => genesReward;
        public BasicMutation MutationReward => mutationReward;
        public string EncounterTitle => encounterTitle;
    }
}