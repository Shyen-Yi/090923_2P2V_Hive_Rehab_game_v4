using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public enum AnchorType
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public class AnchorToScreen : MonoBehaviour
    {
        [SerializeField] private AnchorType _anchorType;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private int _refScreenWidth = 1920; // Default reference screen width
        [SerializeField] private int _refScreenHeight = 1080; // Default reference screen height

        private void OnEnable()
        {
            Vector2 screenPosition = CalculateScreenPosition();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }

        private Vector2 CalculateScreenPosition()
        {
            float x = 0, y = 0;

            switch (_anchorType)
            {
                case AnchorType.TopLeft:
                    x = _offset.x;
                    y = Screen.height - _offset.y;
                    break;
                case AnchorType.Top:
                    x = Screen.width / 2 + _offset.x;
                    y = Screen.height - _offset.y;
                    break;
                case AnchorType.TopRight:
                    x = Screen.width - _offset.x;
                    y = Screen.height - _offset.y;
                    break;
                case AnchorType.Left:
                    x = _offset.x;
                    y = Screen.height / 2 + _offset.y;
                    break;
                case AnchorType.Center:
                    x = Screen.width / 2 + _offset.x;
                    y = Screen.height / 2 + _offset.y;
                    break;
                case AnchorType.Right:
                    x = Screen.width - _offset.x;
                    y = Screen.height / 2 + _offset.y;
                    break;
                case AnchorType.BottomLeft:
                    x = _offset.x;
                    y = _offset.y;
                    break;
                case AnchorType.Bottom:
                    x = Screen.width / 2 + _offset.x;
                    y = _offset.y;
                    break;
                case AnchorType.BottomRight:
                    x = Screen.width - _offset.x;
                    y = _offset.y;
                    break;
            }

            return new Vector2(x, y);
        }
    }

}