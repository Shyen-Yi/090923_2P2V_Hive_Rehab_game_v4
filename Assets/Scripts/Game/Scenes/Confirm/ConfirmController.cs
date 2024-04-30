using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.hive.projectr
{
    public struct ConfirmData : ISceneData
    {
        public string title;
        public string content;
        public string confirm;
        public Action onConfirm;
        public Action onNotConfirm;

        public ConfirmData(string title, string content, string confirm, Action onConfirm, Action onNotConfirm)
        {
            this.title = title;
            this.content = content;
            this.confirm = confirm;
            this.onConfirm = onConfirm;
            this.onNotConfirm = onNotConfirm;
        }
    }

    public class ConfirmController : GameSceneControllerBase
    {
        #region Fields
        private Action _onConfirm;
        private Action _onNotConfirm;
        private bool _isConfirm;
        #endregion

        #region Extra
        private enum ExtraBtn
        {
            Confirm = 0,
            Cross = 1,
        }

        private enum ExtraTMP
        {
            Title = 0,
            Content = 1,
            Confirm = 2,
        }

        private HiveButton _confirmButton;
        private HiveButton _crossButton;

        private TMP_Text _titleText;
        private TMP_Text _contentText;
        private TMP_Text _confirmText;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _confirmButton = Config.ExtraButtons[(int)ExtraBtn.Confirm];
            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];

            _titleText = Config.ExtraTextMeshPros[(int)ExtraTMP.Title];
            _contentText = Config.ExtraTextMeshPros[(int)ExtraTMP.Content];
            _confirmText = Config.ExtraTextMeshPros[(int)ExtraTMP.Confirm];
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            if (data is ConfirmData pData)
            {
                _onConfirm = pData.onConfirm;
                _onNotConfirm = pData.onNotConfirm;

                _titleText.text = pData.title;
                _contentText.text = pData.content;
                _confirmText.text = pData.confirm;
            }
        }

        protected override void OnHide(GameSceneHideState hideState)
        {
            if (hideState == GameSceneHideState.Removed)
            {
                if (_isConfirm)
                {
                    _onConfirm?.Invoke();
                }
                else
                {
                    _onNotConfirm?.Invoke();
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
            _confirmButton.onClick.AddListener(OnConfirmButtonClick);
            _crossButton.onClick.AddListener(OnCrossButtonClick);
        }

        private void UnbindActions()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _crossButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Event
        private void OnConfirmButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            _isConfirm = true;
            GameSceneManager.Instance.GoBack();
        }

        private void OnCrossButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            _isConfirm = false;
            GameSceneManager.Instance.GoBack();
        }
        #endregion
    }
}