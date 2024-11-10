using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.hive.projectr
{
    public class SettingMenuSettingsWidgetController
    {
        private GeneralWidgetConfig _config;

        #region Fields
        private int _level = -1;
        private int _year = -1;
        private int _month = -1;
        private int _goal = -1;
        private string _goalInputCache = string.Empty;
        private string _name = string.Empty;
        #endregion

        #region Extra
        private enum ExtraTMP
        {
            CalendarDate = 0,
            Level = 1,
        }

        private enum ExtraBtn
        {
            LevelUp = 0,
            LevelDown = 1,
            Reset = 2,
        }

        private enum ExtraObj
        {
            NameInputField = 0,
            GoalInputField = 1,
        }

        private TMP_Text _calendarDateText;
        private TMP_Text _levelText;

        private HiveButton _levelUpButton;
        private HiveButton _levelDownButton;
        private HiveButton _resetButton;

        private TMP_InputField _nameInputField;
        private TMP_InputField _goalInputField;
        #endregion

        #region Lifecycle
        public SettingMenuSettingsWidgetController(GeneralWidgetConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _calendarDateText = _config.ExtraTextMeshPros[(int)ExtraTMP.CalendarDate];
            _levelText = _config.ExtraTextMeshPros[(int)ExtraTMP.Level];

            _levelUpButton = _config.ExtraButtons[(int)ExtraBtn.LevelUp];
            _levelDownButton = _config.ExtraButtons[(int)ExtraBtn.LevelDown];
            _resetButton = _config.ExtraButtons[(int)ExtraBtn.Reset];

            _nameInputField = _config.ExtraObjects[(int)ExtraObj.NameInputField].GetComponent<TMP_InputField>();
            _goalInputField = _config.ExtraObjects[(int)ExtraObj.GoalInputField].GetComponent<TMP_InputField>();
        }

        public void Show()
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

        #region UI Binding
        private void BindActions()
        {
            _levelUpButton.onClick.AddListener(OnLevelUpButtonClick);
            _levelDownButton.onClick.AddListener(OnLevelDownButtonClick);
            _resetButton.onClick.AddListener(OnResetButtonClick);

            _nameInputField.onValueChanged.AddListener(OnNameChanged);
            _goalInputField.onSelect.AddListener(OnGoalSelect);
            _goalInputField.onEndEdit.AddListener(OnGoalEndEdit);
        }

        private void UnbindActions()
        {
            _levelUpButton.onClick.RemoveAllListeners();
            _levelDownButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();

            _nameInputField.onValueChanged.RemoveAllListeners();
            _goalInputField.onSelect.RemoveAllListeners();
            _goalInputField.onEndEdit.RemoveAllListeners();
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
            _name = name;
            _nameInputField.text = _name;
        }

        private void RefreshLevel(int level)
        {
            _level = level;
            _levelText.text = $"{_level}";
        }

        private void RefreshGoal(int goal)
        {
            _goal = goal;
            _goalInputField.text = $"{_goal}";
        }

        public void SaveSettings()
        {
            SettingManager.Instance.UpdateDisplayName(_name, true);
            SettingManager.Instance.UpdateLevel(_level, true);
            SettingManager.Instance.UpdateDailyBlock(_goal, true);
        }
        #endregion

        #region Event
        private void OnLevelUpButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            var level = Mathf.Clamp(_level + 1, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);
            RefreshLevel(level);
        }

        private void OnLevelDownButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            var level = Mathf.Clamp(_level - 1, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);
            RefreshLevel(level);
        }

        private void OnResetButtonClick()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            SettingManager.Instance.Reset();
            Show();
        }

        private void OnNameChanged(string displayName)
        {
            RefreshName(displayName);
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

                RefreshGoal(dailyBlock);
            }
            else
            {
                _goalInputField.text = _goalInputCache;
            }

            _goalInputCache = null;
        }
        #endregion
    }
}