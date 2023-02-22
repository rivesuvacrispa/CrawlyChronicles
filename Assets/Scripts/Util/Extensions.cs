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

        public static StringBuilder AddAbilityLine(this StringBuilder sb, string title, float value, float previousValue, bool withPlus = true)
        {
            sb.Append("<color=orange>")
                .Append(title)
                .Append(": ")
                .Append("</color>")
                .Append(value.ToString("n1"));
            
            if (previousValue != 0) 
                sb.Append(" <color=lime>(")
                .Append(withPlus ? "+" : string.Empty)
                .Append((value - previousValue).ToString("n1"))
                .Append(")</color>");
            
            return sb.Append("\n");
        }
    }
}