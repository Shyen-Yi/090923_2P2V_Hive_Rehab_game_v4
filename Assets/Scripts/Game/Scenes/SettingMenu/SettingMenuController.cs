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
        private string _goalInputCache = string.Empty;
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
            var name = SettingManager.Instance.DisplayName;
            var level = SettingManager.Instance.Level;
            var goal = SettingManager.Instance.DailyBlock;

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
            _goalInputField.onSelect.AddListener(OnGoalSelect);
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
            _goalInputField.onSelect.RemoveAllListeners();
            _goalInputField.onEndEdit.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.GoBack();
        }

        private void OnMailButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            GameSceneManager.Instance.ShowScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Setting));
        }

        private void OnLevelUpButtonClick()
        {
            var level = Mathf.Clamp(_level + 1, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);
            RefreshLevel(level);
        }

        private void OnLevelDownButtonClick()
        {
            var level = Mathf.Clamp(_level - 1, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);
            RefreshLevel(level);
        }

        private void OnNameEndEdit(string displayName)
        {
            SettingManager.Instance.UpdateDisplayName(displayName, true);
        }

        private void OnGoalSelect(string txt)
        {
            _goalInputCache = txt;
        }

        private void OnGoalEndEdit(string txt)
        {
            if (int.TryParse(txt, out var dailyBlock))
            {
                dailyBlock = Mathf.Clamp(dailyBlock, GameGeneralConfig.GetData().MinGoal, GameGeneralConfig.GetData().MaxGoal);

                SettingManager.Instance.UpdateDailyBlock(dailyBlock, true);

                _goalInputField.text = $"{dailyBlock}";
            }
            else
            {
                _goalInputField.text = _goalInputCache;
            }

            _goalInputCache = null;
        }

        private void OnLevelUpdate(int level)
        {
            SettingManager.Instance.UpdateLevel(level, true);
        }
        #endregion

        #region Content
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

                OnLevelUpdate(level);
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
}