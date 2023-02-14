using UnityEngine;

namespace Util
{
    public static class ColorUtility
    {
        public static UnityEngine.Color WithAlpha(this UnityEngine.Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }
    }
}