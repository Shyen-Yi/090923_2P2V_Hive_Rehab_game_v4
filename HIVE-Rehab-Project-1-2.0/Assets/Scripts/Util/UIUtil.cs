using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class UIUtil
    {
        public static Vector2 ScreenPosToGameCoordinate(Vector2 screenPos)
        {
            return new Vector2(screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);
        }

        public static Vector2 WorldPosToGameCoordinate(Vector3 worldPos)
        {
            var screenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);
            return ScreenPosToGameCoordinate(screenPos);
        }
    }
}