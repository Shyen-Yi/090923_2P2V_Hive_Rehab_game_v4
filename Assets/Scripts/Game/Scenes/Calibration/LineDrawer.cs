using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Draw(Vector3[] points)
        {
            _lineRenderer.positionCount = points.Length;

            for (var i = 0; i < points.Length; ++i)
            {
                _lineRenderer.SetPosition(i, points[i]);
            }
        }

        public void Activate()
        {
            _lineRenderer.enabled = true;
        }

        public void Deactivate()
        {
            _lineRenderer.enabled = false;
        }
    }
}