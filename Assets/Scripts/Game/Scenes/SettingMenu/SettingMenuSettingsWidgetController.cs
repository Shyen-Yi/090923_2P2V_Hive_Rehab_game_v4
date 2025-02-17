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
        private int _year = -1;
        private int _month = -1;
        private int _goal = -1;
        private string _goalInputCache = string.Empty;
        private int _total = -1;
        private string _totalInputCache = string.Empty;
        private int _currentLevel = -1;
        private string _currentLevelInputCache = string.Empty;
        private string _name = string.Empty;
        #endregion

        #region Extra
        private enum ExtraTMP
        {
            CalendarDate = 0,
        }

        private enum ExtraBtn
        {
            Reset = 0,
        }

        private enum ExtraObj
        {
            NameInputField = 0,
            GoalInputField = 1,
            TotalInputField = 2,
            CurrentLevelInputField = 3,
        }

        private TMP_Text _calendarDateText;

        private HiveButton _resetButton;

        private TMP_InputField _nameInputField;
        private TMP_InputField _goalInputField;
        private TMP_InputField _totalInputField;
        private TMP_InputField _currentLevelInputField;
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

            _resetButton = _config.ExtraButtons[(int)ExtraBtn.Reset];

            _nameInputField = _config.ExtraObjects[(int)ExtraObj.NameInputField].GetComponent<TMP_InputField>();
            _goalInputField = _config.ExtraObjects[(int)ExtraObj.GoalInputField].GetComponent<TMP_InputField>();
            _totalInputField = _config.ExtraObjects[(int)ExtraObj.TotalInputField].GetComponent<TMP_InputField>();
            _currentLevelInputField = _config.ExtraObjects[(int)ExtraObj.CurrentLevelInputField].GetComponent<TMP_InputField>();
        }

        public void Show()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var name = SettingManager.Instance.DisplayName;
            var levelGoal = SettingManager.Instance.LevelGoal;
            var levelTotal = SettingManager.Instance.LevelTotal;

            // calendar date
            RefreshCalendarDate(year, month);

            // name
            RefreshName(name);

            // total
            RefreshLevelTotal(levelTotal);

            // goal
            RefreshLevelGoal(levelGoal);

            // current level
            RefreshCurrentLevel(LevelManager.Instance.CurrentLevel);

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
            _resetButton.onClick.AddListener(OnResetButtonClick);

            _nameInputField.onValueChanged.AddListener(OnNameChanged);
            _goalInputField.onSelect.AddListener(OnGoalSelect);
            _goalInputField.onEndEdit.AddListener(OnGoalEndEdit);
            _totalInputField.onSelect.AddListener(OnTotalSelect);
            _totalInputField.onEndEdit.AddListener(OnTotalEndEdit);
            _currentLevelInputField.onSelect.AddListener(OnCurrentLevelSelect);
            _currentLevelInputField.onEndEdit.AddListener(OnCurrentLevelEndEdit);
        }

        private void UnbindActions()
        {
            _resetButton.onClick.RemoveAllListeners();

            _nameInputField.onValueChanged.RemoveAllListeners();
            _goalInputField.onSelect.RemoveAllListeners();
            _goalInputField.onEndEdit.RemoveAllListeners();
            _totalInputField.onSelect.RemoveAllListeners();
            _totalInputField.onEndEdit.RemoveAllListeners();
            _currentLevelInputField.onSelect.RemoveAllListeners();
            _currentLevelInputField.onEndEdit.RemoveAllListeners();
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

        private void RefreshLevelTotal(int total)
        {
            _total = total;
            _totalInputField.text = $"{_total}";
        }

        private void RefreshLevelGoal(int goal)
        {
            _goal = goal;
            _goalInputField.text = $"{_goal}";
        }

        private void RefreshCurrentLevel(int currentLevel)
        {
            _currentLevel = currentLevel;
            _currentLevelInputField.text = $"{currentLevel}";
        }

        public void SaveSettings()
        {
            SettingManager.Instance.UpdateDisplayName(_name, true);
            SettingManager.Instance.UpdateLevelTotal(_total, true);
            SettingManager.Instance.UpdateLevelGoal(_goal, true);
            LevelManager.Instance.OverrideCurrentLevel(_currentLevel);
        }
        #endregion

        #region Event
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
            if (int.TryParse(txt, out var goal))
            {
                goal = Mathf.Clamp(goal, 1, _total);

                RefreshLevelGoal(goal);
            }
            else
            {
                _goalInputField.text = _goalInputCache;
            }

            _goalInputCache = null;
        }

        private void OnTotalSelect(string txt)
        {
            _totalInputCache = txt;
        }

        private void OnTotalEndEdit(string txt)
        {
            if (int.TryParse(txt, out var total))
            {
                total = Mathf.Max(total, 1);

                if (total != _total || _goal > total)
                {
                    RefreshLevelGoal(Mathf.Max(1, total * GameGeneralConfig.GetData().DefaultGoal / GameGeneralConfig.GetData().DefaultBlock));
                }

                RefreshLevelTotal(total);
            }
            else
            {
                _totalInputField.text = _totalInputCache;
            }

            _totalInputCache = null;
        }

        private void OnCurrentLevelSelect(string txt)
        {
            _currentLevelInputCache = txt;
        }

        private void OnCurrentLevelEndEdit(string txt)
        {
            if (int.TryParse(txt, out var level))
            {
                level = Mathf.Clamp(level, CoreGameLevelConfig.MinLevel, CoreGameLevelConfig.MaxLevel);

                if (level != _currentLevel)
                {
                    RefreshCurrentLevel(level);
                }
            }
            else
            {
                _currentLevelInputField.text = _currentLevelInputCache;
            }

            _currentLevelInputCache = null;
        }
        #endregion
    }
}