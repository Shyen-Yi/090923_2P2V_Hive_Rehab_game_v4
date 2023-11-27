using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public enum StatsDayOfMonthState
    {
        None,
        Active,
        Inactive,
    }

    public struct StatsDayOfMonthWidgetData
    {
        public int day;
        public StatsDayOfMonthState state;
        public bool showBarLeft;
        public bool showBarRight;
        public bool showBarTop;
        public bool showBarBottom;

        public StatsDayOfMonthWidgetData(int day, StatsDayOfMonthState state, bool showBarLeft, bool showBarRight, bool showBarTop, bool showBarBottom)
        {
            this.day = day;
            this.state = state;
            this.showBarLeft = showBarLeft;
            this.showBarRight = showBarRight;
            this.showBarTop = showBarTop;
            this.showBarBottom = showBarBottom;
        }
    }

    public class StatsDayOfMonthWidgetController
    {
        public enum ExtraTMP
        {
            Day = 0,
        }

        private enum ExtraImg
        {
            BarLeft = 0,
            BarRight = 1,
            BarTop = 2,
            BarBottom = 3,
            CheckInactive = 4,
            CheckActive = 5,
        }

        public static void ShowData(GeneralWidgetConfig config, StatsDayOfMonthWidgetData data)
        {
            if (config == null)
                return;

            ShowDay(config, data.day);
            ShowBars(config, data.showBarLeft, data.showBarRight, data.showBarTop, data.showBarBottom);
            ShowState(config, data.state);
        }

        private static void ShowState(GeneralWidgetConfig config, StatsDayOfMonthState state)
        {
            var checkInactive = config.ExtraImages[(int)ExtraImg.CheckInactive];
            var checkActive = config.ExtraImages[(int)ExtraImg.CheckActive];
            checkInactive.enabled = state == StatsDayOfMonthState.Inactive;
            checkActive.enabled = state == StatsDayOfMonthState.Active;
        }

        private static void ShowBars(GeneralWidgetConfig config, bool showBarLeft, bool showBarRight, bool showBarTop, bool showBarBottom)
        {
            var barLeft = config.ExtraImages[(int)ExtraImg.BarLeft];
            var barRight = config.ExtraImages[(int)ExtraImg.BarRight];
            var barTop = config.ExtraImages[(int)ExtraImg.BarTop];
            var barBottom = config.ExtraImages[(int)ExtraImg.BarBottom];

            barLeft.enabled = showBarLeft;
            barRight.enabled = showBarRight;
            barTop.enabled = showBarTop;
            barBottom.enabled = showBarBottom;
        }

        private static void ShowDay(GeneralWidgetConfig config, int day)
        {
            var dayText = config.ExtraTextMeshPros[(int)ExtraTMP.Day];
            dayText.text = $"{day}";
        }
    }
}