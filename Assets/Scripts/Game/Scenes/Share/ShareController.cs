using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hive.projectr
{
    public class ShareController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraTMP
        {
            Content = 0,
        }

        private enum ExtraBtn
        {
            Cross = 0,
            Share = 1,
        }

        private TMP_Text _contentText;

        private HiveButton _crossButton;
        private HiveButton _shareButton;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _contentText = Config.ExtraTextMeshPros[(int)ExtraTMP.Content];

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _shareButton = Config.ExtraButtons[(int)ExtraBtn.Share];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is FeatureInfoData pData)
            {
                var infoDataList = FeatureInfoConfig.GetDataForFeature(pData.type);
                if (infoDataList.Count > 0)
                {
                    var index = Random.Range(0, infoDataList.Count);
                    var infoData = infoDataList[index];
                    _contentText.text = infoData.Desc;
                }
                else
                {
                    Logger.LogError($"FeatureInfoType {pData.type} has no defined data in FeatureInfoConfigData!");
                }
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
            _shareButton.onClick.AddListener(OnShareButtonClick);
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _shareButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.GoBack();
        }

        private void OnShareButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            GameSceneManager.Instance.ShowScene(SceneNames.ShareMenu, null, ()=>
            {
                GameSceneManager.Instance.HideScene(SceneName);
            });
        }
        #endregion
    }
}