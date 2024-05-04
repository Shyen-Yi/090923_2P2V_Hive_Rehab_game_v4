using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "CoreGameConfig", menuName = "ScriptableObject/Config Files/CoreGameConfig")]
    public class CoreGameSO : GameSOBase
    {
        [SerializeField] private List<CoreGameSOItem> _items;

        public List<CoreGameSOItem> Items => _items;
    }

    [System.Serializable]
    public class CoreGameSOItem
    {
        
		[SerializeField] private Single asteroidSpinRoundPerSec;
		[SerializeField] private Int32 startCountdownSec;
		[SerializeField] private Int32 infoTextUpdateSec;
		[SerializeField] private Single gameProgressFillSec;
		[SerializeField] private Single asteroidWarningSec;
		[SerializeField] private Single arrowDistanceFromScreenEdge;

		public Single AsteroidSpinRoundPerSec => asteroidSpinRoundPerSec;
		public Int32 StartCountdownSec => startCountdownSec;
		public Int32 InfoTextUpdateSec => infoTextUpdateSec;
		public Single GameProgressFillSec => gameProgressFillSec;
		public Single AsteroidWarningSec => asteroidWarningSec;
		public Single ArrowDistanceFromScreenEdge => arrowDistanceFromScreenEdge;
    }
}