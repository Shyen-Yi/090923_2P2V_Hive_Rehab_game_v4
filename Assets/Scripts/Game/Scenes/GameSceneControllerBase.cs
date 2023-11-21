using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.hive.projectr
{
    public interface ISceneData
    {
    }

    public struct GameSceneInitData
    {
        public Scene scene;
        public int index;

        public GameSceneInitData(Scene scene, int index)
        {
            this.scene = scene;
            this.index = index;
        }
    }

    public abstract class GameSceneControllerBase
    {
        public string Name => Scene.name;
        protected Scene Scene { get; private set; }
        protected int Index { get; private set; }
        protected GeneralSceneConfig Config { get; private set; }

        public void Init(GameSceneInitData initData)
        {
            Scene = initData.scene;
            Index = initData.index;

            // configure ui camera settings
            var cams = new List<Camera>();
            var canvases = new List<Canvas>();
            var rootGOs = Scene.GetRootGameObjects();
            for (var i = 0; i < rootGOs.Length; ++i)
            {
                var rootGO = rootGOs[i];
                var subCams = rootGO.GetComponentsInChildren<Camera>();
                var subCanvases = rootGO.GetComponentsInChildren<Canvas>();
                cams.AddRange(subCams);
                canvases.AddRange(subCanvases);

                if (Config == null)
                {
                    var sceneConfig = rootGO.GetComponentInChildren<GeneralSceneConfig>();
                    if (sceneConfig != null)
                    {
                        Config = sceneConfig;
                    }
                }
            }

            foreach (var cam in cams)
            {
                CameraManager.Instance.AddToMainStackWithOwner(Scene.name, cam);
            }
            foreach (var canvas in canvases)
            {
                canvas.worldCamera = CameraManager.Instance.UICamera;
                canvas.sortingOrder += Index;
            }

            OnInit();
        }

        public void Show(ISceneData data)
        {
            OnShow(data);
        }

        public void Hide()
        {
            OnHide();
        }

        public void Dispose()
        {
            OnDispose();

            CameraManager.Instance.RemoveFromMainStackWithOwner(Scene.name);
        }

        #region Override
        protected abstract void OnShow(ISceneData data);
        protected abstract void OnHide();
        protected abstract void OnInit();
        protected abstract void OnDispose();
        #endregion
    }
}