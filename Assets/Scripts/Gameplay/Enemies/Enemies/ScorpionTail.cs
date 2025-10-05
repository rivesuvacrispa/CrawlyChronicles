using System.Linq;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Splines;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class ScorpionTail : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private ScorpionSting sting;

        private Spline spline;
        private BezierKnot knot;
        
        public bool TrackPlayer { get; set; }
        
        private void Awake()
        {
            spline = splineContainer.Spline;
            knot = spline.Knots.ToArray()[1];
        }

        private void FixedUpdate()
        {
            if (sting.IsAttacking || (TrackPlayer && sting.CachedDistance > .5f))
                UpdateKnotPosition();
        }
        
        private void UpdateKnotPosition()
        {
            Vector3 stingPos = sting.transform.localPosition;
            knot.Position = stingPos;
            knot.Rotation = Quaternion.LookRotation(
                transform.InverseTransformDirection(sting.LatestDirection), 
                Vector3.back);
            splineContainer.Spline.SetKnot(1, knot);
        }
    }
}