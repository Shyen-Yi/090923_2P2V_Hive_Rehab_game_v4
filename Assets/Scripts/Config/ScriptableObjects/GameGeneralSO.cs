using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "GameGeneralConfig", menuName = "ScriptableObject/Config Files/GameGeneralConfig")]
    public class GameGeneralSO : GameSOBase
    {
        [SerializeField] private List<GameGeneralSOItem> _items;

        public List<GameGeneralSOItem> Items => _items;
    }

    [System.Serializable]
    public class GameGeneralSOItem
    {
        
		[SerializeField] private Int32 defaultBlock;
		[SerializeField] private Int32 defaultGoal;
		[SerializeField] private Single coreGameTransitionSec;
		[SerializeField] private Single calibrationTransitionSec;
		[SerializeField] private String defaultUserName;
		[SerializeField] private String adminUsername;
		[SerializeField] private String adminPassword;
		[SerializeField] private Int32 soundVolumePercentageWhenInBackground;
		[SerializeField] private Int32 dailyMaxAttempt;

		public Int32 DefaultBlock => defaultBlock;
		public Int32 DefaultGoal => defaultGoal;
		public Single CoreGameTransitionSec => coreGameTransitionSec;
		public Single CalibrationTransitionSec => calibrationTransitionSec;
		public String DefaultUserName => defaultUserName;
		public String AdminUsername => adminUsername;
		public String AdminPassword => adminPassword;
		public Int32 SoundVolumePercentageWhenInBackground => soundVolumePercentageWhenInBackground;
		public Int32 DailyMaxAttempt => dailyMaxAttempt;
    }
}