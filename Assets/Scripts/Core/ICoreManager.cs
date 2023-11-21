using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public interface ICoreManager
    {
        public void OnInit();
        public void OnDispose();
    }
}