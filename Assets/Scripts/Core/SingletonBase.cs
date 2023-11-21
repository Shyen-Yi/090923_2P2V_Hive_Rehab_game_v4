using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public abstract class SingletonBase<T> where T : class, new()
    {
        public static T Instance 
        { 
            get => _instance; 
            protected set
            {
                lock (PadLock)
                {
                    _instance = value;
                }
            }
        }

        private static T _instance;

        private static readonly object PadLock = new object();
    }
}