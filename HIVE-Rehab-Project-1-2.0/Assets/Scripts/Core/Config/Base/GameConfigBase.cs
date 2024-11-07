using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public abstract class GameConfigBase
    {
        public void Init()
        {
            OnInit();
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnInit() { }
        protected virtual void OnDispose() { }
    }
}