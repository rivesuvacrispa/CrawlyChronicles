using UnityEngine;

namespace Util
{
    public static class ColorUtility
    {
        public static Color WithAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }
    }
}