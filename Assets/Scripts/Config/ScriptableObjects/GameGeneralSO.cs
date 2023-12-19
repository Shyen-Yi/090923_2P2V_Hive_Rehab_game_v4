using System.Collections.Generic;
using UnityEngine;
using System;

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

		public Int32 DefaultGoal => defaultGoal;
		public Int32 MinGoal => minGoal;
		public Int32 MaxGoal => maxGoal;
		public Single CoreGameTransitionSec => coreGameTransitionSec;
    }
}