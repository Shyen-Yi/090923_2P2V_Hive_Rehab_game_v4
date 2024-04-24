using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class GameManager : MonoBehaviour
    {
        private GameSceneManager _sceneManager;
        private CameraManager _cameraManager;
        private ConfigDataManager _configManager;
        private CSVManager _csvManager;
        private InputManager _inputManager;
        private SettingManager _settingManager;
        private LevelManager _levelManager;
        private StatsManager _statsManager;
        private TimeManager _timeManager;
        private SoundManager _soundManager;

        private MonoBehaviourUtil _monoBehaviourUtil;

        private bool _isGameInitialized;

        private void Awake()
        {
            _isGameInitialized = false;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _timeManager = new TimeManager();
            _timeManager.OnInit();

            _sceneManager = new GameSceneManager();
            _sceneManager.OnInit();

            _cameraManager = new CameraManager();
            _cameraManager.OnInit();

            _configManager = new ConfigDataManager();
            _configManager.OnInit();

            _settingManager = new SettingManager();
            _settingManager.OnInit();

            _csvManager = new CSVManager(); // after setting
            _csvManager.OnInit();

            _inputManager = new InputManager(); // after camera
            _inputManager.OnInit();

            _levelManager = new LevelManager();
            _levelManager.OnInit();

            _statsManager = new StatsManager();
            _statsManager.OnInit();

            _soundManager = new SoundManager();
            _soundManager.OnInit();

            _monoBehaviourUtil = FindObjectOfType<MonoBehaviourUtil>();
            if (_monoBehaviourUtil == null)
            {
                Logger.LogError($"No MonoBehaviourUtil!");
            }

            if (!_isGameInitialized)
            {
                GameSceneManager.Instance.ShowScene(SceneNames.MainMenu, null, () =>
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

            _configManager.OnDispose();
            _configManager = null;

            _csvManager.OnDispose();
            _csvManager = null;

            _inputManager.OnDispose();
            _inputManager = null;

            _settingManager.OnDispose();
            _settingManager = null;

            _levelManager.OnDispose();
            _levelManager = null;

            _statsManager.OnDispose();
            _statsManager = null;

            _timeManager.OnDispose();
            _timeManager = null;

            _soundManager.OnDispose();
            _soundManager = null;

            _monoBehaviourUtil = null;
        }
    }
}