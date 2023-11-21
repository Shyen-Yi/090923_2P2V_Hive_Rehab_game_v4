using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.hive.projectr
{
    public class HiveButton : Button
    {
        public static readonly float PressedSize = .95f;
        public static readonly Vector3 DefaultScale = Vector3.zero;

        private Vector3 _initialScale = DefaultScale;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            image.sprite = spriteState.highlightedSprite;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            image.sprite = spriteState.disabledSprite;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            _initialScale = transform.localScale;
            transform.localScale = Vector3.one * PressedSize;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            transform.localScale = _initialScale;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Hive Button")]
        static void AddHiveButton()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                LogHelper.LogError($"No Canvas can be found!");
                return;
            }

            var buttonObj = new GameObject("HiveButton");
            buttonObj.transform.SetParent(canvas.transform, false);

            var button = buttonObj.AddComponent<HiveButton>();
            button.transform.localPosition = Vector3.zero;
            button.transform.localRotation = Quaternion.identity;
            button.transform.localScale = Vector3.one;
        }
#endif
    }
}