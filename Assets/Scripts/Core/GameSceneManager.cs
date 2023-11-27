using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace com.hive.projectr
{
    public class GameSceneLoadingData
    {
        public string sceneName;
        public ISceneData sceneData;
        public Action onComplete;
        public float progress;

        public GameSceneLoadingData(string sceneName, ISceneData sceneData, Action onComplete)
        {
            this.sceneName = sceneName;
            this.sceneData = sceneData;
            this.onComplete = onComplete;
        }
    }

    public class GameSceneData : IDisposable
    {
        public int index;
        public string sceneName;
        public ISceneData sceneData;
        public GameSceneControllerBase controller;

        public GameSceneData(int index, string sceneName, ISceneData sceneData, GameSceneControllerBase controller)
        {
            this.index = index;
            this.sceneName = sceneName;
            this.sceneData = sceneData;
            this.controller = controller;
        }

        public void Dispose()
        {
            if (controller != null)
            {
                controller.Hide(GameSceneHideState.Remove);
                controller.Dispose();
            }
        }
    }

    public class GameSceneManager : SingletonBase<GameSceneManager>, ICoreManager
    {
        private Queue<GameSceneLoadingData> _scenesToLoad;
        private HashSet<string> _scenesLoading;
        private Dictionary<string, GameSceneData> _sceneDataDict;

        #region Lifecycle
        public void OnInit()
        {
            _scenesToLoad = new Queue<GameSceneLoadingData>();
            _scenesLoading = new HashSet<string>();
            _sceneDataDict = new Dictionary<string, GameSceneData>();

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void OnDispose()
        {
            _scenesToLoad.Clear();
            _scenesLoading.Clear();
            _sceneDataDict.Clear();

            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        private void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                TryUnloadSceneOnTop();
            }
        }
        #endregion

        private GameSceneData GetSceneOnTop()
        {
            var topSceneIndex = int.MinValue;
            GameSceneData topSceneData = null;

            foreach (var pair in _sceneDataDict)
            {
                if (pair.Value.index > topSceneIndex)
                {
                    topSceneData = pair.Value;
                }
            }

            return topSceneData;
        }

        private bool TryUnloadSceneOnTop()
        {
            var sceneOnTop = GetSceneOnTop();
            if (sceneOnTop != null)
            {
                var latestScene = SceneManager.GetSceneByName(sceneOnTop.sceneName);
                if (latestScene.IsValid() && _sceneDataDict.Count > 1)
                {
                    UnloadScene(latestScene.name);
                }
            }

            return false;
        }

        public bool IsLoading()
        {
            return _scenesLoading.Count > 0;
        }

        public void LoadScene(string sceneName, ISceneData data = null, Action onComplete = null)
        {
            var loadingData = new GameSceneLoadingData(sceneName, data, onComplete);
            _scenesToLoad.Enqueue(loadingData);
            TryLoadNextScene();
        }

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
            }
        }

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
            var index = _sceneDataDict.Count;
            sceneControllerBase.Init(new GameSceneInitData(loadedScene, index));
            sceneControllerBase.Show(data.sceneData, GameSceneShowState.New);
            _sceneDataDict[data.sceneName] = new GameSceneData(index, data.sceneName, data.sceneData, sceneControllerBase);

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

        public void UnloadScene(string sceneName, Action onComplete = null)
        {
            MonoBehaviourUtil.Instance.StartCoroutine(UnloadSceneRoutine(sceneName, onComplete));
        }

        private IEnumerator UnloadSceneRoutine(string sceneName, Action onComplete)
        {
            if (_sceneDataDict.TryGetValue(sceneName, out var loadedSceneData))
            {
                loadedSceneData.Dispose();
                _sceneDataDict.Remove(sceneName);
            }

            var sceneOnTop = GetSceneOnTop();
            if (sceneOnTop != null)
            {
                sceneOnTop.controller.Show(sceneOnTop.sceneData, GameSceneShowState.Uncovered);
            }

            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
            {
                yield return null;
            }

            OnSceneUnloaded(sceneName);
            onComplete?.Invoke();
        }

        private void OnSceneUnloaded(string sceneName)
        {

        }
    }
}