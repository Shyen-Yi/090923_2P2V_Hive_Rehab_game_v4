using System.Collections.Generic;
using UnityEngine;
using System;

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

		public Single AsteroidSpinRoundPerSec => asteroidSpinRoundPerSec;
		public Int32 StartCountdownSec => startCountdownSec;
    }
}