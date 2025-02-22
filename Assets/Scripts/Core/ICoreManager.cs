using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    /// <summary>
    /// @ingroup Core
    /// @interface ICoreManager
    /// @brief Defines the core interface for managing the lifecycle of game systems that require initialization and disposal.
    /// 
    /// The 'ICoreManager' interface is intended to be implemented by game system or components that require proper
    /// initialization and disposal. It provides two methods: 'OnInit' for initializing the system or component, and 
    /// 'OnDisposal' for cleaning up resources when the system is no longer needed.
    /// 
    /// This ensures that each component can manage its lifecycle consistently within the game framework.
    /// </summary>
    public interface ICoreManager
    {
        /// <summary>
        /// Initializes the system or component, setting up any necessary data or state.
        /// </summary>
        public void OnInit();

        /// <summary>
        /// Disposes of the system or component, cleaning up any resources or state.
        /// </summary>
        public void OnDispose();
    }
}