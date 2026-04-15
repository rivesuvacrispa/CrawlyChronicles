using UnityEngine;
using Util.Abilities;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Mutations/Active ability")]
    public class ActiveMutation : BasicMutation
    {
        [SerializeField] private float cooldownLvl1;
        [SerializeField] private float cooldownLvl10;

        public float GetCooldown(int level) =>
            Mathf.Lerp(cooldownLvl1, cooldownLvl10, level / 9f);

        public ILevelField Cooldown
            => new LevelFloat(new Vector2(cooldownLvl1, cooldownLvl10))
                .UseKey(LevelFieldKeys.COOLDOWN)
                .UseFormatter(StatFormatter.COOLDOWN);
    }
}