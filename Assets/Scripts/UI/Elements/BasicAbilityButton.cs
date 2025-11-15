using DG.Tweening;
using Gameplay.Mutations;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;
using Util;


namespace UI.Elements
{
    public class BasicAbilityButton : AbstractAbilityButton<BasicAbility, BasicMutation>
    {
        [SerializeField] private Image background;

        private const float duration = 0.75f;
        
        public void PlayDowngrade(int lvl)
        {
            UpdateLevelText(lvl);
            DOTween.Sequence()
                .Insert(0, icon.transform.DOShakeScale(duration, Vector2.one, 15))
                .Insert(0, icon.transform.DOShakePosition(duration, Vector3.one, 15))
                .SetUpdate(true);
        }

        public void PlayBreak()
        {
            DOTween.Sequence()
                .Insert(0, icon.transform.DOShakeScale(duration, Vector2.one * 2f, 15))
                .Insert(0, icon.transform.DOShakePosition(duration, Vector3.one * 3f, 30))
                .Insert(0, icon.DOColor(icon.color.WithAlpha(0f), duration))
                .Insert(0, background.DOColor(background.color.WithAlpha(0f), duration))
                .Insert(0, background.transform.DOScale(Vector3.one * 2, duration))
                .SetUpdate(true)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}