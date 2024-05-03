using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace com.hive.projectr
{
    public struct CalibrationStageResult
    {
        public Vector3 worldPos;

        public CalibrationStageResult(Vector3 worldPos)
        {
            this.worldPos = worldPos;
        }
    }

    public enum CalibrationStatus
    {
        NotStarted,
        Running,
        Paused,
        Ended,
    }

    public class CalibrationController : GameSceneControllerBase
    {
        #region Fields
        private int _stageIndex;
        private CalibrationStatus _status;
        private List<CalibrationStageConfigData> _stageDataList;
        private Dictionary<CalibrationStageType, CalibrationStageResult> _resultDict = new Dictionary<CalibrationStageType, CalibrationStageResult>();
        private Coroutine _endCoroutine;
        private float _holdingTime;
        private float _nextCanHoldTime;
        private float _stageElapsedTime;
        private Vector3 _spacecraftScreenOffsetFromCursor;
        private Vector3 _holdingStartScreenPos;
        private bool _isHolding;
        private Vector3 _spacecraftScreenPos;
        private Vector3 _lastCursorScreenPos;
        private float _idleDuration;
        #endregion

        #region Extra
        private enum ExtraConfig
        {
            TopLeftMark = 0,
            TopRightMark = 1,
            BottomRightMark = 2,
            BottomLeftMark = 3,
            CenterMark = 4,
            Marker = 5,
            Spacecraft = 6,
        }

        private enum ExtraBtn
        {
            Cross = 0,
            Redo = 1,
            Contact = 2,
            Question = 3,
        }

        private enum ExtraTMP
        {
            Instruction = 0,
        }

        private enum ExtraObj
        {
            LineDrawer = 0,
        }

        private enum ExtraRT
        {
            TopLeftCorner = 0,
            TopRightCorner = 1,
            BottomRightCorner = 2,
            BottomLeftCorner = 3,
            Arrow = 4,
        }

        private enum ExtraImg
        {
            RaycastBlock = 0,
        }
        
        private Dictionary<CalibrationStageType, CalibrationMarkController> _markDict;
        private CalibrationMarkerController _marker;
        private SpacecraftController _spacecraftController;

        private HiveButton _crossButton;
        private HiveButton _redoButton;
        private HiveButton _contactButton;
        private HiveButton _questionButton;

        private TMP_Text _instructionText;

        private LineDrawer _lineDrawer;

        private Dictionary<CalibrationStageType, RectTransform> _cornerDict;
        private RectTransform _arrow;

        private Image _raycastBlock;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();

            _stageDataList = CalibrationStageConfig.GetAllStageData();
            _raycastBlock.enabled = false;

            Reset();
        }

        private void InitExtra()
        {
            var topLeftMarkConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.TopLeftMark];
            var topRightMarkConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.TopRightMark];
            var bottomRightMarkConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.BottomRightMark];
            var bottomLeftMarkConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.BottomLeftMark];
            var centerMarkConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.CenterMark];
            _markDict = new Dictionary<CalibrationStageType, CalibrationMarkController>() {
                { CalibrationStageType.TopLeft, new CalibrationMarkController(topLeftMarkConfig) },
                { CalibrationStageType.TopRight, new CalibrationMarkController(topRightMarkConfig) },
                { CalibrationStageType.BottomRight, new CalibrationMarkController(bottomRightMarkConfig) },
                { CalibrationStageType.BottomLeft, new CalibrationMarkController(bottomLeftMarkConfig) },
                { CalibrationStageType.Center, new CalibrationMarkController(centerMarkConfig) },
            };
            var markerConfig = Config.ExtraWidgetConfigs[(int)ExtraConfig.Marker];
            _marker = new CalibrationMarkerController(markerConfig);
            var spacecraftConfig = (SpacecraftConfig)Config.ExtraWidgetConfigs[(int)ExtraConfig.Spacecraft];
            _spacecraftController = new SpacecraftController(spacecraftConfig);

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _redoButton = Config.ExtraButtons[(int)ExtraBtn.Redo];
            _contactButton = Config.ExtraButtons[(int)ExtraBtn.Contact];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];

            _instructionText = Config.ExtraTextMeshPros[(int)ExtraTMP.Instruction];

            _lineDrawer = Config.ExtraObjects[(int)ExtraObj.LineDrawer].GetComponent<LineDrawer>();
            _lineDrawer.Deactivate();

            var topLeftCorner = Config.ExtraRectTransforms[(int)ExtraRT.TopLeftCorner];
            var topRightCorner = Config.ExtraRectTransforms[(int)ExtraRT.TopRightCorner];
            var bottomRightCorner = Config.ExtraRectTransforms[(int)ExtraRT.BottomRightCorner];
            var bottomLeftCorner = Config.ExtraRectTransforms[(int)ExtraRT.BottomLeftCorner];
            _cornerDict = new Dictionary<CalibrationStageType, RectTransform>() { 
                { CalibrationStageType.TopLeft, topLeftCorner },
                { CalibrationStageType.TopRight, topRightCorner },
                { CalibrationStageType.BottomRight , bottomRightCorner },
                { CalibrationStageType.BottomLeft, bottomLeftCorner },
            };
            _arrow = Config.ExtraRectTransforms[(int)ExtraRT.Arrow];

            _raycastBlock = Config.ExtraImages[(int)ExtraImg.RaycastBlock];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (showState == GameSceneShowState.New)
            {
                Start();
            }
            else
            {
                if (_status == CalibrationStatus.Paused)
                {
                    Resume();
                }
            }

            SoundManager.Instance.PlaySound(SoundType.CalibrationBackground);
            SoundManager.Instance.StopSound(SoundType.MenuBackground);

            InputManager.Instance.HideCursor();
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            if (_status == CalibrationStatus.Running)
            {
                Pause();
            }

            InputManager.Instance.ShowCursor();
        }

        protected override void OnDispose()
        {
            UnbindActions();

            _spacecraftController.Dispose();

            if (_endCoroutine != null)
            {
                if (MonoBehaviourUtil.Instance != null)
                {
                    MonoBehaviourUtil.Instance.StopCoroutine(_endCoroutine);
                }
                _endCoroutine = null;
            }
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _crossButton.onClick.AddListener(OnCrossButtonClick);
            _redoButton.onClick.AddListener(OnRedoButtonClick);
            _contactButton.onClick.AddListener(OnContactButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);

            MonoBehaviourUtil.OnUpdate += Tick;
            MonoBehaviourUtil.OnApplicationFocusLost += OnFocusLost;
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _redoButton.onClick.RemoveAllListeners();
            _contactButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();

            MonoBehaviourUtil.OnUpdate -= Tick;
            MonoBehaviourUtil.OnApplicationFocusLost -= OnFocusLost;
        }
        #endregion

        #region Content
        private bool IsIdle()
        {
            if (_idleDuration < -.5f)
                return false;

            return true;
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        private void Start()
        {
            UpdateStatus(CalibrationStatus.Running);
            _nextCanHoldTime = Time.time;

            var minLevelData = CoreGameLevelConfig.GetLevelData(CoreGameLevelConfig.MinLevel);
            _spacecraftController.Activate(new SpacecraftData(minLevelData.SpacecraftSize));

            CSVManager.Instance.OnCalibrationStarted(TimeUtil.Now);
        }

        private void Resume()
        {
            UpdateStatus(CalibrationStatus.Running);
        }

        private void Pause()
        {
            UpdateStatus(CalibrationStatus.Paused);
        }

        private void UpdateStatus(CalibrationStatus status)
        {
            _status = status;
        }

        private void Reset()
        {
            _resultDict.Clear();
            _stageIndex = 0;
            UpdateStatus(CalibrationStatus.NotStarted);
            _raycastBlock.enabled = false;
            _spacecraftScreenOffsetFromCursor = Vector3.zero;
            _stageElapsedTime = 0;

            foreach (var markContorller in _markDict.Values)
            {
                markContorller.Deactivate();
            }

            StopHolding();
            _marker.Deactivate();
        }

        private void End()
        {
            if (_endCoroutine != null)
            {
                if (MonoBehaviourUtil.Instance != null)
                {
                    MonoBehaviourUtil.Instance.StopCoroutine(_endCoroutine);
                }
                _endCoroutine = null;
            }

            _endCoroutine = MonoBehaviourUtil.Instance.StartCoroutine(EndRoutine());
        }

        private void CenterSpacecraft()
        {
            _spacecraftScreenOffsetFromCursor = GetCenterScreenPos() - InputManager.Instance.CursorScreenPosition;
        }

        private Vector3 GetCenterScreenPos()
        {
            var topLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopLeft].position);
            var bottomRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomRight].position);
            return (topLeftScreenPos + bottomRightScreenPos) / 2;
        }

        private void OnStageCompleted()
        {
            SoundManager.Instance.PlaySound(SoundType.CalibrationMarkSet);

            var currentStageData = _stageDataList[_stageIndex];
            _holdingTime = 0;
            _stageElapsedTime = 0;

            _nextCanHoldTime = Time.time + currentStageData.CooldownTime;

            // mark
            if (_markDict.TryGetValue(currentStageData.Stage, out var markController))
            {
                if (currentStageData.Stage == CalibrationStageType.Center)
                {
                    // center of movement area (COULD BE the screen center)
                    var centerScreenPos = GetCenterScreenPos();
                    var targetWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(centerScreenPos);
                    markController.MoveToWorldPos(targetWorldPos);

                    CenterSpacecraft();

                    _marker.SetWorldPosition(targetWorldPos);
                }
                else
                {
                    var targetScreenPos = GetSpacecraftScreenPos();
                    var targetWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(targetScreenPos);
                    markController.MoveToWorldPos(targetWorldPos);
                    _resultDict[currentStageData.Stage] = new CalibrationStageResult(targetWorldPos);
                }

                markController.Activate();
            }

            if (_stageIndex == _stageDataList.Count - 1) // all finished
            {
                End(); 
            }
            else
            {
                _marker.OnSuccess();
                ++_stageIndex;
            }

            StopHolding();
        }

        private Vector3 GetSpacecraftScreenPos()
        {
            return InputManager.Instance.CursorScreenPosition + _spacecraftScreenOffsetFromCursor;
        }

        private IEnumerator EndRoutine()
        {
            UpdateStatus(CalibrationStatus.Ended);
            _spacecraftController.Deactivate();
            _marker.Deactivate();
            _lineDrawer.Activate();
            _instructionText.text = "";
            _raycastBlock.enabled = true;

            var topLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopLeft].position);
            var topRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopRight].position);
            var bottomRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomRight].position);
            var bottomLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomLeft].position);

            CSVManager.Instance.OnCalibrationEnded(new CSVCalibrationEndedData(
                Vector2.zero,
                UIUtil.WorldPosToGameCoordinate(_markDict[CalibrationStageType.TopLeft].GetCurrentWorldPos()),
                UIUtil.WorldPosToGameCoordinate(_markDict[CalibrationStageType.TopRight].GetCurrentWorldPos()),
                UIUtil.WorldPosToGameCoordinate(_markDict[CalibrationStageType.BottomRight].GetCurrentWorldPos()),
                UIUtil.WorldPosToGameCoordinate(_markDict[CalibrationStageType.BottomLeft].GetCurrentWorldPos())
            ));

            var topLeftWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(topLeftScreenPos);
            var topRightWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(topRightScreenPos);
            var bottomRightWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(bottomRightScreenPos);
            var bottomLeftWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(bottomLeftScreenPos);

            var targetTopLeftWorldPos = new Vector3(topLeftWorldPos.x, topLeftWorldPos.y);
            var targetTopRightWorldPos = new Vector3(topRightWorldPos.x, topRightWorldPos.y);
            var targetBottomRightWorldPos = new Vector3(bottomRightWorldPos.x, bottomRightWorldPos.y);
            var targetBottomLeftWorldPos = new Vector3(bottomLeftWorldPos.x, bottomLeftWorldPos.y);

            var stagesToLink = new List<CalibrationStageType>() {
                CalibrationStageType.TopLeft,
                CalibrationStageType.TopRight,
                CalibrationStageType.BottomRight,
                CalibrationStageType.BottomLeft,
            };
            var firstStage = stagesToLink[0];
            var firstPoint = _markDict[firstStage].GetCurrentWorldPos();
            var points = new List<Vector3>() { firstPoint };
            var reachedIndex = 0;
            var reachedCount = 0;
            var targetIndex = (reachedIndex + 1) % stagesToLink.Count;
            var pointMovementDuration = .3f;
            var pointMovementProgress = 0f;
            while (reachedCount < stagesToLink.Count)
            {
                if (points.Count < reachedCount + 2)
                {
                    points.Add(Vector3.zero);
                }

                var startingStage = stagesToLink[reachedIndex];
                var targetStage = stagesToLink[targetIndex];
                var startingWorldPos = _markDict[startingStage].GetCurrentWorldPos();
                var targetWorldPos = _markDict[targetStage].GetCurrentWorldPos();

                pointMovementProgress += 1 / pointMovementDuration * Time.deltaTime;
                points[points.Count - 1] = Vector3.Lerp(startingWorldPos, targetWorldPos, pointMovementProgress);
                _lineDrawer.Draw(points.ToArray());

                if (Vector3.SqrMagnitude(points[points.Count - 1] - targetWorldPos) < .001f) // reached target
                {
                    ++reachedIndex;
                    ++reachedCount;
                    targetIndex = (reachedIndex + 1) % stagesToLink.Count;

                    pointMovementProgress = 0;
                    points.Add(targetWorldPos);
                }

                yield return null;
            }

            var topLeftRefVelocity = Vector3.zero;
            var topRightRefVelocity = Vector3.zero;
            var bottomRightRefVelocity = Vector3.zero;
            var bottomLeftRefVelocity = Vector3.zero;
            var moveToCornerDuration = .35f;
            while (Vector3.SqrMagnitude(_markDict[CalibrationStageType.TopLeft].GetCurrentWorldPos() - targetTopLeftWorldPos) > .001f)
            {
                _markDict[CalibrationStageType.TopLeft].MoveToWorldPos(Vector3.SmoothDamp(_markDict[CalibrationStageType.TopLeft].GetCurrentWorldPos(), targetTopLeftWorldPos, ref topLeftRefVelocity, moveToCornerDuration));
                _markDict[CalibrationStageType.TopRight].MoveToWorldPos(Vector3.SmoothDamp(_markDict[CalibrationStageType.TopRight].GetCurrentWorldPos(), targetTopRightWorldPos, ref topRightRefVelocity, moveToCornerDuration));
                _markDict[CalibrationStageType.BottomRight].MoveToWorldPos(Vector3.SmoothDamp(_markDict[CalibrationStageType.BottomRight].GetCurrentWorldPos(), targetBottomRightWorldPos, ref bottomRightRefVelocity, moveToCornerDuration));
                _markDict[CalibrationStageType.BottomLeft].MoveToWorldPos(Vector3.SmoothDamp(_markDict[CalibrationStageType.BottomLeft].GetCurrentWorldPos(), targetBottomLeftWorldPos, ref bottomLeftRefVelocity, moveToCornerDuration));

                _lineDrawer.Draw(new Vector3[] 
                {
                    _markDict[CalibrationStageType.TopLeft].GetCurrentWorldPos(),
                    _markDict[CalibrationStageType.TopRight].GetCurrentWorldPos(),
                    _markDict[CalibrationStageType.BottomRight].GetCurrentWorldPos(),
                    _markDict[CalibrationStageType.BottomLeft].GetCurrentWorldPos(),
                    _markDict[CalibrationStageType.TopLeft].GetCurrentWorldPos(),
                });

                yield return null;
            }

            yield return new WaitForSeconds(.25f);

            _raycastBlock.enabled = false;

            var scaleFactor = 1f;
            var centerScreenPos = GetCenterScreenPos();
            foreach (var section in _cornerDict.Keys)
            {
                if (_cornerDict.TryGetValue(section, out var actualCorner) &&
                    _resultDict.TryGetValue(section, out var result))
                {
                    var actualCornerScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, actualCorner.position);

                    var playerCornerWorldPos = result.worldPos;
                    var playerCornerScreenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(playerCornerWorldPos);

                    actualCornerScreenPos = new Vector2(actualCornerScreenPos.x - centerScreenPos.x, actualCornerScreenPos.y - centerScreenPos.y);
                    playerCornerScreenPos = new Vector2(playerCornerScreenPos.x - centerScreenPos.x, playerCornerScreenPos.y - centerScreenPos.y);

                    var scaleVector = new Vector2(actualCornerScreenPos.x / playerCornerScreenPos.x, actualCornerScreenPos.y / playerCornerScreenPos.y);
                    scaleFactor = Mathf.Max(scaleFactor, scaleVector.x, scaleVector.y);
                }
            }

            GameSceneManager.Instance.ShowScene(SceneNames.CalibrationEnding, new CalibrationEndingData(new CoreGameData(
                bottomLeftScreenPos,
                topRightScreenPos,
                centerScreenPos,
                scaleFactor, 
                SettingManager.Instance.Level)), ()=> 
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private bool IsCurrentPositionMatchingStage()
        {
            if (_stageIndex >= 0 && _stageIndex < _stageDataList.Count)
            {
                var currentStageData = _stageDataList[_stageIndex];
                if (currentStageData.Stage != CalibrationStageType.Center)
                {
                    var spacecraftScreenpos = GetSpacecraftScreenPos();
                    var centerScreenPos = GetCenterScreenPos();

                    switch (currentStageData.Stage)
                    {
                        case CalibrationStageType.TopLeft:
                            return spacecraftScreenpos.x < centerScreenPos.x && spacecraftScreenpos.y > centerScreenPos.y;
                        case CalibrationStageType.TopRight:
                            return spacecraftScreenpos.x > centerScreenPos.x && spacecraftScreenpos.y > centerScreenPos.y;
                        case CalibrationStageType.BottomRight:
                            return spacecraftScreenpos.x > centerScreenPos.x && spacecraftScreenpos.y < centerScreenPos.y;
                        case CalibrationStageType.BottomLeft:
                            return spacecraftScreenpos.x < centerScreenPos.x && spacecraftScreenpos.y < centerScreenPos.y;
                    }
                }
            }

            return true;
        }

        private void StartHolding()
        {
            _isHolding = true;

            var spacecraftScreenPos = GetSpacecraftScreenPos();
            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(spacecraftScreenPos);
            _holdingStartScreenPos = spacecraftScreenPos;

            if (_stageIndex >= 0 && _stageIndex < _stageDataList.Count)
            {
                var currentStageData = _stageDataList[_stageIndex];
                _instructionText.text = currentStageData.InstructionWhenHolding;

                _spacecraftController.Deactivate();

                switch (currentStageData.Stage)
                {
                    case CalibrationStageType.Center:
                        _marker.Activate(CalibrationMarkerType.Center);
                        break;
                    case CalibrationStageType.TopLeft:
                        _marker.Activate(CalibrationMarkerType.TopLeft);
                        break;
                    case CalibrationStageType.TopRight:
                        _marker.Activate(CalibrationMarkerType.TopRight);
                        break;
                    case CalibrationStageType.BottomRight:
                        _marker.Activate(CalibrationMarkerType.BottomRight);
                        break;
                    case CalibrationStageType.BottomLeft:
                        _marker.Activate(CalibrationMarkerType.BottomLeft);
                        break;
                }

                _marker.SetWorldPosition(spacecraftWorldPos);
                _marker.OnHoldingStart();
            }
            else
            {
                Logger.LogError($"Invalid stageIndex: {_stageIndex}");
            }
        }

        private void StopHolding()
        {
            _isHolding = false;
            _holdingTime = 0;

            var minLevelData = CoreGameLevelConfig.GetLevelData(CoreGameLevelConfig.MinLevel);
            _spacecraftController.Activate(new SpacecraftData(minLevelData.SpacecraftSize));

            if (_stageIndex >= 0 && _stageIndex < _stageDataList.Count)
            {
                var currentStageData = _stageDataList[_stageIndex];
                _instructionText.text = currentStageData.Instruction;
            }
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            GameSceneManager.Instance.GoBack();
        }

        private void OnRedoButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            Reset();
            Start();
        }

        private void OnContactButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Calibration));
        }

        private void IdleTick()
        {
            var cursorScreenPos = InputManager.Instance.CursorScreenPosition;
            var deltaCursorScreenPos = cursorScreenPos - _lastCursorScreenPos;
            _lastCursorScreenPos = cursorScreenPos;
            var isIdle = IsCurrentPositionMatchingStage() &&
                deltaCursorScreenPos.sqrMagnitude < CalibrationConfig.GetData().HoldingMaxScreenOffset;
            if (isIdle)
            {
                if (_idleDuration < -.5f)
                {
                    _idleDuration = 0;
                }

                _idleDuration += Time.deltaTime;
            }
            else
            {
                _idleDuration = -1;
            }
        }

        private void OnFocusLost()
        {
            Pause();
        }

        private void Tick()
        {
            if (_status != CalibrationStatus.Running)
                return;

            IdleTick();

            _stageElapsedTime += Time.deltaTime;
            if (_stageElapsedTime > CalibrationConfig.GetData().StageErrorProtectionTriggerTime && !_isHolding)
            {
                _stageElapsedTime = 0;
                GameSceneManager.Instance.ShowScene(SceneNames.CalibrationErrorProtection);
                Pause();
                return;
            }

            var spacecraftScreenPos = GetSpacecraftScreenPos();
            var spacecraftSpeed = (_spacecraftScreenPos - spacecraftScreenPos).sqrMagnitude / Time.deltaTime;
            _spacecraftScreenPos = spacecraftScreenPos;
            _spacecraftController.SetMoving(spacecraftSpeed > .1f);

            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(spacecraftScreenPos);
            _spacecraftController.SetWorldPos(new Vector3(spacecraftWorldPos.x, spacecraftWorldPos.y, _spacecraftController.GetWorldPos().z));

            var currentStageData = _stageDataList[_stageIndex];
            var currentStage = currentStageData.Stage;
            var isHolding = _idleDuration >= currentStageData.HoldingPreparationTime &&
                Time.time >= _nextCanHoldTime;
            var isInterrupted = _isHolding && (!IsIdle() || Vector3.Magnitude(_holdingStartScreenPos - GetSpacecraftScreenPos()) > CalibrationConfig.GetData().HoldingMaxScreenOffset);
            if (isInterrupted)
            {
                isHolding = false;
                _nextCanHoldTime = Time.time + currentStageData.HoldingPreparationTime; // holding interrupted, need cooldown
                _marker.OnInterrupted();
            }

            if (!_isHolding && isHolding)
            {
                StartHolding();
            }
            else if (_isHolding && !isHolding)
            {
                StopHolding();
            }
            _isHolding = isHolding;

            // arrow
            if (currentStage != CalibrationStageType.TopLeft &&
                currentStage != CalibrationStageType.TopRight &&
                currentStage != CalibrationStageType.BottomRight &&
                currentStage != CalibrationStageType.BottomLeft ||
                _isHolding)
            {
                _arrow.gameObject.SetActive(false);
            }
            else
            {
                _arrow.gameObject.SetActive(true);

                var unitDirection = Vector3.zero;
                switch (currentStage)
                {
                    case CalibrationStageType.TopLeft:
                        unitDirection = new Vector2(-1, 1).normalized;
                        break;
                    case CalibrationStageType.TopRight:
                        unitDirection = new Vector2(1, 1).normalized;
                        break;
                    case CalibrationStageType.BottomRight:
                        unitDirection = new Vector2(1, -1).normalized;
                        break;
                    case CalibrationStageType.BottomLeft:
                        unitDirection = new Vector2(-1, -1).normalized;
                        break;
                    default:
                        Logger.LogError($"Invalid stage: {currentStage}");
                        break;
                }

                var worldDistance = CalibrationConfig.GetData().ArrowWorldDistanceFromCenter;
                var worldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(GetCenterScreenPos());
                var targetWorldPos = new Vector3(worldPos.x, worldPos.y) + worldDistance * unitDirection;
                _arrow.position = new Vector3(targetWorldPos.x, targetWorldPos.y, _arrow.position.z);

                var lookAtWorldPos = new Vector3(worldPos.x, worldPos.y) + worldDistance * unitDirection * 2;
                _arrow.LookAt(lookAtWorldPos, Vector3.forward);
            }

            if (_isHolding)
            {
                _holdingTime += Time.deltaTime;

                var heldCircleCount = (int)(_holdingTime / currentStageData.EachHoldingCheckDuration);
                var heldCircleFill = (_holdingTime % currentStageData.EachHoldingCheckDuration) / currentStageData.EachHoldingCheckDuration;
                _marker.SetCircleFill(heldCircleCount + 1, heldCircleFill);

                if (heldCircleCount >= currentStageData.MaxHoldingCheckCount)
                {
                    OnStageCompleted();
                }
            }

            return;
        }
        #endregion
    }
}