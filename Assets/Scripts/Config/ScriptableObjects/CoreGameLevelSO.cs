using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "CoreGameLevelConfig", menuName = "ScriptableObject/Config Files/CoreGameLevelConfig")]
    public class CoreGameLevelSO : GameSOBase
    {
        [SerializeField] private List<CoreGameLevelSOItem> _items;

        public List<CoreGameLevelSOItem> Items => _items;
    }

    [System.Serializable]
    public class CoreGameLevelSOItem
    {
        
		[SerializeField] private Int32 level;
		[SerializeField] private Int32 requiredDailyWinningStreakToPass;
		[SerializeField] private Single asteroidSpeed;
		[SerializeField] private Single asteroidSize;
		[SerializeField] private Single asteroidLifeTime;
		[SerializeField] private AnimationCurve asteroidMovement;
		[SerializeField] private Single asteroidSpawnGapSec;
		[SerializeField] private Single vacuumSize;
		[SerializeField] private Single spacecraftSize;
		[SerializeField] private Int32 maxAsteroidCount;
		[SerializeField] private Int32 numOfAsteroidCollectedToPass;

		public Int32 Level => level;
		public Int32 RequiredDailyWinningStreakToPass => requiredDailyWinningStreakToPass;
		public Single AsteroidSpeed => asteroidSpeed;
		public Single AsteroidSize => asteroidSize;
		public Single AsteroidLifeTime => asteroidLifeTime;
		public AnimationCurve AsteroidMovement => asteroidMovement;
		public Single AsteroidSpawnGapSec => asteroidSpawnGapSec;
		public Single VacuumSize => vacuumSize;
		public Single SpacecraftSize => spacecraftSize;
		public Int32 MaxAsteroidCount => maxAsteroidCount;
		public Int32 NumOfAsteroidCollectedToPass => numOfAsteroidCollectedToPass;
    }
}