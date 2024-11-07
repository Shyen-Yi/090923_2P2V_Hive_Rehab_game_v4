using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>
    {
        public static T Instance 
        {
            get => _instance;
            private set
            {
                lock (PadLock)
                {
                    _instance = value;
                }
            }
        }

        private static T _instance;

        private static readonly object PadLock = new object();

        public SingletonBase()
        {
            if (Instance == null)
            {
                Instance = (T)this;
            }
        }
    }
}