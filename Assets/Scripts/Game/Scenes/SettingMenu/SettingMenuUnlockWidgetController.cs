using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class SettingMenuUnlockWidgetController
    {
        private GeneralWidgetConfig _config;

        #region Lifecycle
        public SettingMenuUnlockWidgetController(GeneralWidgetConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            BindActions();
        }

        public void Show()
        {
            _config.CanvasGroup.CanvasGroupOn();
        }

        public void Hide()
        {
            _config.CanvasGroup.CanvasGroupOff();
        }

        public void Dispose()
        {
            UnbindActions();
        }
        #endregion

        #region Content
        private void InitExtra()
        {

        }
        #endregion

        #region UI Binding
        private void BindActions()
        {

        }

        private void UnbindActions()
        {

        }
        #endregion
    }
}