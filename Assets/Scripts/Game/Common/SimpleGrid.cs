using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace com.hive.projectr
{
    public class SimpleGrid : MonoBehaviour, IDisposable
    {
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private GameObject _elementPrefab;
        [SerializeField] private Transform _elementRoot;

        private ISimpleGridHandler _handler;
        private List<SimpleGridElement> _elements = new List<SimpleGridElement>();

        private void Awake()
        {
            if (_gridLayoutGroup == null)
            {
                _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
            }

            if (_elementRoot == null)
            {
                _elementRoot = _gridLayoutGroup?.transform;
            }
        }

        public void Init(ISimpleGridHandler handler)
        {
            _handler = handler;
        }

        public void Dispose()
        {
            Clear();
            _handler = null;
        }

        public void Refresh()
        {
            if (_handler == null)
            {
                Debug.LogError($"SimpleGrid has not valid handler!");
                return;
            }

            if (_elementPrefab == null)
            {
                Debug.LogError($"_elementPrefab is null");
                return;
            }

            if (_elementPrefab.GetComponent<GeneralWidgetConfig>() == null)
            {
                Debug.LogError($"_elementPrefab needs a GeneralWidgetConfig!");
                return;
            }

            var newCount = _handler.GetDataCount();
            var prevCount = _elements.Count;

            if (prevCount < newCount)
            {
                for (var i = 0; i < prevCount; ++i)
                {
                    var element = _elements[i];
                    var index = i;
                    element.Index = index;
                    _handler.OnElementShow(element);
                }

                for (var i = 0; i < newCount - prevCount; ++i)
                {
                    var elementObj = GameObject.Instantiate(_elementPrefab, _elementRoot);
                    var elementConfig = elementObj.GetComponent<GeneralWidgetConfig>();
                    var index = i + prevCount;
                    var element = (SimpleGridElement)elementConfig;
                    element.Index = index;
                    _elements.Add(element);
                    _handler.OnElementCreate(element);
                    _handler.OnElementShow(element);
                }
            }
            else if (prevCount > newCount)
            {
                for (var i = prevCount - 1; i >= newCount; --i)
                {
                    var element = _elements[i];
                    _handler.OnElementHide(element);
                    _handler.OnElementDestroy(element);
                    GameObject.Destroy(element.gameObject);

                    _elements.RemoveAt(i);
                }

                for (var i = 0; i < newCount; ++i)
                {
                    var element = _elements[i];
                    var index = i;
                    element.Index = index;
                    _handler.OnElementShow(element);
                }
            }
            else
            {
                for (var i = 0; i < newCount; ++i)
                {
                    var element = _elements[i];
                    var index = i;
                    element.Index = index;
                    _handler.OnElementShow(element);
                }
            }
        }

        public void RefreshElement(int index)
        {
            if (_handler == null)
            {
                Debug.LogError($"SimpleGrid has not valid handler!");
                return;
            }

            if (index >= 0 && index < _elements.Count)
            {
                var element = _elements[index];
                element.Index = index;
                _handler.OnElementShow(element);
            }
        }

        public void Clear()
        {
            for (var i = _elements.Count - 1; i >= 0; --i)
            {
                var element = _elements[i];
                _handler.OnElementHide(element);
                _handler.OnElementDestroy(element);
                GameObject.Destroy(element.gameObject);
            }

            _elements.Clear();
        }
    }

    public class SimpleGridElement : MonoBehaviour
    {
        public int Index { get; set; }
    }

    public interface ISimpleGridHandler
    {
        public int GetDataCount();
        public void OnElementShow(SimpleGridElement element);
        public void OnElementHide(SimpleGridElement element);
        public void OnElementCreate(SimpleGridElement element);
        public void OnElementDestroy(SimpleGridElement element);
    }
}