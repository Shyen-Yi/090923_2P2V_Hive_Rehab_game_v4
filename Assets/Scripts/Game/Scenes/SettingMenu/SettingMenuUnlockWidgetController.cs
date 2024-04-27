using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public class SettingMenuUnlockWidgetController
    {
        #region Fields
        private GeneralWidgetConfig _config;
        private string _username;
        private string _password;
        #endregion

        #region Extra
        private enum ExtraCG
        {
            Notice = 0,
        }

        private enum ExtraTMP
        {
            Notice = 0,
        }

        private enum ExtraObj
        {
            Username = 0,
            Password = 1,
        }

        private CanvasGroup _noticeCanvasGroup;

        private TMP_Text _noticeText;

        private TMP_InputField _usernameInputField;
        private TMP_InputField _passwordInputField;
        #endregion

        #region Lifecycle
        public SettingMenuUnlockWidgetController(GeneralWidgetConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            InitExtra();
            BindActions();
        }

        public void Show()
        {
            Reset();
            RefreshNotice("Unlock the Settings with authorized user information.");

            _config.CanvasGroup.CanvasGroupOn();
        }

        public void Hide()
        {
            _config.CanvasGroup.CanvasGroupOff();
        }

        public void Dispose()
        {
            UnbindActions();
        }
        #endregion

        #region Content
        public void Reset()
        {
            _usernameInputField.text = "";
            _passwordInputField.text = "";
        }

        private void InitExtra()
        {
            _noticeCanvasGroup = _config.ExtraCanvasGroups[(int)ExtraCG.Notice];

            _noticeText = _config.ExtraTextMeshPros[(int)ExtraTMP.Notice];

            _usernameInputField = _config.ExtraObjects[(int)ExtraObj.Username].GetComponent<TMP_InputField>();
            _passwordInputField = _config.ExtraObjects[(int)ExtraObj.Password].GetComponent<TMP_InputField>();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _usernameInputField.onValueChanged.AddListener(OnUsernameValueChanged);
            _passwordInputField.onValueChanged.AddListener(OnPasswordValueChanged);
        }

        private void UnbindActions()
        {
            _usernameInputField.onValueChanged.RemoveAllListeners();
            _passwordInputField.onValueChanged.RemoveAllListeners();
        }
        #endregion

        #region Event
        private void OnUsernameValueChanged(string username)
        {
            RefreshNotice("Unlock the Settings with authorized user information.");
        }

        private void OnPasswordValueChanged(string password)
        {
            RefreshNotice("Unlock the Settings with authorized user information.");
        }
        #endregion

        #region Content
        public bool TryAuthenticate()
        {
            var username = _usernameInputField.text;
            if (string.IsNullOrEmpty(username))
            {
                RefreshNotice("Username cannot be empty.");
                return false;
            }

            var password = _passwordInputField.text;
            if (string.IsNullOrEmpty(password))
            {
                RefreshNotice("Password cannot be empty.");
                return false;
            }

            if (username.Equals(GameGeneralConfig.GetData().AdminUsername) &&
                password.Equals(GameGeneralConfig.GetData().AdminPassword))
            {
                return true;
            }

            RefreshNotice("Invalid username or password.");

            return false;
        }

        private void RefreshNotice(string notice)
        {
            if (string.IsNullOrEmpty(notice))
            {
                _noticeCanvasGroup.CanvasGroupOff();
            }
            else
            {
                _noticeCanvasGroup.CanvasGroupOn();
                _noticeText.text = notice;
            }
        }
        #endregion
    }
}