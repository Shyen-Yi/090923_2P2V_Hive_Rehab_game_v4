using System.Threading;
using System.Xml.Schema;
using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class transitionLoadingBar : MonoBehaviour
{
    public Image progressBar;
    public Image spaceShipNormal;
    public Sprite spaceShipInactive;
    public Sprite spaceShipActive;
    public float defaultSpeed = 1f;
    public UnityEvent<float> OnProgress;
    public UnityEvent OnCompleted;
    public Coroutine animationCoroutine; 

    public void Start()
    { 
        if(progressBar.type !=  Image.Type.Filled)
        {
            UnityEngine.Debug.LogError($"{name}'s progressBar is not of type \"Filled\" so it cannot be used" +
            $" as a progress bar. Disabling this Progress Bar");
            this.enabled = false;
        }

        SetProgress(1.0f, 5.0f);

        if(spaceShipNormal != null)
        {
            spaceShipNormal.sprite = spaceShipInactive;
        }
    }

    public void SetProgress (float Progress)
    {
        SetProgress(Progress, defaultSpeed);
    }

    public void SetProgress(float Progress, float Duration)
    {
        if (Progress < 0 || Progress > 1)
        {
            UnityEngine.Debug.LogWarning($"Invalid progress passed, excepted value is between 0 and 1. Got{Progress}. Clamping");
            Progress = Mathf.Clamp01(Progress);
        }
        if (Progress != progressBar.fillAmount)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            float Speed = 1.0f / Duration;

            animationCoroutine = StartCoroutine(AnimateProgress(Progress, Speed));
        }
    }

    public IEnumerator AnimateProgress(float Progress, float Speed)
    {
        float time = 0;
        float initialProgress = progressBar.fillAmount; 

        spaceShipNormal.sprite = spaceShipActive;

        while (time < 1)
        {
            progressBar.fillAmount = Mathf.Lerp(initialProgress, Progress, time);
            time += Time.deltaTime * Speed;

            float xPos = Mathf.Lerp(-109f, 130f, progressBar.fillAmount);
            spaceShipNormal.rectTransform.anchoredPosition = new Vector2( xPos * 1 ,spaceShipNormal.rectTransform.anchoredPosition.y);

            if (OnProgress != null)
            {
                OnProgress?.Invoke(progressBar.fillAmount);
                spaceShipNormal.sprite = spaceShipActive;
                yield return null;
            }
        }

        progressBar.fillAmount = Progress;
        OnProgress?.Invoke(Progress);
        UnityEngine.Debug.Log($"Animating progress: {progressBar.fillAmount}");

            if (OnCompleted != null)
            {
                OnCompleted?.Invoke();
                spaceShipNormal.sprite = spaceShipActive;

                UnityEngine.Debug.Log("Animation completed");

                LoadNewScene();
            }

        void LoadNewScene()
        {
            SceneManager.LoadScene("CoreGame");
        }
        
    }
}
