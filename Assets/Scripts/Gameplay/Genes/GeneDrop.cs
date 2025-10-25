using Definitions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;

namespace Gameplay.Genes
{
    public class GeneDrop : PickupableObject
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GeneType geneType = GeneType.Neutral;
        [SerializeField] private int amount = -1;

        public delegate void GeneDropEvent(GeneType geneType, int amount);
        public static event GeneDropEvent OnPickedUp;
        public delegate GeneType GeneDropReplaceTypeEvent();

        public static GeneDropReplaceTypeEvent OnGeneDropReplaceTypeRequested;
        
        
        
        protected override void OnPickup()
        {
            OnPickedUp?.Invoke(geneType, amount);
        }
        
        
        public GeneDrop SetData(GeneType newType, int count)
        {
            GeneType? t = OnGeneDropReplaceTypeRequested?.Invoke();
            if (t is not null) newType = t.Value;
            
            amount = count;
            geneType = newType;
            Color geneColor = GlobalDefinitions.GetGeneColor(newType);
            light.color = geneColor;
            spriteRenderer.color = geneColor;
            return this;
        }
    }
}