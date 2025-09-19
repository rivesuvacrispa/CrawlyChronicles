using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scripts.OpenWorld.ShadowCasters
{
    public static class ShadowCaster2DExtensions
    {
        /// <summary>
        /// Replaces the path that defines the shape of the shadow caster.
        /// </summary>
        /// <remarks>
        /// Calling this method will change the shape but not the mesh of the shadow caster. Call SetPathHash afterwards.
        /// </remarks>
        /// <param name="shadowCaster">The object to modify.</param>
        /// <param name="path">The new path to define the shape of the shadow caster.</param>
        public static void SetPath(this ShadowCaster2D shadowCaster, Vector3[] path)
        {
            FieldInfo shapeField = typeof(ShadowCaster2D).GetField("m_ShapePath",
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            shapeField.SetValue(shadowCaster, path);
        }
 
        /// <summary>
        /// Replaces the hash key of the shadow caster, which produces an internal data rebuild.
        /// </summary>
        /// <remarks>
        /// A change in the shape of the shadow caster will not block the light, it has to be rebuilt using this function.
        /// </remarks>
        /// <param name="shadowCaster">The object to modify.</param>
        /// <param name="hash">The new hash key to store. It must be different from the previous key to produce the rebuild. You can use a random number.</param>
        public static void SetPathHash(this ShadowCaster2D shadowCaster, int hash)
        {
            FieldInfo hashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash",
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            hashField.SetValue(shadowCaster, hash);
        }
        
        public static void SetRenderer(this ShadowCaster2D shadowCaster, Renderer renderer)
        {
            FieldInfo hasRendererField = typeof(ShadowCaster2D).GetField("m_HasRenderer",
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            hasRendererField.SetValue(shadowCaster, true);
            
            FieldInfo rendererField = typeof(ShadowCaster2D).GetField("m_Renderer",
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            rendererField.SetValue(shadowCaster, renderer);
        }
    }
}