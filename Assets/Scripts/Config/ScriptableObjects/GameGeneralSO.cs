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
        
		[SerializeField] private Int32 defaultGoal;
		[SerializeField] private Int32 minGoal;
		[SerializeField] private Int32 maxGoal;
		[SerializeField] private Single coreGameTransitionSec;
		[SerializeField] private Single calibrationTransitionSec;
		[SerializeField] private Int32 passingStreakToNextLevel;
		[SerializeField] private String defaultUserName;
		[SerializeField] private String adminUsername;
		[SerializeField] private String adminPassword;
		[SerializeField] private Int32 soundVolumePercentageWhenInBackground;

		public Int32 DefaultGoal => defaultGoal;
		public Int32 MinGoal => minGoal;
		public Int32 MaxGoal => maxGoal;
		public Single CoreGameTransitionSec => coreGameTransitionSec;
		public Single CalibrationTransitionSec => calibrationTransitionSec;
		public Int32 PassingStreakToNextLevel => passingStreakToNextLevel;
		public String DefaultUserName => defaultUserName;
		public String AdminUsername => adminUsername;
		public String AdminPassword => adminPassword;
		public Int32 SoundVolumePercentageWhenInBackground => soundVolumePercentageWhenInBackground;
    }
}