using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

namespace com.hive.projectr
{
    public struct CoreGameData : ISceneData
    {
        public Vector3 bottomLeftScreenPos;
        public Vector3 topRightScreenPos;
        public Vector3 centerScreenPos;
        public float spacecraftMovementScale;
        public int level;

        public CoreGameData(Vector3 bottomLeftScreenPos, Vector3 topRightScreenPos, Vector3 centerScreenPos, float spacecraftMovementScale, int level)
        {
            this.bottomLeftScreenPos = bottomLeftScreenPos;
            this.topRightScreenPos = topRightScreenPos;
            this.centerScreenPos = centerScreenPos;
            this.spacecraftMovementScale = spacecraftMovementScale;
            this.level = level;
        }
    }

    public class CoreGameController : GameSceneControllerBase
    {
        private enum CoreGameState
        {
            NotStarted,
            Countdown,
            Running,
            Paused,
            Finished
        }

        #region Fields
        private Vector3 _bottomLeftScreenPos;
        private Vector3 _topRightScreenPos;
        private float _spacecraftMovementScale;
        private Vector3 _centerScreenPos;
        private Vector3 _spacecraftScreenOffsetFromCursor;
        private Vector3 _spacecraftScreenPos;
        private CoreGameState _state;
        private float _currentGameProgressFill = -1;
        private float _targetGameProgressFill;
        private float _currentAsteroidProgressFill = -1;
        private CoreGameLevelConfigData _levelConfigData;
        private int _endedAsteroidCount;
        private int _collectedAsteroidCount;
        private float _nextAsteroidSpawnTime;
        private float _levelRunningTime;
        private string _currentInfo;

        private static readonly int CountdownTriggerHash = Animator.StringToHash("Count");
        #endregion

        #region Pool
        private Dictionary<int, AsteroidController> _activeAsteroids = new Dictionary<int, AsteroidController>();
        private Stack<AsteroidController> _inactiveAsteroids = new Stack<AsteroidController>();
        #endregion

        #region Extra
        private enum ExtraConfig
        {
            Net = 0,
            Spacecraft = 1,
        }

        private enum ExtraObj
        {
            VacuumContainer = 0,
            AsteroidContainer = 1,
            RaycastBlocker = 2,
        }

        private enum ExtraTMP
        {
            Countdown = 0,
            Info = 1,
        }

        private enum ExtraAnimator
        {
            Countdown = 0,
        }

        private enum ExtraImg
        {
            GameProgressBar = 0,
            AsteroidProgressBar = 1,
        }

        private enum ExtraAnyObj
        {
            AsteroidPrefab = 0,
        }

        private NetController _netController;
        private SpacecraftController _spacecraftController;

        private Dictionary<VacuumType, VacuumController> _vacuumControllers;
        private Transform _asteroidContainer;
        private Transform _raycastBlocker;

        private TMP_Text _countdownText;
        private TMP_Text _infoText;

        private Animator _countdownAnimator;

        private Image _gameProgressBar;
        private Image _asteroidProgressBar;

        private GameObject _asteroidPrefab;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            var netConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.Net];
            _netController = new NetController(netConfig);

            var spacecraftConfig = (SpacecraftConfig)Config.ExtraWidgetConfigs[(int)ExtraConfig.Spacecraft];
            _spacecraftController = new SpacecraftController(spacecraftConfig);

            var vacuumContainer = Config.ExtraObjects[(int)ExtraObj.VacuumContainer];
            var vacuumConfigs = vacuumContainer.GetComponentsInChildren<VacuumConfig>();
            _vacuumControllers = new Dictionary<VacuumType, VacuumController>();
            foreach (var vacuumConfig in vacuumConfigs)
            {
                if (!_vacuumControllers.ContainsKey(vacuumConfig.Type))
                {
                    _vacuumControllers[vacuumConfig.Type] = new VacuumController(vacuumConfig);
                }
            }
            _asteroidContainer = Config.ExtraObjects[(int)ExtraObj.AsteroidContainer];
            _raycastBlocker = Config.ExtraObjects[(int)ExtraObj.RaycastBlocker];

            _countdownText = Config.ExtraTextMeshPros[(int)ExtraTMP.Countdown];
            _infoText = Config.ExtraTextMeshPros[(int)ExtraTMP.Info];

            _countdownAnimator = Config.ExtraAnimators[(int)ExtraAnimator.Countdown];

            _gameProgressBar = Config.ExtraImages[(int)ExtraImg.GameProgressBar];
            _asteroidProgressBar = Config.ExtraImages[(int)ExtraImg.AsteroidProgressBar];

            _asteroidPrefab = (GameObject)Config.ExtraGameObjects[(int)ExtraAnyObj.AsteroidPrefab];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameData pData)
            {
                _bottomLeftScreenPos = pData.bottomLeftScreenPos;
                _topRightScreenPos = pData.topRightScreenPos;
                _spacecraftMovementScale = pData.spacecraftMovementScale;
                _centerScreenPos = pData.centerScreenPos;
                _levelConfigData = CoreGameLevelConfig.GetLevelData(pData.level);
            }

            if (showState == GameSceneShowState.New)
            {
                Reset();
                Start();
            }
            else
            {
                Resume();
            }

            SoundManager.Instance.PlaySound(SoundType.CoreGameBackground);
            SoundManager.Instance.StopSound(SoundType.MenuBackground);
            SoundManager.Instance.StopSound(SoundType.CalibrationBackground);

            InputManager.HideCursor();
            InputManager.SetCursorLockMode(CursorLockMode.None);
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            if (hideState == GameSceneHideState.Covered)
            {
                Pause();
                CSVManager.Instance.PauseRecording();
            }
            else
            {
                CSVManager.Instance.StopRecording();
            }

            SoundManager.Instance.StopSound(SoundType.CoreGameBackground);

            InputManager.ShowCursor();
            InputManager.SetCursorLockMode(CursorLockMode.Confined);
        }

        protected override void OnDispose()
        {
            UnbindActions();

            _vacuumControllers.Clear();
            _spacecraftController.Dispose();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            MonoBehaviourUtil.OnUpdate += Tick;
        }

        private void UnbindActions()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        private void Tick()
        {
            if (_state == CoreGameState.Running)
            {
                if (_activeAsteroids.Count < 1 && Time.time >= _nextAsteroidSpawnTime)
                {
                    SpawnAsteroid();
                }

                InfoTextTick();
                AsteroidProgressTick();
                SpacecraftMovementTick();
            }

            GameProgressTick();
        }

        private void OnAsteroidEnterVacuumAir(int id)
        {
            ++_collectedAsteroidCount;
            OnAsteroidEnded(id);
        }

        private void OnAsteroidLifetimeRunOut(int id)
        {
            OnAsteroidEnded(id);
        }

        private void OnAsteroidSpawned()
        {
            _netController.PlaySpawnAnimation();

            SoundManager.Instance.PlaySound(SoundType.AsteroidSpawn);
        }

        private void OnAsteroidCaptured(int id)
        {
            _spacecraftController.SetCapturing(true);

            // activate a random vacuum
            var vacuumControllers = new List<VacuumController>(_vacuumControllers.Values);
            var index = Random.Range(0, vacuumControllers.Count);
            var activeController = vacuumControllers[index];
            activeController.Activate();

            SoundManager.Instance.PlaySound(SoundType.AsteroidCaught);
        }

        private void OnAsteroidEnded(int id)
        {
            SoundManager.Instance.PlaySound(SoundType.AsteroidGone);

            DestroyAsteroid(id);

            _nextAsteroidSpawnTime = Time.time + _levelConfigData.AsteroidSpawnGapSec;

            _spacecraftController.SetCapturing(false);

            foreach (var controller in _vacuumControllers.Values)
            {
                controller.Deactivate();
            }

            ++_endedAsteroidCount;
            UpdateGameProgress(false);

            if (_endedAsteroidCount >= _levelConfigData.MaxAsteroidCount)
            {
                OnLevelEnded();
            }
        }

        private void OnLevelEnded()
        {
            MonoBehaviourUtil.Instance.StartCoroutine(LevelEndRoutine());
        }

        private void SpacecraftMovementTick()
        {
            var spacecraftScreenPos = GetSpacecraftScreenPos();
            var spacecraftSpeed = (_spacecraftScreenPos - spacecraftScreenPos).sqrMagnitude / Time.deltaTime;
            _spacecraftScreenPos = spacecraftScreenPos;
            _spacecraftController.SetMoving(spacecraftSpeed > .1f);

            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(spacecraftScreenPos);
            _spacecraftController.SetWorldPos(new Vector3(spacecraftWorldPos.x, spacecraftWorldPos.y, _spacecraftController.GetWorldPos().z));
        }

        private void GameProgressTick()
        {
            if (_currentGameProgressFill < _targetGameProgressFill)
            {
                var delta = (_targetGameProgressFill - _currentGameProgressFill) / CoreGameConfig.GetData().GameProgressFillSec * Time.deltaTime;
                _currentGameProgressFill = Mathf.Clamp01(_currentGameProgressFill + delta);
                _gameProgressBar.fillAmount = _currentGameProgressFill;
            }
        }

        private void AsteroidProgressTick()
        {
            var activeAsteroids = new List<AsteroidController>(_activeAsteroids.Values);
            var fill = activeAsteroids.Count > 0
                ? activeAsteroids[0].GetLifetimeProgress()
                : 0;

            if (_currentAsteroidProgressFill < fill)
            {
                _asteroidProgressBar.fillAmount = _currentAsteroidProgressFill = fill;
            }
        }

        private void InfoTextTick()
        {
            if (_state == CoreGameState.Running)
            {
                var coreGameData = CoreGameConfig.GetData();
                _levelRunningTime += Time.deltaTime;
                var infoIndex = ((int)_levelRunningTime / coreGameData.InfoTextUpdateSec) % 2;
                switch (infoIndex)
                {
                    case 0:
                        RefreshInfoText($"Catch, Destroy, Collect");
                        break;
                    case 1:
                        RefreshInfoText($"Level {_levelConfigData.Level}");
                        break;
                    default:
                        Debug.LogError($"Undefined core game info index: {infoIndex}");
                        break;
                }
            }
        }
        #endregion

        #region Content
        private IEnumerator LevelEndRoutine()
        {
            _raycastBlocker.gameObject.SetActive(true);

            UpdateState(CoreGameState.Finished);

            yield return new WaitForSeconds(_levelConfigData.AsteroidSpawnGapSec);

            var isPassed = _collectedAsteroidCount >= _levelConfigData.NumOfAsteroidCollectedToPass;

            LevelManager.Instance.OnLevelCompleted(_levelConfigData.Level, isPassed);
            StatsManager.Instance.OnLevelCompleted(new LevelStats(_levelConfigData.Level, _collectedAsteroidCount));

            SoundManager.Instance.PlaySound(SoundType.CoreGameLevelEnd);

            if (isPassed)
            {
                GameSceneManager.Instance.ShowScene(SceneNames.CoreGameLevelPassed, new CoreGameLevelPassedData(new CoreGameData(_bottomLeftScreenPos, _topRightScreenPos, _centerScreenPos, _spacecraftMovementScale, _levelConfigData.Level)), () =>
                {
                    GameSceneManager.Instance.HideScene(SceneName);
                });
            }
            else
            {
                GameSceneManager.Instance.ShowScene(SceneNames.CoreGameLevelFailed, new CoreGameLevelFailedData(new CoreGameData(_bottomLeftScreenPos, _topRightScreenPos, _centerScreenPos, _spacecraftMovementScale, _levelConfigData.Level)), () =>
                {
                    GameSceneManager.Instance.HideScene(SceneName);
                });
            }

            _raycastBlocker.gameObject.SetActive(false);
        }

        private void DestroyAsteroid(int id)
        {
            if (_activeAsteroids.TryGetValue(id, out var controller))
            {
                controller.Stop(() => 
                {
                    PutAsteroidBack(controller);
                    _asteroidProgressBar.fillAmount = _currentAsteroidProgressFill = 0;
                });
            }
        }

        private void RefreshInfoText(string info)
        {
            if (!string.IsNullOrEmpty(_currentInfo) && _currentInfo == info)
                return;

            _currentInfo = info;
            _infoText.text = _currentInfo;
        }

        private void UpdateGameProgress(bool isInstant)
        {
            var fillAmount = Mathf.Clamp01((float)_endedAsteroidCount / _levelConfigData.MaxAsteroidCount);
            fillAmount = Mathf.Clamp01(fillAmount);

            _targetGameProgressFill = fillAmount;

            if (isInstant)
            {
                _gameProgressBar.fillAmount = _currentGameProgressFill = _targetGameProgressFill;
            }
        }

        private void Reset()
        {
            UpdateState(CoreGameState.NotStarted);

            // vacuums
            foreach (var controller in _vacuumControllers.Values)
            {
                controller.Deactivate();
                controller.SetAirSize(_levelConfigData.VacuumSize);
            }

            _spacecraftController.Deactivate();
            CenterSpacecraft();

            RefreshInfoText("");
        }

        private void Resume()
        {
            MonoBehaviourUtil.Instance.StartCoroutine(CountdownRoutine(() =>
            {
                UpdateState(CoreGameState.Running);
            }));
        }

        private void Pause()
        {
            UpdateState(CoreGameState.Paused);
        }

        private void Start()
        {
            if (_state == CoreGameState.NotStarted)
            {
                _spacecraftController.Deactivate();
                MonoBehaviourUtil.Instance.StartCoroutine(CountdownRoutine(() =>
                {
                    _nextAsteroidSpawnTime = Time.time + _levelConfigData.AsteroidSpawnGapSec;
                    _spacecraftController.Activate(new SpacecraftData(_levelConfigData.SpacecraftSize));
                    CenterSpacecraft();
                    UpdateState(CoreGameState.Running);

                    CSVManager.Instance.StartRecording(_spacecraftController.Transform);
                }));
            }
            else if (_state == CoreGameState.Paused)
            {
                _spacecraftController.Activate(new SpacecraftData(_levelConfigData.SpacecraftSize));
                UpdateState(CoreGameState.Running);

                CSVManager.Instance.StartRecording(_spacecraftController.Transform);
            }

            LevelManager.Instance.OnLevelStarted(_levelConfigData.Level);
            UpdateGameProgress(true);
        }

        private IEnumerator CountdownRoutine(Action onCountdownFinished)
        {
            UpdateState(CoreGameState.Countdown);

            var countNumAnimStateName = "CountNum";
            var duration = 0f;
            foreach (var clip in _countdownAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Equals(countNumAnimStateName))
                {
                    duration = clip.length;
                    break;
                }
            }

            var num = CoreGameConfig.GetData().StartCountdownSec + 1;
            var currentNum = num;
            _countdownText.text = $"{num}";
            _countdownAnimator.SetTrigger(CountdownTriggerHash);

            while (num > 0)
            {
                if (!_countdownAnimator.GetCurrentAnimatorStateInfo(0).IsName(countNumAnimStateName) &&
                    num == currentNum)
                {
                    --num;
                }

                if (num > 0 && currentNum != num)
                {
                    currentNum = num;
                    _countdownText.text = $"{currentNum}";
                    _countdownAnimator.SetTrigger(CountdownTriggerHash);
                }

                yield return null;
            }

            onCountdownFinished?.Invoke();
        }

        private void UpdateState(CoreGameState state)
        {
            _state = state;
        }

        private Vector3 GetSpacecraftScreenPosRaw()
        {
            return InputManager.Instance.CursorScreenPos + _spacecraftScreenOffsetFromCursor;
        }

        private Vector3 GetSpacecraftScreenPos()
        {
            var screenPos = GetSpacecraftScreenPosRaw();

            screenPos = new Vector3(screenPos.x - _centerScreenPos.x, screenPos.y - _centerScreenPos.y, screenPos.z);
            screenPos = new Vector3(screenPos.x * _spacecraftMovementScale, screenPos.y * _spacecraftMovementScale, screenPos.z);
            screenPos = new Vector3(screenPos.x + _centerScreenPos.x, screenPos.y + _centerScreenPos.y, screenPos.z);

            return screenPos;
        }

        private void CenterSpacecraft()
        {
            _spacecraftScreenOffsetFromCursor = _centerScreenPos - InputManager.Instance.CursorScreenPos;
        }
        #endregion

        #region Asteroid
        private void SpawnAsteroid()
        {
            var controller = GetAsteroidController();

            var degree = Random.Range(0, 360);
            var radian = Mathf.Deg2Rad * degree;
            var spacecraftScreenPos = GetSpacecraftScreenPos();
            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(spacecraftScreenPos);
            var spacecraftRadius = _spacecraftController.GetWorldSpaceCaptureRadius();
            var startWorldPos = GetRandomWorldPointOutsideCircle(spacecraftWorldPos, spacecraftRadius);
            var startScreenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(startWorldPos);

            //var startDirection = (_centerScreenPos - startScreenPos);
            var startRad = Random.Range(0, Mathf.PI * 2);
            var startDirection = new Vector2(Mathf.Cos(startRad), Mathf.Sin(startRad));

            var id = controller.Id;
            controller.Start(new AsteroidData(startWorldPos, startDirection, _levelConfigData.AsteroidSpeed, _levelConfigData.AsteroidSize, _levelConfigData.AsteroidLifeTime, _levelConfigData.AsteroidMovement, OnAsteroidEnterVacuumAir, OnAsteroidLifetimeRunOut, OnAsteroidCaptured));

            OnAsteroidSpawned();
        }

        private Vector3 GetRandomWorldPointOutsideCircle(Vector3 circleWorldPos, float radius)
        {
            var cam = CameraManager.Instance.MainCamera;
            var randomPoint = Vector3.zero;
            var sqrRadius = radius * radius;

            do
            {
                // Convert screen edges to world space
                var bottomLeftWorldPos = cam.ScreenToWorldPoint(_bottomLeftScreenPos) + new Vector3(2, 2);
                var topRightWorldPos = cam.ScreenToWorldPoint(_topRightScreenPos) + new Vector3(-2, -2);
                float minX = bottomLeftWorldPos.x;
                float maxX = topRightWorldPos.x;
                float minY = bottomLeftWorldPos.y;
                float maxY = topRightWorldPos.y;

                // Generate a random point within screen bounds
                randomPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));

                // Check if the point is outside the circle's radius
            } while ((randomPoint - circleWorldPos).sqrMagnitude <= sqrRadius);

            return randomPoint;
        }

        private AsteroidController GetAsteroidController()
        {
            AsteroidController controller = null;

            if (_inactiveAsteroids.Count < 1)
            {
                var asteroidGO = GameObject.Instantiate(_asteroidPrefab, _asteroidContainer);
                var config = asteroidGO.GetComponent<AsteroidConfig>();
                controller = new AsteroidController(config);
            }
            else
            {
                controller = _inactiveAsteroids.Pop();
            }
            
            _activeAsteroids[controller.Id] = controller;

            controller.Activate();
            return controller;
        }

        private void PutAsteroidBack(AsteroidController controller)
        {
            if (controller == null)
            {
                Logger.LogError($"Null AsteroidController!");
                return;
            }

            _activeAsteroids.Remove(controller.Id);
            _inactiveAsteroids.Push(controller);
            controller.Deactivate();
        }
        #endregion
    }
}