using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelController : MonoBehaviour
{   
    public Button upButton;
    public Button downButton;

    public TextMeshProUGUI level;

    private int currentLevelIndex = 1;
    private int minLevelIndex = 1;
    private int maxLevelIndex = 10;

    // Start is called before the first frame update
    void Start()
    {
        upButton.onClick.AddListener(IncrementLevel);
        downButton.onClick.AddListener(DecrementLevel);
        
    }

    void IncrementLevel()
    {
        if(currentLevelIndex < maxLevelIndex)
        {
            currentLevelIndex += 1;
            UpdateLevelText();
        }
    }

    void DecrementLevel()
    {
        if(currentLevelIndex > minLevelIndex)
        {
            currentLevelIndex -=1;
            UpdateLevelText();
        }
    }

    void UpdateLevelText()
    {
        level.text = currentLevelIndex.ToString("00");
    }
}
