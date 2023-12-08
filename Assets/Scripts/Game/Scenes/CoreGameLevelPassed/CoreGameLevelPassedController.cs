using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hive.projectr
{
    public struct CoreGameLevelPassedData : ISceneData
    {
        public CoreGameData coreGameData;

        public CoreGameLevelPassedData(CoreGameData coreGameData)
        {
            this.coreGameData = coreGameData;
        }
    }

    public class CoreGameLevelPassedController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraBtn
        {
            Exit = 0,
            Replay = 1,
            NextLevel = 2,
        }

        private enum ExtraTMP
        {
            Content = 0,
        }

        private HiveButton _exitButton;
        private HiveButton _replayButton;
        private HiveButton _nextLevelButton;

        private TMP_Text _contentText;
        #endregion

        #region Fields
        private CoreGameData _coreGameData;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _exitButton = Config.ExtraButtons[(int)ExtraBtn.Exit];
            _replayButton = Config.ExtraButtons[(int)ExtraBtn.Replay];
            _nextLevelButton = Config.ExtraButtons[(int)ExtraBtn.NextLevel];

            _contentText = Config.ExtraTextMeshPros[(int)ExtraTMP.Content];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameLevelPassedData pData)
            {
                _coreGameData = pData.coreGameData;
            }

            _nextLevelButton.gameObject.SetActive(_coreGameData.level < CoreGameLevelConfig.MaxLevel);

            var feautureInfoList = FeatureInfoConfig.GetDataForFeature(FeatureType.CoreGameLevelPassed);
            if (feautureInfoList.Count > 0)
            {
                _contentText.text = string.Format(feautureInfoList[0].Desc, _coreGameData.level);
            }
            else
            {
                Logger.LogError($"No defined feature info data for {FeatureType.CoreGameLevelPassed}");
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
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _replayButton.onClick.AddListener(OnReplayButtonClick);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
        }
        
        private void UnbindActions()
        {
            _exitButton.onClick.RemoveAllListeners();
            _replayButton.onClick.RemoveAllListeners();
            _nextLevelButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnExitButtonClick()
        {
            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);
        }

        private void OnReplayButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, _coreGameData, ()=>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private void OnNextLevelButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, new CoreGameData(_coreGameData.bottomLeftScreenPos, _coreGameData.topRightScreenPos, _coreGameData.centerScreenPos, _coreGameData.spacecraftMovementScale, _coreGameData.level + 1), () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }
        #endregion
    }
}