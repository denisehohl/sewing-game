using System.Collections.Generic;
using UnityEngine;

namespace Moreno.SewingGame
{
    public class ThreadVisualizer : MonoBehaviour
    {
        [SerializeField] 
        private List<Transform> points = new List<Transform>();
        [SerializeField] 
        private LineRenderer lineRenderer;

        void Awake()
        {
            // Ensure we have a LineRenderer reference, if not set in Inspector
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
        }

        private void Update()
        {
            if (points == null || points.Count == 0 || lineRenderer == null)
                return;

            // Filter only active Transforms
            List<Vector3> activePositions = new List<Vector3>();

            foreach (Transform t in points)
            {
                if (t != null && t.gameObject.activeInHierarchy)
                {
                    activePositions.Add(t.position);
                }
            }

            lineRenderer.positionCount = activePositions.Count;

            for (int i = 0; i < activePositions.Count; i++)
            {
                lineRenderer.SetPosition(i, activePositions[i]);
            }
        }
    }
}