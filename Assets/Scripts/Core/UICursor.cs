using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.hive.projectr
{
    public class UICursor : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = true;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = (RectTransform)transform;
        }

        /// <summary>
        /// For the cursor on Canvas - Overlay, all positions are equal.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            return _rt.position;
        }

        public void SetPosition(Vector2 position)
        {
            _rt.position = new Vector2(position.x, position.y);
        }

        public void Enable()
        {
            IsEnabled = true;
            _rt.gameObject.SetActive(true);
        }

        public void Disable()
        {
            IsEnabled = false;
            _rt.gameObject.SetActive(false);
        }
    }
}