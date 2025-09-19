using UnityEngine;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class ScarabBeetleArmor : MonoBehaviour
    {
        private BodyPainter painter;

        private void Awake() => painter = GetComponent<BodyPainter>();
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            painter.FadeOut(0.4f);
        }
    }
}