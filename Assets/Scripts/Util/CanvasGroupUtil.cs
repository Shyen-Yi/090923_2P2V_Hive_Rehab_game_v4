using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public static class CanvasGroupUtil
    {
        public static void CanvasGroupOn(this CanvasGroup cg)
        {
            if (cg == null)
                return;

            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        public static void CanvasGroupOff(this CanvasGroup cg)
        {
            if (cg == null)
                return;

            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}