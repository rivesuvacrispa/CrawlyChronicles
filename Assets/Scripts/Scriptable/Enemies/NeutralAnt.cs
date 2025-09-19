using Gameplay.Genes;
using UnityEngine;

namespace Scriptable.Enemies
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy/Neutral")]
    public class NeutralAnt : Enemy
    {
        [SerializeField] private GeneType geneType;

        public GeneType GeneType => geneType;
    }
}