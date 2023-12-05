using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace com.hive.projectr
{
    public struct CoreGameData : ISceneData
    {
        public float spacecraftMovementScale;
        public Vector3 centerScreenPos;

        public CoreGameData(float spacecraftMovementScale, Vector3 centerScreenPos)
        {
            this.spacecraftMovementScale = spacecraftMovementScale;
            this.centerScreenPos = centerScreenPos;
        }
    }

    public class CoreGameController : GameSceneControllerBase
    {
        private enum CoreGameState
        {
            NotStarted,
            StartCountdown,
            Running,
            Paused,
            Finished
        }

        #region Fields
        private float _spacecraftMovementScale;
        private Vector3 _centerScreenPos;
        private Vector3 _spacecraftScreenOffsetFromCursor;
        private Vector3 _spacecraftScreenPos;
        private CoreGameState _state;
        private float _progressFill;
        private float _maxProgressWidth;

        private static readonly int CountdownTriggerHash = Animator.StringToHash("Count");
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
        }

        private enum ExtraTMP
        {
            Countdown = 0,
        }

        private enum ExtraAnimator
        {
            Countdown = 0,
        }

        private enum ExtraImg
        {
            ProgressBar = 0,
        }

        private enum ExtraRT
        {
            SpacecraftRoot = 0,
        }

        private NetController _netController;
        private SpacecraftController _spacecraftController;

        private Dictionary<VacuumType, VacuumController> _vacuumControllers;

        private TMP_Text _countdownText;

        private Animator _countdownAnimator;

        private Image _progressBar;

        private RectTransform _spacecraftRoot;
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

            var spacecraftConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.Spacecraft];
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

            _countdownText = Config.ExtraTextMeshPros[(int)ExtraTMP.Countdown];

            _countdownAnimator = Config.ExtraAnimators[(int)ExtraAnimator.Countdown];

            _progressBar = Config.ExtraImages[(int)ExtraImg.ProgressBar];

            _spacecraftRoot = Config.ExtraRectTransforms[(int)ExtraRT.SpacecraftRoot];
            _maxProgressWidth = ((RectTransform)_spacecraftRoot.parent).rect.width;
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameData pData)
            {
                _spacecraftMovementScale = pData.spacecraftMovementScale;
                _centerScreenPos = pData.centerScreenPos;
            }

            if (showState == GameSceneShowState.New)
            {
                Reset();
                Start();
            }

            InputManager.HideCursor();
            InputManager.SetCursorLockMode(CursorLockMode.None);
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            InputManager.ShowCursor();
            InputManager.SetCursorLockMode(CursorLockMode.Confined);
        }

        protected override void OnDispose()
        {
            UnbindActions();

            _vacuumControllers.Clear();
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
            if (_state != CoreGameState.Running)
                return;

            SpacecraftMovementTick();

            // debug
            if (InputManager.GetKeyDown(KeyCode.UpArrow))
            {
                _vacuumControllers[VacuumType.Top].Activate();
            }
            else if (InputManager.GetKeyUp(KeyCode.UpArrow))
            {
                _vacuumControllers[VacuumType.Top].Deactivate();
            }
            else if (InputManager.GetKeyDown(KeyCode.LeftArrow))
            {
                _vacuumControllers[VacuumType.Left].Activate();
            }
            else if(InputManager.GetKeyUp(KeyCode.LeftArrow))
            {
                _vacuumControllers[VacuumType.Left].Deactivate();
            }
            else if (InputManager.GetKeyDown(KeyCode.RightArrow))
            {
                _vacuumControllers[VacuumType.Right].Activate();
            }
            else if (InputManager.GetKeyUp(KeyCode.RightArrow))
            {
                _vacuumControllers[VacuumType.Right].Deactivate();
            }
            else if (InputManager.GetKeyDown(KeyCode.DownArrow))
            {
                _vacuumControllers[VacuumType.Bottom].Activate();
            }
            else if (InputManager.GetKeyUp(KeyCode.DownArrow))
            {
                _vacuumControllers[VacuumType.Bottom].Deactivate();
            }
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
        #endregion

        #region Content
        private void SetProgressBar(float fillAmount)
        {
            fillAmount = Mathf.Clamp01(fillAmount);

            if (fillAmount == _progressFill)
                return;

            _progressFill = fillAmount;

            // fill amount
            _progressBar.fillAmount = _progressFill;

            // marker pos
            var xPos = Mathf.Lerp(0, _maxProgressWidth, _progressFill);
            _spacecraftRoot.anchoredPosition = new Vector2(xPos, _spacecraftRoot.anchoredPosition.y);
        }

        private void Reset()
        {
            UpdateState(CoreGameState.NotStarted);

            foreach (var controller in _vacuumControllers.Values)
            {
                controller.Deactivate();
            }

            _spacecraftController.Deactivate();
            CenterSpacecraft();
        }

        private void Start()
        {
            _spacecraftController.Activate();

            if (_state == CoreGameState.NotStarted)
            {
                MonoBehaviourUtil.Instance.StartCoroutine(StartRoutine());
            }
            else if (_state == CoreGameState.Paused)
            {
                UpdateState(CoreGameState.Running);
            }
        }

        private IEnumerator StartRoutine()
        {
            UpdateState(CoreGameState.StartCountdown);

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

            CenterSpacecraft();
            UpdateState(CoreGameState.Running);
        }

        private void UpdateState(CoreGameState state)
        {
            _state = state;
            Logger.LogError($"UpdateState: {state}");
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
    }
}