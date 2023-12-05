using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class ScaleWithScreen : MonoBehaviour
    {
        [SerializeField] private int _refScreenWidth;
        [SerializeField] private int _refScreenHeight;
        [SerializeField] private bool _scaleWithWidth;
        [SerializeField] private bool _scaleWithHeight;

        private Vector3 _initScale;

        private void Awake()
        {
            _initScale = transform.localScale;
        }

        private void OnEnable()
        {
            var scaleX = 1f;
            if (_scaleWithWidth && _refScreenWidth > 0)
            {
                scaleX = _initScale.x * (float)Screen.width / _refScreenWidth;
            }

            if (_scaleWithHeight)
            {
                scaleX = _initScale.x * (float)Screen.height / _refScreenHeight;
            }

            Logger.LogError($"Screen.width: {Screen.width} | Screen.height: {Screen.height} | _refScreenWidth: {_refScreenWidth} | _refScreenHeight: {_refScreenHeight} | _initScale: {_initScale} | afterScale: {new Vector2(scaleX, _initScale.x)}");

            transform.localScale = new Vector3(scaleX, _initScale.y, transform.localScale.z);
        }
    }
}