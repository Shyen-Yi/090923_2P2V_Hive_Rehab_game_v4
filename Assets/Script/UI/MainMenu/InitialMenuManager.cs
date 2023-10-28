using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InitialMenuManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
// {
//     public GameObject hoverImageContainer; // Reference to the container for the hover image.
//     public GameObject normalImageContainer; // Reference to the container for the normal image.

//     private Image hoverImage;
//     private Image normalImage;

//     private Sprite originalNormalImage; // the image displayed while the button is not hovered.
//     private Sprite originalHoverImage; // the image displayed while the button is hovered.

//     public void PlayGame()
//     {
//         SceneManager.LoadSceneAsync("settingMenu");
//     }

//     private void Start()
//     {
//         hoverImage = hoverImageContainer.GetComponent<Image>();
//         normalImage = normalImageContainer.GetComponent<Image>();
//         originalHoverImage = hoverImage.sprite;
//         originalNormalImage = normalImage.sprite;
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         if (hoverImage != null)
//         {
//             hoverImage.sprite = originalHoverImage;
//         }
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         if (normalImage != null)
//         {
//             normalImage.sprite = originalNormalImage;
//         }
//     }
// }
{
    public Image buttonImage; 
    public Sprite normalImage;
    public Sprite hoverImage;

    public float vectorX;
    public float vectorY;
    public float vectorDeduct;

    private void Start()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = normalImage;

        }

    }

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