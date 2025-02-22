using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class GameManager
    /// @brief Manages the overall game flow, including initializing systems, handling game state, and managing core game features.
    ///
    /// The GameManager class is responsible for initializing all essential game systems such as scene management, camera,
    /// configuration, input, and other core services. It ensures proper initialization and management of the game's systems
    /// and handles game state transitions like entering the main menu or quitting the game.
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the singleton instance of the GameManager.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Indicates whether the game is currently focused (active).
        /// </summary>
        public bool IsFocused => _monoBehaviourUtil != null ? _monoBehaviourUtil.IsFocused : false;

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
        private MailManager _mailManager;

        private MonoBehaviourUtil _monoBehaviourUtil;

        private bool _isGameInitialized;

        /// <summary>
        /// Initializes the GameManager and sets up various game systems such as time management, scene management, input,
        /// camera, and other core systems. It also ensures the game starts in the main menu.
        /// </summary>
        private void Awake()
        {
            Instance = this;

            if (SystemInfo.systemMemorySize >= 1024)
            {
                Application.targetFrameRate = 120;
            }
            else
            {
                Application.targetFrameRate = 60;
            }

            QualitySettings.vSyncCount = 0;

            _isGameInitialized = false;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes all game systems when the game object is enabled. This includes setting up services like the scene manager,
        /// input manager, level manager, and more.
        /// </summary>
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

            _mailManager = new MailManager();
            _mailManager.OnInit();

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

        /// <summary>
        /// Cleans up and disposes of all game systems when the game object is disabled.
        /// </summary>
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

            _mailManager.OnDispose();
            _mailManager = null;

            _monoBehaviourUtil = null;
        }

        /// <summary>
        /// Quits the game, either stopping play mode in the Unity editor or closing the application in a built version.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}