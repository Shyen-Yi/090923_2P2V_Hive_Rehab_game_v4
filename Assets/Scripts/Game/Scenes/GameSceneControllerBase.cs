using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.hive.projectr
{
    public interface ISceneData
    {
        // Interface for scene-specific data
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

    public enum GameSceneShowState
    {
        New,        ///< The scene is being added to the top of stack
        Uncovered,  ///< The scene is being uncovered (e.g., brought into view from behind another scene).
    }

    public enum GameSceneHideState
    {
        Removed,    ///< The scene is being removed from the stack
        Covered,    ///< The scene is being covered (e.g., hidden behind another scene).
    }

    /// @ingroup GameScene
    /// @class GameSceneControllerBase
    /// @brief Base class for managing the initialization, display, and hiding of a game scene's UI and logic.
    /// 
    /// The `GameSceneControllerBase` class provides an abstract base for controlling game scenes, including scene initialization,
    /// visibility management (show/hide), and disposal. It handles the configuration of UI elements like cameras and canvases, 
    /// ensuring that the scene's UI components are properly set up and linked to the correct scene context.
    public abstract class GameSceneControllerBase
    {
        public string SceneName => Scene.name;  ///< The name of the scene being controlled.
        protected Scene Scene { get; private set; }  ///< The scene object being controlled.
        protected int Index { get; private set; }  ///< The index of the scene in the game flow.
        protected GeneralSceneConfig Config { get; private set; }  ///< The configuration associated with the scene.

        /// <summary>
        /// Initializes the game scene with the given initialization data.
        /// </summary>
        public void Init(GameSceneInitData initData)
        {
            Scene = initData.scene;
            Index = initData.index;

            // Set up cameras and canvases for the scene
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

                // Configure the scene settings if not already set
                if (Config == null)
                {
                    var sceneConfig = rootGO.GetComponentInChildren<GeneralSceneConfig>();
                    if (sceneConfig != null)
                    {
                        Config = sceneConfig;
                    }
                }
            }

            // Add cameras to the camera stack and configure canvas settings
            foreach (var cam in cams)
            {
                CameraManager.Instance.AddToMainStackWithOwner(Scene.name, cam);
            }
            foreach (var canvas in canvases)
            {
                canvas.worldCamera = CameraManager.Instance.UICamera;
                canvas.sortingOrder += Index * 100;
            }

            Logger.Log($"GameSceneController::Init - SceneName: {SceneName}");

            OnInit();
        }

        /// <summary>
        /// Shows the scene, making its UI visible and active.
        /// </summary>
        public void Show(ISceneData data, GameSceneShowState showState)
        {
            Logger.Log($"GameSceneController::Show - SceneName: {SceneName} | ShowState: {showState}");

            OnShow(data, showState);
            Config.CanvasGroup.CanvasGroupOn();
        }

        /// <summary>
        /// Hides the scene, making its UI invisible and inactive.
        /// </summary>
        public void Hide(GameSceneHideState hideState)
        {
            Logger.Log($"GameSceneController::Hide - SceneName: {SceneName} | HideState: {hideState}");

            OnHide(hideState);
            Config.CanvasGroup.CanvasGroupOff();
        }

        /// <summary>
        /// Disposes of the scene controller, cleaning up any resources and removing it from the camera stack.
        /// </summary>
        public void Dispose()
        {
            Logger.Log($"GameSceneController::Dispose - SceneName: {SceneName}");

            OnDispose();

            CameraManager.Instance.RemoveFromMainStackWithOwner(Scene.name);
        }

        #region Virtual
        /// <summary>
        /// Virtual method for handling the display logic when the scene is shown.
        /// </summary>
        protected virtual void OnShow(ISceneData data, GameSceneShowState showState) { }

        /// <summary>
        /// Virtual method for handling the logic when the scene is hidden.
        /// </summary>
        protected virtual void OnHide(GameSceneHideState hideState) { }

        /// <summary>
        /// Virtual method for scene-specific initialization logic.
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Virtual method for scene-specific disposal logic.
        /// </summary>
        protected virtual void OnDispose() { }
        #endregion
    }
}