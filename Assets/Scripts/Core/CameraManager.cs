using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace com.hive.projectr
{
    public class CameraManager : SingletonBase<CameraManager>, ICoreManager
    {
        public Camera MainCamera 
        {
            get
            {
                if (_mainCam == null)
                {
                    _mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                }
                return _mainCam;
            }
        }
        public Camera UICamera
        {
            get
            {
                if (_uiCam == null)
                {
                    _uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
                }
                return _uiCam;
            }
        }

        private Camera _mainCam;
        private Camera _uiCam;

        private Dictionary<object, List<Camera>> _camOwnerDict = new Dictionary<object, List<Camera>>();

        private static readonly int HomelessCamId = 0;

        public void OnInit()
        {
            Instance = this;
        }

        public void OnDispose()
        {
        }

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

        public void AddToMainStackWithOwner(object owner, Camera cam)
        {
            if (owner == null)
            {
                if (cam == null)
                {
                    return;
                }

                LogHelper.LogError($"Homeless camera! {cam.name}");
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