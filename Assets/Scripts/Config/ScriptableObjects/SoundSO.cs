using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "ScriptableObject/Config Files/SoundConfig")]
    public class SoundSO : GameSOBase
    {
        [SerializeField] private List<SoundSOItem> _items;

        public List<SoundSOItem> Items => _items;
    }

    [System.Serializable]
    public class SoundSOItem
    {
        
		[SerializeField] private SoundType type;
		[SerializeField] private AssetReference clip;
		[SerializeField] private SoundPlayType playType;
		[SerializeField] private Boolean isLoop;
		[SerializeField] private Boolean needCache;

		public SoundType Type => type;
		public AssetReference Clip => clip;
		public SoundPlayType PlayType => playType;
		public Boolean IsLoop => isLoop;
		public Boolean NeedCache => needCache;
    }
}