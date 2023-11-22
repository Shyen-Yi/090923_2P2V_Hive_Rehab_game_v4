using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hive.projectr
{
    public enum FeatureType
    {
        Setting,
    }

    public struct FeatureInfoData : ISceneData
    {
        public FeatureType type;

        public FeatureInfoData(FeatureType type)
        {
            this.type = type;
        }
    }

    public class FeatureInfoController : GameSceneControllerBase
    {
        #region Extra
        private enum ExtraTMP
        {
            Content = 0,
        }

        private enum ExtraBtn
        {
            Cross = 0,
        }

        private TMP_Text _contentText;

        private HiveButton _crossButton;
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
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(Name);
        }
        #endregion
    }
}