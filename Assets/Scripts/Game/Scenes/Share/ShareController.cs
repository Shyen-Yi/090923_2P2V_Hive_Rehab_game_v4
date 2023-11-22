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

        protected override void OnShow(ISceneData data)
        {
            if (data is FeatureInfoData pData)
            {
                if (FeatureInfoConfig.FeatureInfoDict.TryGetValue(pData.type, out var info))
                {
                    _contentText.text = info.desc;
                }
                else
                {
                    LogHelper.LogError($"FeatureInfoType {pData.type} has no defined data in FeatureInfoConfigData!");
                }
            }
        }

        protected override void OnHide()
        {
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
            GameSceneManager.Instance.UnloadScene(Name);
        }

        private void OnShareButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.ShareMenu, null, ()=>
            {
                GameSceneManager.Instance.UnloadScene(Name);
            });
        }
        #endregion
    }
}