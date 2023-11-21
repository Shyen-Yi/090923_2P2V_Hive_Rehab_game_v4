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
        public string name;
        public ISceneData data;
        public GameSceneControllerBase controller;

        public GameSceneData(int index, string name, ISceneData data, GameSceneControllerBase controller)
        {
            this.index = index;
            this.name = name;
            this.data = data;
            this.controller = controller;
        }

        public void Dispose()
        {
            if (controller != null)
            {
                controller.Hide();
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
            Instance = this;

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
                TryUnloadLatestScene();
            }
        }
        #endregion

        private void TryUnloadLatestScene()
        {
            var latestSceneIndex = 0;
            var latestSceneName = string.Empty;
            foreach (var pair in _sceneDataDict)
            {
                if (pair.Value.index > latestSceneIndex)
                {
                    latestSceneName = pair.Key;
                }
            }
            if (!string.IsNullOrEmpty(latestSceneName))
            {
                var latestScene = SceneManager.GetSceneByName(latestSceneName);
                if (latestScene.IsValid() && _sceneDataDict.Count > 1)
                {
                    UnloadScene(latestScene.name);
                }
            }
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

            // dispose prev if existed
            if (_sceneDataDict.TryGetValue(data.sceneName, out var loadedSceneData))
            {
                loadedSceneData.Dispose();
                _sceneDataDict.Remove(data.sceneName);
            }

            // generate scene controller and init
            var sceneControllerBase = Activator.CreateInstance(Type.GetType($"com.hive.projectr.{data.sceneName}Controller")) as GameSceneControllerBase;
            var index = _sceneDataDict.Count;
            sceneControllerBase.Init(new GameSceneInitData(loadedScene, index));
            sceneControllerBase.Show(data.sceneData);
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
            if (_sceneDataDict.TryGetValue(sceneName, out var loadedSceneData))
            {
                loadedSceneData.Dispose();
                _sceneDataDict.Remove(sceneName);
            }
        }
    }
}