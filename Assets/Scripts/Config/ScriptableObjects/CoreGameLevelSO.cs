using System.Collections.Generic;
using UnityEngine;
using System;

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
		[SerializeField] private Single asteroidSpeed;
		[SerializeField] private Single asteroidSize;
		[SerializeField] private Single asteroidLifeTime;
		[SerializeField] private AnimationCurve asteroidMovement;
		[SerializeField] private Single vacuumSize;
		[SerializeField] private Single spacecraftSize;

		public Int32 Level => level;
		public Single AsteroidSpeed => asteroidSpeed;
		public Single AsteroidSize => asteroidSize;
		public Single AsteroidLifeTime => asteroidLifeTime;
		public AnimationCurve AsteroidMovement => asteroidMovement;
		public Single VacuumSize => vacuumSize;
		public Single SpacecraftSize => spacecraftSize;
    }
}