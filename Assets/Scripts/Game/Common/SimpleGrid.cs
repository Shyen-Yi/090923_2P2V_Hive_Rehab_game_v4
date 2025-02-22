using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class SimpleGrid
    /// @brief A component that manages a dynamic grid layout, handling the creation, destruction, and updating of grid elements.
    /// 
    /// The `SimpleGrid` class is responsible for managing a grid layout in Unity. It dynamically handles the creation and updating of grid
    /// elements based on the number of data items provided by the handler. It also manages the instantiation and destruction of the grid
    /// elements when the grid's content changes, ensuring that the UI stays in sync with the underlying data.
    public class SimpleGrid : MonoBehaviour, IDisposable
    {
        [SerializeField] private GridLayoutGroup _gridLayoutGroup; ///< The GridLayoutGroup used to organize grid elements.
        [SerializeField] private GameObject _elementPrefab; ///< The prefab used to instantiate grid elements.
        [SerializeField] private Transform _elementRoot; ///< The root object where grid elements will be instantiated.

        private ISimpleGridHandler _handler; ///< The handler that provides data and controls the behavior of grid elements.
        private List<SimpleGridElement> _elements = new List<SimpleGridElement>(); ///< The list of currently active grid elements.

        /// <summary>
        /// Initializes the SimpleGrid with the provided handler.
        /// </summary>
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

        /// <summary>
        /// Initializes the SimpleGrid with the provided handler.
        /// </summary>
        public void Init(ISimpleGridHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Disposes of the SimpleGrid by clearing all elements and setting the handler to null.
        /// </summary>
        public void Dispose()
        {
            Clear();
            _handler = null;
        }

        /// <summary>
        /// Refreshes the grid based on the handler's data. Adds, removes, or updates grid elements as needed.
        /// </summary>
        public void Refresh()
        {
            if (_handler == null)
            {
                Logger.LogError($"SimpleGrid has not valid handler!");
                return;
            }

            if (_elementPrefab == null)
            {
                Logger.LogError($"_elementPrefab is null");
                return;
            }

            if (_elementPrefab.GetComponent<GeneralWidgetConfig>() == null)
            {
                Logger.LogError($"_elementPrefab needs a GeneralWidgetConfig!");
                return;
            }

            // Handle grid element addition, removal, and updates
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

        /// <summary>
        /// Refreshes a specific grid element at the given index.
        /// </summary>
        public void RefreshElement(int index)
        {
            if (_handler == null)
            {
                Logger.LogError($"SimpleGrid has not valid handler!");
                return;
            }

            if (index >= 0 && index < _elements.Count)
            {
                var element = _elements[index];
                element.Index = index;
                _handler.OnElementShow(element);
            }
        }

        /// <summary>
        /// Clears all grid elements and removes them from the scene.
        /// </summary>
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

    /// <summary>
    /// Represents an individual element in the SimpleGrid.
    /// </summary>
    public class SimpleGridElement : MonoBehaviour
    {
        /// <summary>
        /// The index of the element in the grid.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Interface that defines the behavior for handling grid elements in the SimpleGrid.
    /// </summary>
    public interface ISimpleGridHandler
    {
        /// <summary>
        /// Gets the total count of data elements to be displayed in the grid.
        /// </summary>
        public int GetDataCount();

        /// <summary>
        /// Called when an element should be shown in the grid.
        /// </summary>
        public void OnElementShow(SimpleGridElement element);

        /// <summary>
        /// Called when an element should be hidden in the grid.
        /// </summary>
        public void OnElementHide(SimpleGridElement element);

        /// <summary>
        /// Called when a new element is created in the grid.
        /// </summary>
        public void OnElementCreate(SimpleGridElement element);

        /// <summary>
        /// Called when an element should be destroyed in the grid.
        /// </summary>
        public void OnElementDestroy(SimpleGridElement element);
    }
}