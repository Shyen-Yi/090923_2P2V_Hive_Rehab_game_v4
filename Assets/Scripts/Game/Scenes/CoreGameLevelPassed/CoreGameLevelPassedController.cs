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
            Play = 1,
            NextLevel = 2,
            Replay = 3,
        }

        private enum ExtraTMP
        {
            Content = 0,
        }

        private HiveButton _exitButton;
        private HiveButton _playButton;
        private HiveButton _nextLevelButton;
        private HiveButton _replayButton;

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
            _playButton = Config.ExtraButtons[(int)ExtraBtn.Play];
            _nextLevelButton = Config.ExtraButtons[(int)ExtraBtn.NextLevel];
            _replayButton = Config.ExtraButtons[(int)ExtraBtn.Replay];

            _contentText = Config.ExtraTextMeshPros[(int)ExtraTMP.Content];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is CoreGameLevelPassedData pData)
            {
                _coreGameData = pData.coreGameData;
            }

            if (_coreGameData.level < CoreGameLevelConfig.MaxLevel)
            {
                _replayButton.gameObject.SetActive(false);

                if (LevelManager.Instance.LatestLevelPassedStreak < GameGeneralConfig.GetData().PassingStreakToNextLevel)
                {
                    _playButton.gameObject.SetActive(true);
                    _nextLevelButton.gameObject.SetActive(false);
                }
                else
                {
                    // next level!
                    _playButton.gameObject.SetActive(false);
                    _nextLevelButton.gameObject.SetActive(true);

                    SoundManager.Instance.PlaySound(SoundType.LevelUp);
                }
            }
            else
            {
                _replayButton.gameObject.SetActive(true);
                _playButton.gameObject.SetActive(false);
                _nextLevelButton.gameObject.SetActive(false);
            }

            var feautureInfoList = FeatureInfoConfig.GetDataForFeature(FeatureType.CoreGameLevelPassed);
            if (feautureInfoList.Count > 0)
            {
                _contentText.text = string.Format(feautureInfoList[0].Desc, _coreGameData.level);
            }
            else
            {
                Logger.LogError($"No defined feature info data for {FeatureType.CoreGameLevelPassed}");
            }

            SoundManager.Instance.PlaySound(SoundType.CoreGameLevelEndShowReport);
            SoundManager.Instance.PlaySound(SoundType.MenuBackground);
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            SoundManager.Instance.StopSound(SoundType.MenuBackground);
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
            _playButton.onClick.AddListener(OnPlayButtonClick);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
            _replayButton.onClick.AddListener(OnReplayButtonClick);
        }
        
        private void UnbindActions()
        {
            _exitButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
            _nextLevelButton.onClick.RemoveAllListeners();
            _replayButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnExitButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.GoBack(SceneNames.MainMenu);
        }

        private void OnPlayButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, _coreGameData, () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private void OnReplayButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, _coreGameData, ()=>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }

        private void OnNextLevelButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.CoreGame, new CoreGameData(_coreGameData.bottomLeftScreenPos, _coreGameData.topRightScreenPos, _coreGameData.centerScreenPos, _coreGameData.spacecraftMovementScale, _coreGameData.level + 1), () =>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }
        #endregion
    }
}