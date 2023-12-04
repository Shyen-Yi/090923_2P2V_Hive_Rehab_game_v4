using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public interface IPoolable
    {
    }

    public class PoolManager : SingletonBase<PoolManager>, ICoreManager
    {
        private Dictionary<int, HashSet<IPoolable>> _poolDict; // hashcode, storage

        public void OnInit()
        {
            _poolDict = new Dictionary<int, HashSet<IPoolable>>();
        }

        public void OnDispose()
        {
            _poolDict.Clear();
            _poolDict = null;
        }

        public void RegisterPool(IPoolable prefab)
        {
            if (prefab == null)
                return;

            var hashCode = prefab.GetHashCode();
            if (!_poolDict.ContainsKey(hashCode))
            {
                _poolDict[hashCode] = new HashSet<IPoolable>();
            }
            else
            {
                Logger.LogError($"Pool already registered! Hashcode: {hashCode}");
            }
        }

        public void DeregisterPool(IPoolable prefab)
        {
            if (prefab == null)
                return;

            var hashCode = prefab.GetHashCode();
            if (_poolDict.ContainsKey(hashCode))
            {
                _poolDict.Remove(hashCode);
            }
            else
            {
                Logger.LogError($"Pool not registered yet! Hashcode: {hashCode}");
            }
        }
    }
}