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
            var scaleX = _initScale.x;
            if (_scaleWithWidth && _refScreenWidth > 0)
            {
                scaleX *= (float)Screen.width / _refScreenWidth;
            }

            var scaleY = _initScale.y;
            if (_scaleWithHeight)
            {
                scaleY *= (float)Screen.height / _refScreenHeight;
            }

            transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);
        }
    }
}