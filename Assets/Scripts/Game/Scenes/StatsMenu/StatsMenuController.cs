using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace com.hive.projectr
{
    public class StatsMenuController : GameSceneControllerBase, ISimpleGridHandler
    {
        #region Extra
        private enum ExtraConfig
        {
            DayOfWeekSun = 0,
            DayOfWeekMon = 1,
            DayOfWeekTues = 2,
            DayOfWeekWed = 3,
            DayOfWeekThurs = 4,
            DayOfWeekFri = 5,
            DayOfWeekSat = 6,
        }

        private enum ExtraTMP
        {
            Performance = 0,
            Remark = 1,
            Streak = 2,
            Month = 3,
        }

        private enum ExtraBtn
        {
            Cross = 0,
            Share = 1,
            Contact = 2,
            Question = 3,
            PrevMonth = 4,
            NextMonth = 5,
        }

        private enum ExtraObj
        {
            Grid = 0,
        }

        private GeneralWidgetConfig _dayOfWeekSun;
        private GeneralWidgetConfig _dayOfWeekMon;
        private GeneralWidgetConfig _dayOfWeekTues;
        private GeneralWidgetConfig _dayOfWeekWed;
        private GeneralWidgetConfig _dayOfWeekThurs;
        private GeneralWidgetConfig _dayOfWeekFri;
        private GeneralWidgetConfig _dayOfWeekSat;

        private TMP_Text _performanceText;
        private TMP_Text _remarkText;
        private TMP_Text _streakText;
        private TMP_Text _monthText;

        private HiveButton _crossButton;
        private HiveButton _shareButton;
        private HiveButton _contactButton;
        private HiveButton _questionButton;
        private HiveButton _prevMonthButton;
        private HiveButton _nextMonthButton;

        private SimpleGrid _grid;
        #endregion

        #region Fields
        private List<StatsDayOfMonthWidgetData> _dayOfMonthDataList = new List<StatsDayOfMonthWidgetData>();
        private int _year;
        private int _month;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _dayOfWeekSun = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekSun];
            _dayOfWeekMon = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekMon];
            _dayOfWeekTues = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekTues];
            _dayOfWeekWed = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekWed];
            _dayOfWeekThurs = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekThurs];
            _dayOfWeekFri = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekFri];
            _dayOfWeekSat = Config.ExtraWidgetConfigs[(int)ExtraConfig.DayOfWeekSat];

            _performanceText = Config.ExtraTextMeshPros[(int)ExtraTMP.Performance];
            _remarkText = Config.ExtraTextMeshPros[(int)ExtraTMP.Remark];
            _streakText = Config.ExtraTextMeshPros[(int)ExtraTMP.Streak];
            _monthText = Config.ExtraTextMeshPros[(int)ExtraTMP.Month];

            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];
            _shareButton = Config.ExtraButtons[(int)ExtraBtn.Share];
            _contactButton = Config.ExtraButtons[(int)ExtraBtn.Contact];
            _questionButton = Config.ExtraButtons[(int)ExtraBtn.Question];
            _prevMonthButton = Config.ExtraButtons[(int)ExtraBtn.PrevMonth];
            _nextMonthButton = Config.ExtraButtons[(int)ExtraBtn.NextMonth];

            _grid = Config.ExtraObjects[(int)ExtraObj.Grid].GetComponent<SimpleGrid>();
        }

        protected override void OnShow(ISceneData data, GameSceneShowState showState)
        {
            base.OnShow(data, showState);

            _year = DateTime.Now.Year;
            _month = DateTime.Now.Month;

            RefreshPerformance();
            RefreshWeekly();
            RefreshMonthly();
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region Content
        private void RefreshPerformance()
        {

        }

        private void RefreshWeekly()
        {

        }

        private void RefreshMonthly()
        {
            // text
            _monthText.text = $"{TimeUtil.GetMonthName(_month)}, {_year}";

            // grid
            _dayOfMonthDataList.Clear();

            var daysInMonth = DateTime.DaysInMonth(_year, _month);
            for (var i = 0; i < daysInMonth; ++i)
            {
                var day = i + 1;
                var showLeft = day % 7 != 1;
                var showRight = day % 7 != 0;
                var showTop = day / 7 > 1;
                var showBottom = !(day % 7 > 0 && daysInMonth / 7 == day / 7);
                var data = new StatsDayOfMonthWidgetData(day, StatsDayOfMonthState.Active, showLeft, showRight, showTop, showBottom);
                _dayOfMonthDataList.Add(data);
            }

            _grid.Refresh();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _grid.Init(this);

            _crossButton.onClick.AddListener(OnCrossButtonClick);
            _shareButton.onClick.AddListener(OnShareButtonClick);
            _contactButton.onClick.AddListener(OnContactButtonClick);
            _questionButton.onClick.AddListener(OnQuestionButtonClick);
            _prevMonthButton.onClick.AddListener(OnPrevMonthButtonClick);
            _nextMonthButton.onClick.AddListener(OnNextMonthButtonClick);
        }

        private void UnbindActions()
        {
            _grid.Dispose();

            _crossButton.onClick.RemoveAllListeners();
            _shareButton.onClick.RemoveAllListeners();
            _contactButton.onClick.RemoveAllListeners();
            _questionButton.onClick.RemoveAllListeners();
            _prevMonthButton.onClick.RemoveAllListeners();
            _nextMonthButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(SceneName);
        }

        private void OnShareButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.Share);
        }

        private void OnContactButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.ContactInfo);
        }

        private void OnQuestionButtonClick()
        {
            GameSceneManager.Instance.LoadScene(SceneNames.FeatureInfo, new FeatureInfoData(FeatureType.Stats));
        }

        private void OnPrevMonthButtonClick()
        {
            if (_month == 1)
            {
                --_year;
                _month = 12;
            }
            else
            {
                --_month;
            }

            RefreshMonthly();
            _prevMonthButton.gameObject.SetActive(_month > 1 || _year > 1);
        }

        private void OnNextMonthButtonClick()
        {
            if (_month == 12)
            {
                ++_year;
                _month = 1;
            }
            else
            {
                ++_month;
            }

            RefreshMonthly();
            _nextMonthButton.gameObject.SetActive(_month < 12 || _year < 2999);
        }
        #endregion

        #region Grid
        public int GetDataCount()
        {
            return _dayOfMonthDataList.Count;
        }

        public void OnElementShow(SimpleGridElement element)
        {
            if (element is GeneralWidgetConfig config)
            {
                var data = _dayOfMonthDataList[config.Index];
                StatsDayOfMonthWidgetController.ShowData(config, data);
            }
        }

        public void OnElementHide(SimpleGridElement element)
        {
        }

        public void OnElementCreate(SimpleGridElement element)
        {
        }

        public void OnElementDestroy(SimpleGridElement element)
        {
        }
        #endregion
    }

    public class StatsMenuStorage
    {
        public PerformanceSOItem lastPerformance;
    }

}