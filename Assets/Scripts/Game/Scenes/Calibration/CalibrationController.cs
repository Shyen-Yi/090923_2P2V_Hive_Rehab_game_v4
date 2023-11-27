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
        private Vector2[] calibrationCorners = new Vector2[4]; // To store the calibrated corners
        private int calibrationCornerIndex = 0;
        private bool isCalibrating = false;
        private float calibrationTime = 3f; // Time required to hold a corner for calibration
        private float calibrationTimer = 0f;
        private bool isCalibrated = false;

        private int _stageIndex;
        private CalibrationStatus _status;
        private List<CalibrationStageConfigData> _stageDataList;
        private Dictionary<CalibrationStageType, CalibrationStageResult> _resultDict = new Dictionary<CalibrationStageType, CalibrationStageResult>();
        private Coroutine _endCoroutine;
        private float _holdingTime;
        private float _nextCanHoldTime;
        #endregion

        #region Extra
        private enum ExtraConfig
        {
            TopLeftMark = 0,
            TopRightMark = 1,
            BottomRightMark = 2,
            BottomLeftMark = 3,
            Marker = 4,
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
            _markDict = new Dictionary<CalibrationStageType, CalibrationMarkController>() {
                { CalibrationStageType.TopLeft, new CalibrationMarkController(topLeftMarkConfig) },
                { CalibrationStageType.TopRight, new CalibrationMarkController(topRightMarkConfig) },
                { CalibrationStageType.BottomRight, new CalibrationMarkController(bottomRightMarkConfig) },
                { CalibrationStageType.BottomLeft, new CalibrationMarkController(bottomLeftMarkConfig) },
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

            _raycastBlock = Config.ExtraImages[(int)ExtraImg.RaycastBlock];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            base.OnShow(data, showState);

            if (_status == CalibrationStatus.Paused)
            {
                Resume();
            }
            else
            {
                Start();
            }
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            base.OnHide(hideState);

            if (_status == CalibrationStatus.Running)
            {
                Pause();
            }
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
        private void Start()
        {
            _status = CalibrationStatus.Running;
            _nextCanHoldTime = Time.time;
            _spacecraft.gameObject.SetActive(true);

            InputManager.Instance.Recenter();
            RefreshStage();
        }
        
        private void Resume()
        {
            _status = CalibrationStatus.Running;
            _nextCanHoldTime = Time.time;
        }

        private void Pause()
        {
            _status = CalibrationStatus.Paused;
        }

        private void Reset()
        {
            _resultDict.Clear();
            _stageIndex = 0;
            _status = CalibrationStatus.NotStarted;
            _instructionText.text = "";
            _raycastBlock.enabled = false;

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

        private void OnStageCompleted()
        {
            var currentStage = _stageDataList[_stageIndex];

            var cooldown = currentStage.CooldownTime;
            _nextCanHoldTime = Time.time + cooldown;
            _holdingTime = 0;

            // mark
            if (_markDict.TryGetValue(currentStage.Stage, out var markController))
            {
                var inputScreenPos = InputManager.Instance.CurrentScreenPos;
                var inputWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(inputScreenPos);

                markController.MoveToWorldPos(inputWorldPos);
                markController.Activate();
            }

            if (_stageIndex == _stageDataList.Count - 1) // all finished
            {
                End(); 
            }
            else
            {
                ++_stageIndex;
                RefreshStage();
            }
        }

        private IEnumerator EndRoutine()
        {
            _status = CalibrationStatus.Ended;
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

            var firstStage = _stageDataList[0].Stage;
            var firstPoint = _markDict[firstStage].GetCurrentWorldPos();
            var points = new List<Vector3>() { firstPoint };
            var reachedIndex = 0;
            var reachedCount = 0;
            var targetIndex = (reachedIndex + 1) % _stageDataList.Count;
            var pointMovementDuration = .3f;
            var pointMovementProgress = 0f;
            while (reachedCount < _stageDataList.Count)
            {
                if (points.Count < reachedCount + 2)
                {
                    points.Add(Vector3.zero);
                }

                var startingStage = _stageDataList[reachedIndex].Stage;
                var targetStage = _stageDataList[targetIndex].Stage;
                var startingWorldPos = _markDict[startingStage].GetCurrentWorldPos();
                var targetWorldPos = _markDict[targetStage].GetCurrentWorldPos();

                pointMovementProgress += 1 / pointMovementDuration * Time.deltaTime;
                points[points.Count - 1] = Vector3.Lerp(startingWorldPos, targetWorldPos, pointMovementProgress);
                _lineDrawer.Draw(points.ToArray());

                if (Vector3.SqrMagnitude(points[points.Count - 1] - targetWorldPos) < .001f) // reached target
                {
                    ++reachedIndex;
                    ++reachedCount;
                    targetIndex = (reachedIndex + 1) % _stageDataList.Count;

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

            GameSceneManager.Instance.LoadScene(SceneNames.TransitionCoreGame, null, () =>
            {
                GameSceneManager.Instance.UnloadScene(SceneName);
            });
        }

        private void RefreshStage()
        {
            if (_stageIndex >= 0 && _stageIndex < _stageDataList.Count)
            {
                var stageData = _stageDataList[_stageIndex];
                _instructionText.text = stageData.Instruction;
            }
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(SceneName);
        }

        private void OnRedoButtonClick()
        {
            Reset();
            Start();
        }

        private void OnContactButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Calibration));
        }

        private void Tick()
        {
            if (_status != CalibrationStatus.Running)
                return;

            var inputScreenPos = InputManager.Instance.CurrentScreenPos;
            var inputWorldPos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(inputScreenPos);

            _spacecraft.position = new Vector3(inputWorldPos.x, inputWorldPos.y, _spacecraft.position.z);

            if (Time.time >= _nextCanHoldTime &&
                InputManager.GetMouseButton(1)) // kun todo - replace with "holding + staying still" behaviour
            {
                _marker.Activate();
                _spacecraft.gameObject.SetActive(false);
                _marker.SetWorldPosition(inputWorldPos);
                _holdingTime += Time.deltaTime;

                var currentStageData = _stageDataList[_stageIndex];
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
            }

            return;


            //if (!isCalibrated)
            //{
            //    if (isCalibrating)
            //    {
            //        calibrationTimer += Time.deltaTime;
            //        if (calibrationTimer >= calibrationTime)
            //        {
            //            calibrationCorners[calibrationCornerIndex] = Input.mousePosition;
            //            calibrationCornerIndex++;
            //            if (calibrationCornerIndex >= 4)
            //            {
            //                // All corners are calibrated
            //                isCalibrated = true;
            //                isCalibrating = false;
            //            }
            //            else
            //            {
            //                calibrationTimer = 0f;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Check if the player wants to calibrate
            //        if (Input.GetMouseButtonDown(0))
            //        {
            //            isCalibrating = true;
            //            calibrationTimer = 0f;
            //        }
            //    }
            //}
            //else
            //{
            //    // Calculate the calibrated screen dimensions
            //    float minX = Mathf.Min(calibrationCorners[0].x, calibrationCorners[2].x);
            //    float minY = Mathf.Min(calibrationCorners[1].y, calibrationCorners[3].y);
            //    float maxX = Mathf.Max(calibrationCorners[0].x, calibrationCorners[2].x);
            //    float maxY = Mathf.Max(calibrationCorners[1].y, calibrationCorners[3].y);

            //    // Apply these dimensions to determine the allowed gameplay area
            //    Rect allowedArea = new Rect(minX, minY, maxX - minX, maxY - minY);

            //    // Check if the mouse is within the allowed gameplay area
            //    Vector2 mousePosition = Input.mousePosition;
            //    if (allowedArea.Contains(mousePosition))
            //    {
            //        // You can perform mouse-marking logic here if needed
            //        if (Input.GetMouseButtonDown(0))
            //        {
            //            // Instantiate a mark at the mouse position
            //            GameObject.Instantiate(_markPrefab, mousePosition, Quaternion.identity);
            //        }
            //    }
            //}
        }
        #endregion
    }
}