using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class GameSceneLoadingData
    /// @brief Holds data related to a scene being loaded, including the scene name, associated data, and a completion callback.
    ///
    /// This class encapsulates the information required for loading a scene, such as the scene name, any associated data
    /// (through the `ISceneData` interface), and an optional callback to be invoked once the scene has finished loading.
    public class GameSceneLoadingData
    {
        /// <summary>
        /// The name of the scene being loaded.
        /// </summary>
        public string sceneName;

        /// <summary>
        /// The data associated with the scene, implementing the `ISceneData` interface.
        /// </summary>
        public ISceneData sceneData;

        /// <summary>
        /// A callback that will be called once the scene has finished loading.
        /// </summary>
        public Action onComplete;

        /// <summary>
        /// The loading progress of the scene (a value between 0 and 1).
        /// </summary>
        public float progress;

        /// <summary>
        /// Initializes a new instance of the `GameSceneLoadingData` class.
        /// </summary>
        /// <param name="sceneName">The name of the scene being loaded.</param>
        /// <param name="sceneData">The data associated with the scene.</param>
        /// <param name="onComplete">The callback to be invoked once the scene is loaded.</param>
        public GameSceneLoadingData(string sceneName, ISceneData sceneData, Action onComplete)
        {
            this.sceneName = sceneName;
            this.sceneData = sceneData;
            this.onComplete = onComplete;
        }
    }

    /// @ingroup Core
    /// @class GameSceneData
    /// @brief Holds data related to a loaded scene, including its index, associated data, and the scene controller.
    ///
    /// This class contains data about a scene that has already been loaded, including the scene's index, the associated
    /// scene data (through the `ISceneData` interface), and the controller for the scene. It also provides a method to 
    /// dispose of the scene's resources when no longer needed.
    public class GameSceneData : IDisposable
    {
        /// <summary>
        /// The index of the scene in the scene stack.
        /// </summary>
        public int index;

        /// <summary>
        /// The name of the loaded scene.
        /// </summary>
        public string sceneName;

        /// <summary>
        /// The data associated with the scene, implementing the `ISceneData` interface.
        /// </summary>
        public ISceneData sceneData;

        /// <summary>
        /// The controller for the loaded scene, responsible for managing the scene's behavior and UI.
        /// </summary>
        public GameSceneControllerBase controller;

        /// <summary>
        /// Initializes a new instance of the `GameSceneData` class.
        /// </summary>
        /// <param name="index">The index of the scene in the scene stack.</param>
        /// <param name="sceneName">The name of the loaded scene.</param>
        /// <param name="sceneData">The data associated with the scene.</param>
        /// <param name="controller">The controller for the loaded scene.</param>
        public GameSceneData(int index, string sceneName, ISceneData sceneData, GameSceneControllerBase controller)
        {
            this.index = index;
            this.sceneName = sceneName;
            this.sceneData = sceneData;
            this.controller = controller;
        }

        /// <summary>
        /// Disposes of the resources used by the scene, including hiding and disposing of its controller.
        /// </summary>
        public void Dispose()
        {
            if (controller != null)
            {
                controller.Hide(GameSceneHideState.Removed);
                controller.Dispose();
            }
        }
    }

    /// @ingroup Core
    /// @class GameSceneManager
    /// @brief Manages scene loading, unloading, and transitions between game scenes.
    /// 
    /// The GameSceneManager class controls the loading and unloading of scenes in the game, handling scene
    /// transitions, tracking the scene stack, and ensuring the proper initialization and disposal of scenes
    /// and their associated controllers. It provides functionality for managing multiple scenes and their visibility.
    public class GameSceneManager : SingletonBase<GameSceneManager>, ICoreManager
    {
        private Queue<GameSceneLoadingData> _scenesToLoad;
        private HashSet<string> _scenesLoading;
        private Dictionary<string, GameSceneData> _sceneDataDict;

        private static int LoadedSceneId = 0;

        #region Lifecycle
        /// <summary>
        /// Initializes the GameSceneManager instance, setting up necessary data structures.
        /// </summary>
        public void OnInit()
        {
            _scenesToLoad = new Queue<GameSceneLoadingData>();
            _scenesLoading = new HashSet<string>();
            _sceneDataDict = new Dictionary<string, GameSceneData>();

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        /// <summary>
        /// Disposes of the GameSceneManager instance, cleaning up data structures and event subscriptions.
        /// </summary>
        public void OnDispose()
        {
            _scenesToLoad.Clear();
            _scenesLoading.Clear();
            _sceneDataDict.Clear();

            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        /// <summary>
        /// Handles input events and manages the back button behavior on Android.
        /// </summary>
        private void Tick()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (InputManager.GetKeyDown(KeyCode.Escape))
            {
                if (!IsLoading())
                {
                    if (_sceneDataDict.Count > 1)
                    {
                        GoBack();
                    }
                    else
                    {
                        ShowScene(SceneNames.Confirm, new ConfirmData("Confirm", "Are you sure you want to quit the game?", "Yes", ConfirmQuit, null));
                    }
                }
            }

            void ConfirmQuit()
            {
                Application.Quit();
            }
#endif
        }
        #endregion

        #region Scene Management
        /// <summary>
        /// Gets the scene data for the scene at the top of the scene stack.
        /// </summary>
        /// <returns>The scene data for the topmost scene in the stack.</returns>
        private GameSceneData GetSceneOnTop()
        {
            var topSceneIndex = int.MinValue;
            GameSceneData topSceneData = null;

            foreach (var pair in _sceneDataDict)
            {
                if (pair.Value.index > topSceneIndex)
                {
                    topSceneIndex = pair.Value.index;
                    topSceneData = pair.Value;
                }
            }

            return topSceneData;
        }

        /// <summary>
        /// Tries to hide the scene at the top of the stack.
        /// </summary>
        /// <param name="showLastScene">Whether to show the last scene after hiding the top scene.</param>
        /// <returns>True if a scene was hidden, false otherwise.</returns>
        private bool TryHideSceneOnTop(bool showLastScene)
        {
            var sceneOnTop = GetSceneOnTop();
            if (sceneOnTop != null)
            {
                var latestScene = SceneManager.GetSceneByName(sceneOnTop.sceneName);
                if (latestScene.IsValid() && _sceneDataDict.Count > 1)
                {
                    UnloadScene(latestScene.name, showLastScene);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if any scenes are currently being loaded.
        /// </summary>
        /// <returns>True if there are scenes loading, otherwise false.</returns>
        public bool IsLoading()
        {
            return _scenesLoading.Count > 0;
        }

        /// <summary>
        /// Displays a new scene and passes data to it.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="data">The data to pass to the scene's controller.</param>
        /// <param name="onComplete">Callback to be executed once the scene has loaded.</param>
        public void ShowScene(string sceneName, ISceneData data = null, Action onComplete = null)
        {
            LoadScene(sceneName, data, onComplete);
        }

        /// <summary>
        /// Hides a scene and unloads it.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <param name="onComplete">Callback to be executed once the scene has been unloaded.</param>
        public void HideScene(string sceneName, Action onComplete = null)
        {
            UnloadScene(sceneName, true, onComplete);
        }

        /// <summary>
        /// Retrieves the controller for a loaded scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The controller for the specified scene, or null if not found.</returns>
        public GameSceneControllerBase GetLoadedSceneController(string sceneName)
        {
            if (_sceneDataDict.TryGetValue(sceneName, out var sceneData))
            {
                return sceneData.controller;
            }

            return null;
        }
        #endregion

        #region Scene Loading/Unloading
        /// <summary>
        /// Loads a new scene and enqueues it for loading.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="data">The data to pass to the scene controller.</param>
        /// <param name="onComplete">Callback to be executed when the scene has loaded.</param>
        private void LoadScene(string sceneName, ISceneData data = null, Action onComplete = null)
        {
            var loadingData = new GameSceneLoadingData(sceneName, data, onComplete);
            _scenesToLoad.Enqueue(loadingData);
            TryLoadNextScene();
        }

        /// <summary>
        /// Tries to load the next scene in the loading queue.
        /// </summary>
        private void TryLoadNextScene()
        {
            if (_scenesToLoad.Count > 0)
            {
                var nextSceneLoadingData = _scenesToLoad.Dequeue();
                var currentScene = SceneManager.GetSceneByName(nextSceneLoadingData.sceneName);
                if (!currentScene.IsValid() || currentScene.name != nextSceneLoadingData.sceneName)
                {
                    MonoBehaviourUtil.Instance.StartCoroutine(LoadSceneRoutine(nextSceneLoadingData));
                }
                else
                {
                    Logger.LogError($"{currentScene.name} is already loaded and still valid!");
                }
            }
        }

        /// <summary>
        /// Coroutine to load a scene asynchronously.
        /// </summary>
        /// <param name="data">The loading data for the scene.</param>
        /// <returns>A coroutine to load the scene.</returns>
        private IEnumerator LoadSceneRoutine(GameSceneLoadingData data)
        {
            _scenesLoading.Add(data.sceneName);

            var loadOp = SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone && data != null)
            {
                data.progress = loadOp.progress / .9f;
                yield return null;
            }

            if (data != null)
            {
                OnSceneLoaded(data);
                data.onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Called when a scene has been successfully loaded.
        /// Initializes and shows the loaded scene.
        /// </summary>
        /// <param name="data">The loading data for the scene.</param>
        private void OnSceneLoaded(GameSceneLoadingData data)
        {
            _scenesLoading.Remove(data.sceneName);

            var loadedScene = SceneManager.GetSceneByName(data.sceneName);

            // dispose if scene already exists
            if (_sceneDataDict.TryGetValue(data.sceneName, out var loadedSceneData))
            {
                loadedSceneData.Dispose();
                _sceneDataDict.Remove(data.sceneName);
            }

            // hide top
            var sceneOnTop = GetSceneOnTop();
            if (sceneOnTop != null)
            {
                sceneOnTop.controller.Hide(GameSceneHideState.Covered);
            }

            // generate scene controller and init
            var sceneControllerTypeName = $"com.hive.projectr.{data.sceneName}Controller";
            var sceneControllerType = TypeUtil.GetType(sceneControllerTypeName);
            if (sceneControllerType == null)
            {
                Logger.LogError($"{sceneControllerTypeName} not found");
                return;
            }

            var sceneControllerBase = Activator.CreateInstance(sceneControllerType) as GameSceneControllerBase;
            var id = ++LoadedSceneId;
            sceneControllerBase.Init(new GameSceneInitData(loadedScene, id));
            sceneControllerBase.Show(data.sceneData, GameSceneShowState.New);

            _sceneDataDict[data.sceneName] = new GameSceneData(id, data.sceneName, data.sceneData, sceneControllerBase);

            // check loading queue
            while (_scenesToLoad.Count > 0)
            {
                var nextSceneLoadingData = _scenesToLoad.Peek();
                if (!_sceneDataDict.ContainsKey(nextSceneLoadingData.sceneName))
                {
                    TryLoadNextScene();
                    break;
                }
            }
        }

        /// <summary>
        /// Unloads a scene asynchronously.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <param name="showLastScene">Whether to show the last scene after unloading the current one.</param>
        /// <param name="onComplete">Callback to be executed when the scene is unloaded.</param>
        private void UnloadScene(string sceneName, bool showLastScene = true, Action onComplete = null)
        {
            MonoBehaviourUtil.Instance.StartCoroutine(UnloadSceneRoutine(sceneName, showLastScene, onComplete));
        }

        /// <summary>
        /// Coroutine to unload a scene asynchronously.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <param name="showLastScene">Whether to show the last scene after unloading the current one.</param>
        /// <param name="onComplete">Callback to be executed when the scene is unloaded.</param>
        /// <returns>A coroutine to unload the scene.</returns>
        private IEnumerator UnloadSceneRoutine(string sceneName, bool showLastScene, Action onComplete)
        {
            if (_sceneDataDict.TryGetValue(sceneName, out var loadedSceneData))
            {
                loadedSceneData.Dispose();
                _sceneDataDict.Remove(sceneName);
            }

            if (showLastScene)
            {
                var sceneOnTop = GetSceneOnTop();
                if (sceneOnTop != null)
                {
                    sceneOnTop.controller.Show(sceneOnTop.sceneData, GameSceneShowState.Uncovered);
                }
            }

            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
            {
                yield return null;
            }

            OnSceneUnloaded(sceneName);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Called when a scene has been unloaded.
        /// </summary>
        /// <param name="sceneName">The name of the scene that was unloaded.</param>
        private void OnSceneUnloaded(string sceneName)
        {

        }

        /// <summary>
        /// Goes back to the previous scene in stack by unloading the topmost scene and showing the one uncovered.
        /// </summary>
        /// <param name="onComplete">A callback to be executed when scene loading/unloading is completed.</param>
        public void GoBack(Action onComplete = null)
        {
            GoBack(null, onComplete);
        }

        /// <summary>
        /// Goes back to a specified scene which is previously loaded in stack by unloading the topmost scene and showing the one uncovered.
        /// </summary>
        /// <param name="sceneName">Name of the scene to be on top.</param>
        /// <param name="onComplete">A callback to be executed when scene loading/unloading is completed.</param>
        public void GoBack(string sceneName, Action onComplete = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                var sceneOnTop = GetSceneOnTop();
                if (sceneOnTop != null)
                {
                    UnloadScene(sceneOnTop.sceneName, true, onComplete);
                }
                else
                {
                    onComplete?.Invoke();
                }
            }
            else
            {
                var allSceneData = new List<GameSceneData>(_sceneDataDict.Values);
                allSceneData.Sort(CmpSceneData);

                for (var i = allSceneData.Count - 1; i >= 0; --i)
                {
                    var sceneData = allSceneData[i];
                    if (allSceneData[i].sceneName.Equals(sceneName))
                    {
                        sceneData.controller.Show(sceneData.sceneData, GameSceneShowState.Uncovered);

                        var unloadCounter = allSceneData.Count - i;
                        for (var j = allSceneData.Count - 1; j > i; --j)
                        {
                            var data = allSceneData[j];
                            UnloadScene(data.sceneName, false);
                        }

                        break;
                    }
                }
            }
        }

        private int CmpSceneData(GameSceneData a, GameSceneData b)
        {
            if (a == null || b == null)
                return 0;

            return a.index - b.index;
        }

        #endregion
    }
}