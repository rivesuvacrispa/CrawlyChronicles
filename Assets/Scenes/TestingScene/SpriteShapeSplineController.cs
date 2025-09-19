using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace DelaunatorSharp
{
    public class SpriteShapeSplineController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private VoronoyGenerator generator;
        [SerializeField] private SpriteShape spriteShapeProfile;
        [Header("Options")]
        [SerializeField] private float scale = 1f;
        [SerializeField] private int maxShapes = 20;

        private SpriteShapeController[] shapeControllers;


        public void UpdateFromEditor()
        {
            CreateShapes(generator.VoronoiCells);
        }


        public void CreateShapes(VoronoiCell[] cells)
        {
            CleanupShapes();

            var validCells = cells.Where(c => c.Points.Length >= 3).ToArray();
            var shapesAmount = validCells.Length;
            shapeControllers = new SpriteShapeController[shapesAmount];
            for (var i = 0; i < shapesAmount; i++)
            {
                var cell = validCells[i];
                shapeControllers[i] = CreateShape(cell);
                
                if (i == maxShapes - 1) return;
            }
        }

        private void CleanupShapes()
        {
            Transform t = transform;
            var toCleanup = new List<GameObject>(t.childCount);
            toCleanup.AddRange(from Transform c in t select c.gameObject);

            foreach (GameObject o in toCleanup) DestroyImmediate(o);
            shapeControllers = null;
        }

        private SpriteShapeController CreateShape(VoronoiCell cell)
        {
            GameObject root = new GameObject($"ShapeController {cell.Index}");
            SpriteShapeController shapeController = root.AddComponent<SpriteShapeController>();
            root.transform.SetParent(transform);

            shapeController.splineDetail = 16;
            shapeController.spriteShape = spriteShapeProfile;
            shapeController.fillPixelsPerUnit = 128;
            
            if (cell.Biome is not null)
            {
                if (cell.Biome.SpriteShape is not null)
                    shapeController.spriteShape = cell.Biome.SpriteShape;
                else 
                    shapeController.spriteShapeRenderer.color = cell.Biome.Color;
            }

            CreateSpline(shapeController, cell);
            
            return shapeController;
        }

        private void CreateSpline(SpriteShapeController controller, VoronoiCell cell)
        {
            Spline s = controller.spline;
            s.Clear();
            var points = cell.Points.Reverse().ToArray();

            Point GetRelativePoint(int index)
            {
                int len = points.Length;
                return points[(index + len) % len];
            }

            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i].AsV2();

                try
                {
                    s.InsertPointAt(i, point * scale);
                    
                    Vector2 prevPoint = GetRelativePoint(i - 1).AsV2();
                    s.SetTangentMode(i, ShapeTangentMode.Continuous);
                    Vector2 nextPoint = GetRelativePoint(i + 1).AsV2();
                    s.SetLeftTangent(i, (prevPoint - point).normalized * 0.25f);
                    s.SetRightTangent(i, (nextPoint - point).normalized * 0.25f);
                }
                catch (ArgumentException e)
                {
                    
                }


            }

            // controller.BakeMesh();
        }
    }
}