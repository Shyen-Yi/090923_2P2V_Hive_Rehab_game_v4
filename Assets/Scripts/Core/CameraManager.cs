using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class CameraManager
    /// @brief Manages the game's camera system, including camera positioning and movement.
    /// 
    /// The CameraManager class controls the camera in the game, handling its position, field of view,
    /// and any transformations needed for game scenes. It provides functionality for adding and removing
    /// cameras to the main camera's stack, and for managing cameras that are used for UI or game scenes.
    public class CameraManager : SingletonBase<CameraManager>, ICoreManager
    {
        /// <summary>
        /// The main camera used in the game.
        /// It retrieves the camera with the "MainCamera" tag (from TagNames) if it hasn't been set yet.
        /// </summary>
        public Camera MainCamera 
        {
            get
            {
                if (_mainCam == null)
                {
                    _mainCam = GameObject.FindGameObjectWithTag(TagNames.MainCamera).GetComponent<Camera>();
                }
                return _mainCam;
            }
        }

        /// <summary>
        /// The camera used to capture UI.
        /// It retrieves the camera with the "UICamera" tag (from TagNames) if it hasn't been set yet.
        /// </summary>
        public Camera UICamera
        {
            get
            {
                if (_uiCam == null)
                {
                    _uiCam = GameObject.FindGameObjectWithTag(TagNames.UICamera).GetComponent<Camera>();
                }
                return _uiCam;
            }
        }

        private Camera _mainCam;
        private Camera _uiCam;

        private Dictionary<object, List<Camera>> _camOwnerDict = new Dictionary<object, List<Camera>>();

        private static readonly int HomelessCamId = 0;

        /// <summary>
        /// Initializes the CameraManager instance. 
        /// This method is called when the CameraManager is initialized.
        /// </summary>
        public void OnInit()
        {
        }

        /// <summary>
        /// Disposes of the CameraManager instance.
        /// This method is called when the CameraManager is disposed.
        /// </summary>
        public void OnDispose()
        {
        }

        /// <summary>
        /// Adds a camera (whose owner is unknown) to the main camera's stack.
        /// </summary>
        /// <param name="cam">The camera to add to the main camera's stack.</param>
        public void AddToMainStack(Camera cam)
        {
            if (cam == null)
            {
                return;
            }

            _camOwnerDict[HomelessCamId] = new List<Camera>() { cam };
            var universalCamData = MainCamera.GetUniversalAdditionalCameraData();
            universalCamData.cameraStack.Add(cam);
        }

        /// <summary>
        /// Removes a camera (whose owner is unknown) from the main camera's stack.
        /// </summary>
        /// <param name="cam">The camera to remove from the main camera's stack.</param>
        public void RemoveFromMainStack(Camera cam)
        {
            if (cam == null)
            {
                return;
            }

            if (_camOwnerDict.TryGetValue(HomelessCamId, out var list))
            {
                list.Remove(cam);
            }
            var universalCamData = MainCamera.GetUniversalAdditionalCameraData();
            universalCamData.cameraStack.Remove(cam);
        }

        /// <summary>
        /// Adds a camera (with a specific owner) to the main camera's stack.
        /// </summary>
        /// <param name="owner">A unique reference to the owner.</param>
        /// <param name="cam">The camera to add to the main camera's stack.</param>
        public void AddToMainStackWithOwner(object owner, Camera cam)
        {
            if (owner == null)
            {
                if (cam == null)
                {
                    return;
                }

                Logger.LogError($"Homeless camera! {cam.name}");
                _camOwnerDict[HomelessCamId] = new List<Camera>() { cam };
            }
            else
            {
                if (!_camOwnerDict.TryGetValue(owner, out var list))
                {
                    list = new List<Camera>();
                    _camOwnerDict[owner] = list;
                }
                list.Add(cam);
            }

            var universalCamData = MainCamera.GetUniversalAdditionalCameraData();
            universalCamData.cameraStack.Add(cam);
        }

        /// <summary>
        /// Removes all cameras (with the same specific owner) from the main camera's stack.
        /// </summary>
        /// <param name="owner">The reference to the owner of all cameras to remove.</param>
        public void RemoveFromMainStackWithOwner(object owner)
        {
            if (owner == null)
            {
                return;
            }

            if (_camOwnerDict.TryGetValue(owner, out var list))
            {
                var universalCamData = MainCamera.GetUniversalAdditionalCameraData();

                for (var i = 0; i < list.Count; ++i)
                {
                    var cam = list[i];
                    universalCamData.cameraStack.Remove(cam);
                }
                _camOwnerDict.Remove(owner);
            }
        }
    }
}