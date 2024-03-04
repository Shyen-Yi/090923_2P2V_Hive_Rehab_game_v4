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

    public enum CalibrationMarkerType
    {
        Center = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        BottomLeft = 4,
    }

    public enum CalibrationMarkerLabelType
    {
        None = 0,
        Hold = 1,
        Redo = 2,
        Success = 3,
    }

    public class CalibrationMarkerController
    {
        private GeneralWidgetConfig _config;

        #region Extra
        private enum ExtraRenderer
        {
            Center = 0,
            Circle1 = 1,
            Circle2 = 2,
            Circle3 = 3,
        }

        private enum ExtraObj
        {
            Hold = 0,
            Redo = 1,
            Success = 2,
        }

        private enum ExtraSprite
        {
            Center = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4,
        }

        private SpriteRenderer _centerRenderer;
        private SpriteRenderer _circle1Renderer;
        private SpriteRenderer _circle2Renderer;
        private SpriteRenderer _circle3Renderer;

        private Dictionary<CalibrationMarkerLabelType, Transform> _labelDict;

        private Dictionary<CalibrationMarkerType, Sprite> _spriteDict;
        #endregion

        public CalibrationMarkerController(GeneralWidgetConfig config)
        {
            _config = config;

            InitExtra();
        }

        private void InitExtra()
        {
            _centerRenderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Center];
            _circle1Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle1];
            _circle2Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle2];
            _circle3Renderer = (SpriteRenderer)_config.ExtraRenderers[(int)ExtraRenderer.Circle3];

            var hold = _config.ExtraObjects[(int)ExtraObj.Hold];
            var redo = _config.ExtraObjects[(int)ExtraObj.Redo];
            var _success = _config.ExtraObjects[(int)ExtraObj.Success];
            _labelDict = new Dictionary<CalibrationMarkerLabelType, Transform>() {
                { CalibrationMarkerLabelType.Hold, hold },
                { CalibrationMarkerLabelType.Redo, redo },
                { CalibrationMarkerLabelType.Success, _success }
            };

            var center = _config.ExtraSprites[(int)ExtraSprite.Center];
            var topLeft = _config.ExtraSprites[(int)ExtraSprite.TopLeft];
            var topRight = _config.ExtraSprites[(int)ExtraSprite.TopRight];
            var bottomRight = _config.ExtraSprites[(int)ExtraSprite.BottomRight];
            var bottomLeft = _config.ExtraSprites[(int)ExtraSprite.BottomLeft];
            _spriteDict = new Dictionary<CalibrationMarkerType, Sprite>()
            {
                { CalibrationMarkerType.Center, center },
                { CalibrationMarkerType.TopLeft, topLeft },
                { CalibrationMarkerType.TopRight, topRight },
                { CalibrationMarkerType.BottomRight, bottomRight },
                { CalibrationMarkerType.BottomLeft, bottomLeft },
            };
        }

        public void Activate(CalibrationMarkerType markerType)
        {
            if (_spriteDict.TryGetValue(markerType, out var sprite))
            {
                _centerRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Undefined marker type: {markerType}");
            }

            SetAlpha(1);

            _config.gameObject.SetActive(true);
        }

        public void SetWorldPosition(Vector3 worldPos)
        {
            _config.transform.position = new Vector3(worldPos.x, worldPos.y, _config.transform.position.z);
        }

        public void OnHoldingStart()
        {
            SetAlpha(1);
            ShowLabel(CalibrationMarkerLabelType.Hold);
        }

        public void OnInterrupted()
        {
            ResetAllCircles();
            SetAlpha(CalibrationConfig.GetData().PendingMarkerAlpha);
            ShowLabel(CalibrationMarkerLabelType.Redo);
        }

        public void OnSuccess()
        {
            ResetAllCircles();
            SetAlpha(0);
            ShowLabel(CalibrationMarkerLabelType.Success);
        }

        public void Deactivate()
        {
            ResetAllCircles();
            ShowLabel(CalibrationMarkerLabelType.None);
            _config.gameObject.SetActive(false);
        }

        public void SetCircleFill(int circle, float fill)
        {
            if (Enum.IsDefined(typeof(CalibrationMarkerCircle), circle))
            {
                var pCircle = (CalibrationMarkerCircle)circle;
                fill = Mathf.Clamp01(fill);
                ShaderUtil.SetRadialFill(GetRenderer(pCircle).material, fill);
            }
        }

        private void ResetAllCircles()
        {
            ShaderUtil.SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle1).material, 0);
            ShaderUtil.SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle2).material, 0);
            ShaderUtil.SetRadialFill(GetRenderer(CalibrationMarkerCircle.Circle3).material, 0);
        }

        private void SetAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            _centerRenderer.color = new Color(1, 1, 1, alpha);
            _circle1Renderer.color = new Color(1, 1, 1, alpha);
            _circle2Renderer.color = new Color(1, 1, 1, alpha);
            _circle3Renderer.color = new Color(1, 1, 1, alpha);
        }

        private void ShowLabel(CalibrationMarkerLabelType labelType)
        {
            foreach (var pair in _labelDict)
            {
                pair.Value.gameObject.SetActive(pair.Key == labelType);
            }
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
    }
}