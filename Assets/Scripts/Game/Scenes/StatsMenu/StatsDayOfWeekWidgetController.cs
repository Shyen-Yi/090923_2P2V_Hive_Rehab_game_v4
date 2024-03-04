using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public struct StatsDayOfWeekWidgetData
    {
        public bool isActive;

        public StatsDayOfWeekWidgetData(bool isActive)
        {
            this.isActive = isActive;
        }
    }

    public class StatsDayOfWeekWidgetController
    {
        private enum ExtraCG
        {
            Inactive = 0,
            Active = 1,
        }

        public static void ShowData(GeneralWidgetConfig config, StatsDayOfWeekWidgetData data)
        {
            if (config == null)
                return;

            var inactiveCanvasGroup = config.ExtraCanvasGroups[(int)ExtraCG.Inactive];
            var activeCanvasGroup = config.ExtraCanvasGroups[(int)ExtraCG.Active];
            if (data.isActive)
            {
                activeCanvasGroup.CanvasGroupOn();
                inactiveCanvasGroup.CanvasGroupOff();
            }
            else
            {
                activeCanvasGroup.CanvasGroupOff();
                inactiveCanvasGroup.CanvasGroupOn();
            }
        }
    }
}