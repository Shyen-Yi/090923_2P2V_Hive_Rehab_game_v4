using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.hive.projectr
{
    public class UICursor : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = true;

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public Vector2 GetScreenPosition()
        {
            var screenPos = CameraManager.Instance.UICamera.WorldToScreenPoint(transform.position);
            return screenPos;
        }

        public void SetScaledAndCenteredPosition(Vector2 position)
        {
            if (IsEnabled)
            {
                ((RectTransform)transform).anchoredPosition = new Vector2(position.x, position.y);
            }
        }

        public void Enable()
        {
            IsEnabled = true;
            transform.gameObject.SetActive(true);
        }

        public void Disable()
        {
            IsEnabled = false;
            transform.gameObject.SetActive(false);
        }
    }
}