using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class SingletonBase
    /// @brief A generic singleton base class that ensures only one instance of a derived class exists.
    /// 
    /// The `SingletonBase` class provides a thread-safe implementation for creating singleton patterns. It ensures that only one
    /// instance of the derived class exists, and it provides access to that instance through the `Instance` property. The constructor
    /// is protected to prevent instantiation outside of the derived class, ensuring the singleton pattern is followed.
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

        /// <summary>
        /// The constructor for the `SingletonBase` class. It ensures that only one instance of the derived class is created.
        /// </summary>
        public SingletonBase()
        {
            if (Instance == null)
            {
                Instance = (T)this;
            }
        }
    }
}