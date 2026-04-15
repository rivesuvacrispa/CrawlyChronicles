using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Util.Abilities
{
    public readonly struct StatFormatter
    {
        private readonly string suffix;
        private readonly Func<float, float> transformer;
        private readonly bool swapColors;
        private readonly float multiplier;
        private const string COLOR_GREEN = "green";
        private const string COLOR_RED = "red";
        private const string COLOR_ORANGE = "orange";
        
        public StatFormatter(string suffix = "", Func<float, float> transformer = null, bool swapColors = false, float multiplier = 1f)
        {
            this.suffix = suffix;
            this.transformer = transformer ?? (v => v); 
            this.swapColors = swapColors;
            this.multiplier = multiplier;
        }

        public float TransformValue(float v)
        {
            float vv = transformer(v) * multiplier;
            return vv;
        }

        public string FormatString(int currentIndex, int againstIndex, string translationKey)
        {
            string colorUp = swapColors ? COLOR_RED : COLOR_GREEN;
            string colorDown = swapColors ? COLOR_GREEN : COLOR_RED;
            string name =
                LocalizationSettings.StringDatabase.GetLocalizedString(
                    LevelFieldKeys.TABLE_REFERENCE, translationKey);

            return
                $"{{{currentIndex}:cond:>0?<color={COLOR_ORANGE}>{name}:</color> {{{currentIndex}:0.##}}{suffix} " +
                $"{{{againstIndex}:cond:>0?<color={colorUp}>(+{{{againstIndex}:0.##}}{suffix})</color>" +
                $"|=0?|<color={colorDown}>({{{againstIndex}:0.##}}{suffix})</color>}}|}}<br>";
        }

        public StatFormatter WithMultiplier(float mult) => new StatFormatter(suffix, transformer, swapColors, mult);

        public static readonly StatFormatter DECIMAL = new(string.Empty);
        public static readonly StatFormatter PERCENT = new("%", v => Mathf.RoundToInt(v * 100));
        public static readonly StatFormatter DEGREE = new("°");
        public static readonly StatFormatter SECONDS = new("s");
        public static readonly StatFormatter COOLDOWN = new("s", swapColors: true);
    }
}