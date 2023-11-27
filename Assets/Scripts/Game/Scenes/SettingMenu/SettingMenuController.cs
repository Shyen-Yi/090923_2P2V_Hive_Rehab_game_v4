using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

namespace com.hive.projectr
{
    public class SettingMenuController : GameSceneControllerBase
    {
        #region Fields
        private int _level = -1;
        private int _year = -1;
        private int _month = -1;
        private int _goal = -1;
        private string _name = string.Empty;
        #endregion

        #region Extra
        private enum ExtraBtn
        {
            Cross = 0,
            Mail = 1,
            Question = 2,
            LevelUp = 3,
            LevelDown = 4,
        }

        private enum ExtraTMP
        {
            CalendarDate = 0,
            Level = 1,
        }

        private enum ExtraObj
        {
            NameInputField = 0,
            GoalInputField = 1,
        }

        private HiveButton _crossButton;
        private HiveButton _mailButton;
        private HiveButton _questionButton;
        private HiveButton _levelUpButton;
        private HiveButton _levelDownButton;

        private TMP_Text _calendarDateText;
        private TMP_Text _levelText;

        private TMP_InputField _nameInputField;
        private TMP_InputField _goalInputField;
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
            _mailButton = Config.ExtraButtons[(int)ExtraBtn.Mail];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];
            _levelUpButton = Config.ExtraButtons[(int)ExtraBtn.LevelUp];
            _levelDownButton = Config.ExtraButtons[(int)ExtraBtn.LevelDown];

            _calendarDateText = Config.ExtraTextMeshPros[(int)ExtraTMP.CalendarDate];
            _levelText = Config.ExtraTextMeshPros[(int)ExtraTMP.Level];

            _nameInputField = Config.ExtraObjects[(int)ExtraObj.NameInputField].GetComponent<TMP_InputField>();
            _goalInputField = Config.ExtraObjects[(int)ExtraObj.GoalInputField].GetComponent<TMP_InputField>();
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var name = "";
            var level = GameGeneralConfig.GetData().DefaultLevel;
            var goal = GameGeneralConfig.GetData().DefaultGoal;

            if (PlayerPrefs.HasKey(PlayerPrefsKey.SettingMenuStorage))
            {
                var storage = JsonConvert.DeserializeObject<SettingMenuStorage>(PlayerPrefs.GetString(PlayerPrefsKey.SettingMenuStorage));
                if (storage != null)
                {
                    name = storage.name;
                    level = storage.level;
                    goal = storage.goal;
                }
            }

            // calendar date
            RefreshCalendarDate(year, month);

            // name
            RefreshName(name);

            // level
            RefreshLevel(level);

            // goal
            RefreshGoal(goal);
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
            _mailButton.onClick.AddListener(OnMailButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);
            _levelUpButton.onClick.AddListener(OnLevelUpButtonClick);
            _levelDownButton.onClick.AddListener(OnLevelDownButtonClick);

            _nameInputField.onEndEdit.AddListener(OnNameEndEdit);
            _goalInputField.onEndEdit.AddListener(OnGoalEndEdit);
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
            _mailButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();
            _levelUpButton.onClick.RemoveAllListeners();
            _levelDownButton.onClick.RemoveAllListeners();

            _nameInputField.onEndEdit.RemoveAllListeners();
            _goalInputField.onEndEdit.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(SceneName);
        }

        private void OnMailButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Setting));
        }

        private void OnLevelUpButtonClick()
        {
            var level = Mathf.Clamp(_level + 1, GameGeneralConfig.GetData().MinLevel, GameGeneralConfig.GetData().MaxLevel);
            RefreshLevel(level);
        }

        private void OnLevelDownButtonClick()
        {
            var level = Mathf.Clamp(_level - 1, GameGeneralConfig.GetData().MinLevel, GameGeneralConfig.GetData().MaxLevel);
            RefreshLevel(level);
        }

        private void OnNameEndEdit(string txt)
        {
            SaveStorage();
        }

        private void OnGoalEndEdit(string txt)
        {
            SaveStorage();
        }

        private void OnLevelUpdate()
        {
            SaveStorage();
        }
        #endregion

        #region Content
        private void SaveStorage()
        {
            var storage = new SettingMenuStorage(_name, _level, _goal);
            PlayerPrefs.SetString(PlayerPrefsKey.SettingMenuStorage, JsonConvert.SerializeObject(storage));
        }

        private void RefreshCalendarDate(int year, int month)
        {
            if (_year != year || _month != month)
            {
                _year = year;
                _month = month;

                var date = new DateTime(_year, _month, 1);
                _calendarDateText.text = date.ToString("MMMM, yyyy");
            }
        }

        private void RefreshName(string name)
        {
            if (_name == null || !_name.Equals(name))
            {
                _name = name;
                _nameInputField.text = _name;
            }
        }

        private void RefreshLevel(int level)
        {
            if (_level != level)
            {
                _level = level;
                _levelText.text = $"{_level}";

                OnLevelUpdate();
            }
        }

        private void RefreshGoal(int goal)
        {
            if (_goal != goal)
            {
                _goal = goal;
                _goalInputField.text = $"{_goal}";
            }
        }
        #endregion
    }

    #region Storage
    public class SettingMenuStorage
    {
        public string name;
        public int level;
        public int goal;

        public SettingMenuStorage()
        {
            name = "";
            level = 1;
            goal = 1;
        }

        public SettingMenuStorage(string name, int level, int goal)
        {
            this.name = name;
            this.level = level;
            this.goal = goal;
        }
    }
    #endregion
}