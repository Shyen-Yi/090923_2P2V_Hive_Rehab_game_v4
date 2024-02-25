using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public enum ESpacecraftPanelState
    {
        None,
        Hold,
        Redo,
        Success,
    }

    public class SpacecraftCalibrationController : SpacecraftController
    {
        #region Extra
        private enum ExtraObj
        {
            Hold = 0,
            Redo = 1,
            Success = 2,
        }

        private Transform _hold;
        private Transform _redo;
        private Transform _success;
        #endregion

        public SpacecraftCalibrationController(SpacecraftConfig config) : base(config)
        {
            InitExtra();

            _hold.gameObject.SetActive(false);
            _redo.gameObject.SetActive(false);
            _success.gameObject.SetActive(false);
        }

        private void InitExtra()
        {
            _hold = _config.ExtraObjects[(int)ExtraObj.Hold];
            _redo = _config.ExtraObjects[(int)ExtraObj.Redo];
            _success = _config.ExtraObjects[(int)ExtraObj.Success];
        }

        public void OnHold()
        {
        }

        public void OnRedo()
        {
        }

        public void OnSuccess()
        {
        }
    }
}