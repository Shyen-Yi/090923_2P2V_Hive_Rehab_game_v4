using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public enum CalibrationMarkerCircle
    {
        Circle1 = 1,
        Circle2 = 2,
        Circle3 = 3,
    }

    public class CalibrationMarkerController
    {
        private GeneralWidgetConfig _config;

        #region Extra
        private enum ExtraRenderer
        {
            Circle1 = 0,
            Circle2 = 1,
            Circle3 = 2,
        }

        private SpriteRenderer _circle1Renderer;
        private SpriteRenderer _circle2Renderer;
        private SpriteRenderer _circle3Renderer;
        #endregion

        public CalibrationMarkerController(GeneralWidgetConfig config)
        {
            _config = config;

            InitExtra();
        }

        private void InitExtra()
        {
            _circle1Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle1];
            _circle2Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle2];
            _circle3Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle3];
        }

        public void Activate()
        {
            _config.gameObject.SetActive(true);
        }

        public void SetWorldPosition(Vector3 worldPos)
        {
            _config.transform.position = new Vector3(worldPos.x, worldPos.y, _config.transform.position.z);
        }

        public void Deactivate()
        {
            ResetAllCircles();
            _config.gameObject.SetActive(false);
        }

        public void SetCircleFill(int circle, float fill)
        {
            if (Enum.IsDefined(typeof(CalibrationMarkerCircle), circle))
            {
                var pCircle = (CalibrationMarkerCircle)circle;
                fill = Mathf.Clamp01(fill);
                SetRadialFill(GetRenderer(pCircle).material, fill);
            }
        }

        public void ResetAllCircles()
        {
            SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle1).material, 0);
            SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle2).material, 0);
            SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle3).material, 0);
        }

        private SpriteRenderer GetRenderer(CalibrationMarkerCircle circle)
        {
            switch (circle)
            {
                case CalibrationMarkerCircle.Circle1:
                    return _circle1Renderer;
                case CalibrationMarkerCircle.Circle2:
                    return _circle2Renderer;
                case CalibrationMarkerCircle.Circle3:
                    return _circle3Renderer;
                default:
                    return null;
            }
        }

        private void SetRadialFill(Material mat, float fill)
        {
            mat.SetFloat("_Arc2", 360 - 360 * fill);
        }
    }
}