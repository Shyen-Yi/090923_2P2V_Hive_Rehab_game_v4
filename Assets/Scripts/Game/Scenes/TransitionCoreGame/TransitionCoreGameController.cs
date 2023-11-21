using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.hive.projectr
{
    public class TransitionCoreGameController : GameSceneControllerBase
    {
        #region Fields
        public Coroutine animationCoroutine;
        #endregion

        #region Extra
        private enum ExtraBtn
        {
            Cross = 0,
        }

        private enum ExtraImg
        {
            ProgressBar = 0,
            SpaceShip = 1,
        }

        private enum ExtraSprite
        {
            SpaceshipInactive = 0,
            SpaceshipActive = 1,
        }

        private enum ExtraInt
        {
            DefaultSpeed = 0,
        }

        private enum ExtraRT
        {
            SpaceshipRoot = 0,
        }

        private HiveButton _crossButton;

        private Image _progressBar;
        private Image _spaceshipImg;

        private Sprite _spaceshipInactive;
        private Sprite _spaceshipActive;

        private float _defaultSpeed = 1f;

        private RectTransform _spaceshipRoot;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();

            SetProgress(1.0f, 5.0f);

            if (_spaceshipImg != null)
            {
                _spaceshipImg.sprite = _spaceshipInactive;
            }
        }

        private void InitExtra()
        {
            _crossButton = Config.ExtraButtons[(int)ExtraBtn.Cross];

            _progressBar = Config.ExtraImages[(int)ExtraImg.ProgressBar];
            _spaceshipImg = Config.ExtraImages[(int)ExtraImg.SpaceShip];

            _spaceshipInactive = Config.ExtraSprites[(int)ExtraSprite.SpaceshipInactive];
            _spaceshipActive = Config.ExtraSprites[(int)ExtraSprite.SpaceshipActive];

            _defaultSpeed = Config.ExtraInts[(int)ExtraInt.DefaultSpeed] / 1000f;

            _spaceshipRoot = Config.ExtraRectTransforms[(int)ExtraRT.SpaceshipRoot];
        }

        protected override void OnShow(ISceneData data)
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            _crossButton.onClick.AddListener(OnCrossButtonClick);
        }

        private void UnbindActions()
        {
            _crossButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Callback
        private void OnCrossButtonClick()
        {
            GameSceneManager.Instance.UnloadScene(Name);
        }
        #endregion

        #region Content
        public void SetProgress(float Progress)
        {
            SetProgress(Progress, _defaultSpeed);
        }

        public void SetProgress(float Progress, float Duration)
        {
            if (Progress < 0 || Progress > 1)
            {
                LogHelper.LogWarning($"Invalid progress passed, excepted value is between 0 and 1. Got{Progress}. Clamping");
                Progress = Mathf.Clamp01(Progress);
            }
            if (Progress != _progressBar.fillAmount)
            {
                float Speed = 1.0f / Duration;

                if (animationCoroutine != null)
                {
                    MonoBehaviourUtil.Instance.StopCoroutine(animationCoroutine);
                }

                animationCoroutine = MonoBehaviourUtil.Instance.StartCoroutine(AnimateProgress(Progress, Speed));
            }
        }

        public IEnumerator AnimateProgress(float Progress, float Speed)
        {
            float time = 0;
            float initialProgress = _progressBar.fillAmount;

            _spaceshipImg.sprite = _spaceshipActive;

            while (time < 1)
            {
                if (_progressBar == null)
                    yield break;

                _progressBar.fillAmount = Mathf.Lerp(initialProgress, Progress, time);
                time += Time.deltaTime * Speed;

                var xPos = Mathf.Lerp(0, ((RectTransform)_spaceshipRoot.parent).rect.width, _progressBar.fillAmount);
                _spaceshipRoot.anchoredPosition = new Vector2(xPos, _spaceshipRoot.anchoredPosition.y);

                yield return null;
            }

            _progressBar.fillAmount = Progress;

            GameSceneManager.Instance.LoadScene(SceneNames.CoreGame, null, ()=>
            {
                GameSceneManager.Instance.UnloadScene(Name);
            });
        }
        #endregion
    }
}