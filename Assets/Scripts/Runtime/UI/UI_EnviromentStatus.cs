using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_EnviromentStatus : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public RectTransform clockHand;

    private void OnEnable()
    {
        GameEventsManager.Instance.dateTimeEvents.onDateChanged += UpdateDateText;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.dateTimeEvents.onDateChanged -= UpdateDateText;
    }

    private void UpdateDateText(DateTime dateTime)
    {
        float hourAngle = -(float)(dateTime.Hour * 360f) / 24f -(float)(dateTime.Minute * 360f) / 1440f - 180f;
        if (clockHand != null) clockHand.localRotation = Quaternion.Euler(0, 0, hourAngle);

        dateText.text = $"{dateTime.Day:D2} - {dateTime.Month:D2} - {dateTime.Year:D4}";
        
        int hour = dateTime.Hour % 12;
        hour = (hour == 0) ? 12 : hour;
        string amPm = dateTime.Hour >= 12 ? "PM" : "AM";

        timeText.text = $"{hour:D2} : {dateTime.Minute:D2} {amPm}";
    }
}
