using System.Text;
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
    }
}