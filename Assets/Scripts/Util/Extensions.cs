using System.Text;
using Definitions;
using UnityEngine;

namespace Util
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        public static Gradient FastGradient(this Gradient g, Color a, Color b)
        {
            g.SetKeys(
                new[]
                {
                    new GradientColorKey(a, 0),
                    new GradientColorKey(b, 1)
                }, new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1),
                });
            return g;
        }

        public static void PlayDead(this BodyPainter painter, int sortOrder)
        {
            painter.SetSortingLayer("Ground");
            painter.SetSortingOrder(sortOrder);
            painter.SetMaterial(GlobalDefinitions.DefaultSpriteMaterial);
            painter.Paint(GlobalDefinitions.DeathGradient, 1f);
            if(!painter.TryGetComponent(out Rigidbody2D partRb)) 
                partRb = painter.gameObject.AddComponent<Rigidbody2D>();
            partRb.transform.localScale = Vector3.one;
            partRb.simulated = true;
            partRb.gravityScale = 0;
            partRb.drag = 1f;
            partRb.angularDrag = 2f;
            partRb.angularVelocity = 720f;
            partRb.AddForce(Random.insideUnitCircle.normalized * 2f, 
                ForceMode2D.Impulse);
        }

        public static StringBuilder AddAbilityLine(
            this StringBuilder sb, 
            string title, 
            float value, 
            float previousValue, 
            bool withUpgradePlus = true, 
            bool percent = false,
            string prefix = "",
            string suffix = "")
        {
            sb.Append("<color=orange>")
                .Append(title)
                .Append(": ")
                .Append("</color>")
                .Append(prefix)
                .Append(percent ? $"{(int)(value * 100)}%" : value.ToString("0.##"))
                .Append(suffix);
            
            if (previousValue != 0)
            {
                float diff = value - previousValue;
                sb.Append(" <color=lime>(")
                    .Append(withUpgradePlus ? "+" : string.Empty)
                    .Append(percent ? $"{(int)(diff * 100)}%" : diff.ToString("0.##"))
                    .Append(suffix)
                    .Append(")")
                    .Append("</color>");
            }
            
            return sb.Append("\n");
        }

        public static StringBuilder AppendColored(this StringBuilder sb, string color, string text) 
            => sb.Append($"<color={color}>{text}</color>");
    }
}