using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUpDown : MonoBehaviour
{
   public RectTransform panelTransform;
   public float moveSpeed = 2.0f;
   public float maxY = 200.0f;
   public float minY = 100.0f;

   private bool movingUp = true;
   
   void Start()
   {
      if(panelTransform == null)
      {
         panelTransform = GetComponent<RectTransform>();
      }
   }

   void Update()
   {
      if(movingUp)
      {
         panelTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
         if(panelTransform.anchoredPosition.y >= maxY)
         {
            movingUp = false;
         }
      }
      else
      {
         panelTransform.anchoredPosition -= Vector2.up *moveSpeed * Time.deltaTime;
         if(panelTransform.anchoredPosition.y <= minY)
         {
            movingUp = true;
         }
      }
   }

}
