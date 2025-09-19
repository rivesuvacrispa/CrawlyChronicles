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
            partRb.linearDamping = 1f;
            partRb.angularDamping = 2f;
            partRb.angularVelocity = 720f;
            partRb.AddForce(Random.insideUnitCircle.normalized * 2f, 
                ForceMode2D.Impulse);
        }
    }
}