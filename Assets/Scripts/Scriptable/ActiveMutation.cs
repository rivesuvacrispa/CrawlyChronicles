using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Mutations/Active ability")]
    public class ActiveMutation : BasicMutation
    {
        [SerializeField] private KeyCode keyCode;
        [SerializeField] private float cooldownLvl1;
        [SerializeField] private float cooldownLvl10;

        public float GetCooldown(int level) =>
            Mathf.Lerp(cooldownLvl1, cooldownLvl10, level / 9f);
        
        public KeyCode KeyCode => keyCode;
    }
}