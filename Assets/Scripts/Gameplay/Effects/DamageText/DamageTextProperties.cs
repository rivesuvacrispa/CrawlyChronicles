using TMPro;
using UnityEngine;

namespace Gameplay.Effects.DamageText
{
    public class DamageTextProperties
    {
        public readonly VertexGradient? color;
        public readonly TMP_FontAsset font;
        public readonly float size;
        public readonly float lifetime;

        public DamageTextProperties(VertexGradient? color, TMP_FontAsset font, float relativeSize, float relativeLifetime)
        {
            this.color = color;
            this.font = font;
            size = relativeSize;
            lifetime = relativeLifetime;
        }
    }
}