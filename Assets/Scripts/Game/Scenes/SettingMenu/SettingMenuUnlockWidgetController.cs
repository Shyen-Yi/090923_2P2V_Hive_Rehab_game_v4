using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public class SettingUserInfoStorage
    {
        public Dictionary<string, string> dict;

        //public SettingUserInfoStorage(string username, string password)
        //{
        //    dict = new Dictionary<string, string>();
        //    dict.Add(username, password);
        //}
    }

    public class SettingMenuUnlockWidgetController
    {
        #region Fields
        private GeneralWidgetConfig _config;
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
            RefreshNotice("");

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
            RefreshNotice("");
        }

        private void OnPasswordValueChanged(string password)
        {
            RefreshNotice("");
        }
        #endregion

        #region Content
        public bool TryAuthenticate()
        {
            var username = _usernameInputField.text;
            if (string.IsNullOrEmpty(username))
            {
                RefreshNotice("Invalid username");
                return false;
            }

            var password = _passwordInputField.text;
            if (string.IsNullOrEmpty(password))
            {
                RefreshNotice("Invalid password");
                return false;
            }

            if (PlayerPrefs.HasKey(PlayerPrefKeys.UserInfoStorage))
            {
                var prevUserInfoStorageJson = PlayerPrefs.GetString(PlayerPrefKeys.UserInfoStorage);
                var prevUserInfoStorage = JsonConvert.DeserializeObject<SettingUserInfoStorage>(prevUserInfoStorageJson);

                if (prevUserInfoStorage != null && prevUserInfoStorage.dict != null)
                {
                    // check
                    if (prevUserInfoStorage.dict.TryGetValue(username, out var prevPassword))
                    {
                        if (prevPassword.Equals(password))
                        {
                            RefreshNotice("");
                            return true;
                        }
                        else
                        {
                            RefreshNotice("Wrong password!");
                            return true;
                        }
                    }
                    else
                    {
                        RefreshNotice("Username not found!");
                        return false;
                    }
                }
                else
                {
                    RefreshNotice("Found user info storage. Parse failed.");
                    return false;
                }
            }
            else
            {
                // save & pass
                var storage = new SettingUserInfoStorage() { 
                    dict = new Dictionary<string, string>()
                    {
                        { username, password }
                    }
                };
                var storageJson = JsonConvert.SerializeObject(storage);
                PlayerPrefs.SetString(PlayerPrefKeys.UserInfoStorage, storageJson);

                return true;
            }
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