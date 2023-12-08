using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.hive.projectr
{
    public struct TransitionCoreGameData : ISceneData
    {
        public CoreGameData coreGameData;

        public TransitionCoreGameData(CoreGameData coreGameData)
        {
            this.coreGameData = coreGameData;
        }
    }

    public class TransitionCoreGameController : GameSceneControllerBase
    {
        #region Fields
        private float _progressFill;
        private float _maxProgressWidth;
        private CoreGameData _coreGameData;
        #endregion

        #region Extra
        private enum ExtraBtn
        {
            Cross = 0,
        }

        private enum ExtraImg
        {
            ProgressBar = 0,
        }

        private enum ExtraRT
        {
            SpaceshipRoot = 0,
        }

        private HiveButton _crossButton;

        private Image _progressBar;

        private RectTransform _spaceshipRoot;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];

            _progressBar = Config.ExtraImages[(int)ExtraImg.ProgressBar];

            _spaceshipRoot = Config.ExtraRectTransforms[(int)ExtraRT.SpaceshipRoot];

            _maxProgressWidth = ((RectTransform)_spaceshipRoot.parent).rect.width;
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is TransitionCoreGameData pData)
            {
                _coreGameData = pData.coreGameData;
            }
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _crossButton.onClick.AddListener(OnCrossButtonClick);

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();

            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.GoBack();
        }

        private void Tick()
        {
            if (_progressFill > -.5f)
            {
                if (_progressFill < 1f)
                {
                    // fill amount
                    var maxFillTime = GameGeneralConfig.GetData().CoreGameTransitionSec;
                    _progressBar.fillAmount = _progressFill = _progressFill + 1 / maxFillTime * Time.deltaTime;

                    // marker pos
                    var xPos = Mathf.Lerp(0, _maxProgressWidth, _progressFill);
                    _spaceshipRoot.anchoredPosition = new Vector2(xPos, _spaceshipRoot.anchoredPosition.y);
                }
                else
                {
                    _progressBar.fillAmount = _progressFill = 1;
                    _spaceshipRoot.anchoredPosition = new Vector2(_maxProgressWidth, _spaceshipRoot.anchoredPosition.y);

                    GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, _coreGameData, () =>
                    {
                        GameSceneManager.Instance.HideScene(SceneName);
                    });

                    // set to -1 to stop being affected by ticking
                    _progressFill = -1;
                }
            }
        }
        #endregion
    }
}