using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hive.projectr
{
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
            Title = 1,
        }

        private enum ExtraBtn
        {
            Cross = 0,
        }

        private TMP_Text _contentText;
        private TMP_Text _titleText;

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
            _titleText = Config.ExtraTextMeshPros[(int)ExtraTMP.Title];

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
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
                    _titleText.text = infoData.Title;
                    _contentText.text = infoData.Desc;
                }
                else
                {
                    Logger.LogError($"FeatureInfoType {pData.type} has no defined data in FeatureInfoConfig!");
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
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(SceneName);
        }
        #endregion
    }
}