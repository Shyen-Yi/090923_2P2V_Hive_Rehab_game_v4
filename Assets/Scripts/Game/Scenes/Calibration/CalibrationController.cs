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
        private Vector3 _spacecraftOffsetFromCursor;
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
            Spacecraft = 0,
            LineDrawer = 1,
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

        private HiveButton _crossButton;
        private HiveButton _redoButton;
        private HiveButton _contactButton;
        private HiveButton _questionButton;

        private TMP_Text _instructionText;

        private Transform _spacecraft;
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

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _redoButton = Config.ExtraButtons[(int)ExtraBtn.Redo];
            _contactButton = Config.ExtraButtons[(int)ExtraBtn.Contact];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];

            _instructionText = Config.ExtraTextMeshPros[(int)ExtraTMP.Instruction];

            _spacecraft = Config.ExtraObjects[(int)ExtraObj.Spacecraft];
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
            base.OnShow(data, showState);

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

            InputManager.HideCursor();
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            base.OnHide(hideState);

            if (_status == CalibrationStatus.Running)
            {
                Pause();
            }

            InputManager.ShowCursor();
        }

        protected override void OnDispose()
        {
            UnbindActions();

            if (_endCoroutine != null)
            {
                MonoBehaviourUtil.Instance.StopCoroutine(_endCoroutine);
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
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _redoButton.onClick.RemoveAllListeners();
            _contactButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();

            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Content
        public void Restart()
        {
            Reset();
            Start();
        }

        private void Start()
        {
            UpdateStatus(CalibrationStatus.Running);
            _nextCanHoldTime = Time.time;
            _spacecraft.gameObject.SetActive(true);
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
            _instructionText.text = "";
            _raycastBlock.enabled = false;
            _spacecraftOffsetFromCursor = Vector3.zero;
            _holdingTime = 0;
            _stageElapsedTime = 0;

            foreach (var markContorller in _markDict.Values)
            {
                markContorller.Deactivate();
            }
            _marker.Deactivate();
        }

        private void End()
        {
            if (_endCoroutine != null)
            {
                MonoBehaviourUtil.Instance.StopCoroutine(_endCoroutine);
                _endCoroutine = null;
            }

            _endCoroutine = MonoBehaviourUtil.Instance.StartCoroutine(EndRoutine());
        }

        private void CenterSpacecraft()
        {
            _spacecraftOffsetFromCursor = new Vector3(Screen.width / 2, Screen.height / 2, 0) - InputManager.Instance.CursorScreenPos;
        }

        private Vector3 GetCenterScreenPos()
        {
            var topLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopLeft].position);
            var bottomRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomRight].position);
            return (topLeftScreenPos + bottomRightScreenPos) / 2;
        }

        private void OnStageCompleted()
        {
            var currentStage = _stageDataList[_stageIndex];
            _holdingTime = 0;
            _stageElapsedTime = 0;

            _nextCanHoldTime = Time.time + currentStage.CooldownTime;

            // mark
            if (_markDict.TryGetValue(currentStage.Stage, out var markController))
            {
                if (currentStage.Stage == CalibrationStageType.Center)
                {
                    // center of movement area (COULD BE the screen center)
                    var centerScreenPos = GetCenterScreenPos();
                    var targetWorldPos = CameraManager.Instance.UICamera.ScreenToWorldPoint(centerScreenPos);
                    markController.MoveToWorldPos(targetWorldPos);
                    CenterSpacecraft();
                }
                else
                {
                    var targetScreenPos = GetSpacecraftScreenPos();
                    var targetWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(targetScreenPos);
                    markController.MoveToWorldPos(targetWorldPos);
                }

                markController.Activate();
            }

            if (_stageIndex == _stageDataList.Count - 1) // all finished
            {
                End(); 
            }
            else
            {
                ++_stageIndex;
            }
        }

        private Vector3 GetSpacecraftScreenPos()
        {
            return InputManager.Instance.CursorScreenPos + _spacecraftOffsetFromCursor;
        }

        private IEnumerator EndRoutine()
        {
            UpdateStatus(CalibrationStatus.Ended);
            _spacecraft.gameObject.SetActive(false);
            _marker.Deactivate();
            _lineDrawer.Activate();
            _instructionText.text = "";
            _raycastBlock.enabled = true;

            var topLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopLeft].position);
            var topRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.TopRight].position);
            var bottomRightScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomRight].position);
            var bottomLeftScreenPos = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, _cornerDict[CalibrationStageType.BottomLeft].position);

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

            GameSceneManager.Instance.ShowScene(SceneNames.CalibrationEnding, null, ()=> 
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private bool IsCurrentPositionMatchingStage()
        {
            if (_stageIndex >= 0 && _stageIndex < _stageDataList.Count)
            {
                var currentStage = _stageDataList[_stageIndex];
                if (currentStage.Stage != CalibrationStageType.Center)
                {
                    var spacecraftScreenpos = GetSpacecraftScreenPos();
                    var centerScreenPos = GetCenterScreenPos();

                    switch (currentStage.Stage)
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
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.GoBack();
        }

        private void OnRedoButtonClick()
        {
            Reset();
            Start();
        }

        private void OnContactButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Calibration));
        }

        private void Tick()
        {
            if (_status != CalibrationStatus.Running)
                return;

            _stageElapsedTime += Time.deltaTime;
            if (_stageElapsedTime > CalibrationConfig.GetData().StageErrorProtectionTriggerTime)
            {
                _stageElapsedTime = 0;
                GameSceneManager.Instance.ShowScene(SceneNames.CalibrationErrorProtection);
                Pause();
                return;
            }

            var spacecraftWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(GetSpacecraftScreenPos());
            _spacecraft.position = new Vector3(spacecraftWorldPos.x, spacecraftWorldPos.y, _spacecraft.position.z);

            var currentStageData = _stageDataList[_stageIndex];
            var currentStage = currentStageData.Stage;
            var isHolding = InputManager.Instance.IdleDuration >= currentStageData.HoldingPreparationTime &&
                IsCurrentPositionMatchingStage();

            // arrow
            if (currentStage != CalibrationStageType.TopLeft &&
                currentStage != CalibrationStageType.TopRight &&
                currentStage != CalibrationStageType.BottomRight &&
                currentStage != CalibrationStageType.BottomLeft ||
                isHolding)
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

            if (Time.time >= _nextCanHoldTime && isHolding)
            {
                _marker.Activate();
                _spacecraft.gameObject.SetActive(false);
                _marker.SetWorldPosition(spacecraftWorldPos);
                _holdingTime += Time.deltaTime;
                _instructionText.text = currentStageData.InstructionWhenHolding;

                var heldCircleCount = (int)(_holdingTime / currentStageData.EachHoldingCheckDuration);
                var heldCircleFill = (_holdingTime % currentStageData.EachHoldingCheckDuration) / currentStageData.EachHoldingCheckDuration;

                _marker.SetCircleFill(heldCircleCount + 1, heldCircleFill);

                if (heldCircleCount >= currentStageData.MaxHoldingCheckCount)
                {
                    OnStageCompleted();
                }
            }
            else
            {
                _marker.Deactivate();
                _spacecraft.gameObject.SetActive(true);
                _holdingTime = 0;
                _instructionText.text = currentStageData.Instruction;
            }

            return;
        }
        #endregion
    }
}