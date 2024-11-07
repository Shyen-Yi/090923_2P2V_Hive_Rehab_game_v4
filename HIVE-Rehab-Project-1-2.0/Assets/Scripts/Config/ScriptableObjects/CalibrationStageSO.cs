using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "CalibrationStageConfig", menuName = "ScriptableObject/Config Files/CalibrationStageConfig")]
    public class CalibrationStageSO : GameSOBase
    {
        [SerializeField] private List<CalibrationStageSOItem> _items;

        public List<CalibrationStageSOItem> Items => _items;
    }

    [System.Serializable]
    public class CalibrationStageSOItem
    {
        
		[SerializeField] private CalibrationStageType stage;
		[SerializeField] private Int32 sortingId;
		[SerializeField] private String instruction;
		[SerializeField] private String instructionWhenHolding;
		[SerializeField] private Int32 maxHoldingCheckCount;
		[SerializeField] private Single eachHoldingCheckDuration;
		[SerializeField] private Single holdingPreparationTime;
		[SerializeField] private Single cooldownTime;

		public CalibrationStageType Stage => stage;
		public Int32 SortingId => sortingId;
		public String Instruction => instruction;
		public String InstructionWhenHolding => instructionWhenHolding;
		public Int32 MaxHoldingCheckCount => maxHoldingCheckCount;
		public Single EachHoldingCheckDuration => eachHoldingCheckDuration;
		public Single HoldingPreparationTime => holdingPreparationTime;
		public Single CooldownTime => cooldownTime;
    }
}