using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class LineDrawer
    /// @brief A utility for drawing lines in the scene using Unity's LineRenderer component.
    /// 
    /// The `LineDrawer` class provides functionality to draw lines in the game scene by utilizing Unity's `LineRenderer` component.
    /// It allows for defining a series of points and visually connecting them with a line, enabling the creation of paths, ropes,
    /// or any other linear representations in the scene. It also provides methods for enabling or disabling the line's visibility.
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        /// <summary>
        /// Initializes the LineRenderer component.
        /// </summary>
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        /// <summary>
        /// Draws a line by setting a series of points.
        /// </summary>
        /// <param name="points">An array of points that define the line's path.</param>
        public void Draw(Vector3[] points)
        {
            _lineRenderer.positionCount = points.Length;

            for (var i = 0; i < points.Length; ++i)
            {
                _lineRenderer.SetPosition(i, points[i]);
            }
        }

        /// <summary>
        /// Activates the LineRenderer to make the line visible in the scene.
        /// </summary>
        public void Activate()
        {
            _lineRenderer.enabled = true;
        }

        /// <summary>
        /// Deactivates the LineRenderer to hide the line from the scene.
        /// </summary>
        public void Deactivate()
        {
            _lineRenderer.enabled = false;
        }
    }
}