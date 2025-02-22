using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.HandsModule;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class MotionTracker
    /// @brief Tracks hand movements using the Leap Motion framework, managing hand model data and sensitivity settings.
    /// 
    /// The `MotionTracker` class interfaces with the Leap Motion framework to track hand movements in the game. It provides access
    /// to hand model data and exposes sensitivity settings that can be adjusted to fine-tune hand tracking behavior.
    public class MotionTracker : MonoBehaviour
    {
        /// <summary>
        /// The HandModelManager component that manages the hand models.
        /// </summary>
        [SerializeField] private HandModelManager _handModelManager;

        /// <summary>
        /// The sensitivity setting that controls the tracking responsiveness.
        /// </summary>
        [SerializeField, Range(.1f, 10f)] private float _sensitivity;

        /// <summary>
        /// A base sensitivity scale that adjusts the sensitivity for different contexts.
        /// </summary>
        [SerializeField] private float _baseSensitivityScale;

        /// <summary>
        /// Gets the HandModelManager associated with the MotionTracker.
        /// </summary>
        public HandModelManager HandModelManager => _handModelManager;

        /// <summary>
        /// Gets the sensitivity value for hand tracking.
        /// </summary>
        public float Sensitivity => _sensitivity;

        /// <summary>
        /// Gets the base sensitivity scale applied to hand tracking.
        /// </summary>
        public float BaseSensitivityScale => _baseSensitivityScale;
    }
}