using System;
using Camera;
using UnityEngine;

namespace Util.Particles
{
    public class ParticleSystemLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private new ParticleSystem particleSystem;
        
        private Mesh m;
        public ParticleSystem ParticleSystem => particleSystem;
        public bool IsPlaying => particleSystem.isPlaying;
        public bool PlayOnAwake { get; set; }
        

        public void Stop()
        {
            particleSystem.Stop();
        }

        private void OnEnable()
        {
            if (PlayOnAwake) particleSystem.Play();
        }

        public void Play(Vector3[] positions)
        {
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            UpdateMesh();
        }

        public void UpdateMesh()
        {
            if (MainCamera.Instance is null) return;
            UnityEngine.Camera cam = MainCamera.Camera;
            if (cam is null || lineRenderer.positionCount == 0) return;
            
            m = new Mesh();
            lineRenderer.BakeMesh(m, cam);
            
            var shapeModule = particleSystem.shape;
            shapeModule.mesh = m;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;

            var emissionModule = particleSystem.emission;
            
            emissionModule.SetBurst(0, new ParticleSystem.Burst(0, m.triangles.Length / 3 + 2));
            
            particleSystem.Play();
        }
    }
}