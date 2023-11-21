using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DatePickerController : MonoBehaviour
{
    // public GameObject theDisplay;
    public TextMeshProUGUI dateText;
    public int year;
    public int month;
    public int day;

    void Update()
    { 

        {
            year = System.DateTime.Now.Year;
            month = System.DateTime.Now.Month;
            day = System.DateTime.Now.Day;

            string dateString = year + "/" + month.ToString("00") + "/" + day.ToString("00");
            dateText.text = dateString;
            // theDisplay.GetComponent<TextMeshProUGUI>().text = dateString;
        }
    }
}
