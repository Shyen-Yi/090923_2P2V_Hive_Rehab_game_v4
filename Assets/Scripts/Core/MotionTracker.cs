using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.HandsModule;

namespace com.hive.projectr
{
    public class MotionTracker : MonoBehaviour
    {
        [SerializeField] private HandModelManager _handModelManager;
        [SerializeField, Range(.1f, 10f)] private float _sensitivity;
        [SerializeField] private float _baseSensitivityScale;

        public HandModelManager HandModelManager => _handModelManager;
        public float Sensitivity => _sensitivity;
        public float BaseSensitivityScale => _baseSensitivityScale;
    }
}