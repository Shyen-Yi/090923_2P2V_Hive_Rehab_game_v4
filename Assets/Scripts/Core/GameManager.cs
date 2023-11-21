using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class GameManager : MonoBehaviour
    {
        private GameSceneManager _sceneManager;
        private CameraManager _cameraManager;
        private MonoBehaviourUtil _monoBehaviourUtil;

        private bool _isGameInitialized;

        private void Awake()
        {
            _isGameInitialized = false;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _sceneManager = new GameSceneManager();
            _sceneManager.OnInit();

            _cameraManager = new CameraManager();
            _cameraManager.OnInit();

            _monoBehaviourUtil = FindObjectOfType<MonoBehaviourUtil>();
            if (_monoBehaviourUtil == null)
            {
                LogHelper.LogError($"No MonoBehaviourUtil!");
            }

            if (!_isGameInitialized)
            {
                GameSceneManager.Instance.LoadScene(SceneNames.MainMenu, null, () =>
                {
                    _isGameInitialized = true;
                });
            }
        }

        private void OnDisable()
        {
            _sceneManager.OnDispose();
            _sceneManager = null;

            _cameraManager.OnDispose();
            _cameraManager = null;

            _monoBehaviourUtil = null;
        }
    }
}