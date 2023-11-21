using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InitialMenuManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage; 
    public Sprite normalImage;
    public Sprite hoverImage;

    public float vectorX;
    public float vectorY;
    public float vectorDeduct;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Called when the mouse pointer enters the button.
        if (buttonImage != null)
        {
            vectorX += vectorDeduct;
            vectorY += vectorDeduct;
            buttonImage.sprite = hoverImage;
            Vector2 newSize = new Vector2(vectorX, vectorY);
            buttonImage.rectTransform.sizeDelta = newSize;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Called when the mouse pointer exits the button.
        if (buttonImage != null)
        {
            vectorX -= vectorDeduct;
            vectorY -= vectorDeduct;
            buttonImage.sprite = normalImage;
            Vector2 newSize2 = new Vector2(vectorX, vectorY);
            buttonImage.rectTransform.sizeDelta = newSize2;
        }
    }

}