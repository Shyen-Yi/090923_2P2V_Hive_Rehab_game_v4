using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "CalibrationConfig", menuName = "ScriptableObject/Config Files/CalibrationConfig")]
    public class CalibrationSO : GameSOBase
    {
        [SerializeField] private List<CalibrationSOItem> _items;

        public List<CalibrationSOItem> Items => _items;
    }

    [System.Serializable]
    public class CalibrationSOItem
    {
        
		[SerializeField] private Single arrowWorldDistanceFromCenter;
		[SerializeField] private Single stageErrorProtectionTriggerTime;
		[SerializeField] private Single holdingMaxScreenOffset;
		[SerializeField] private Single pendingMarkerAlpha;

		public Single ArrowWorldDistanceFromCenter => arrowWorldDistanceFromCenter;
		public Single StageErrorProtectionTriggerTime => stageErrorProtectionTriggerTime;
		public Single HoldingMaxScreenOffset => holdingMaxScreenOffset;
		public Single PendingMarkerAlpha => pendingMarkerAlpha;
    }
}