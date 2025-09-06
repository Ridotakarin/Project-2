using System;
using UnityEngine;

public class DateTimeEvents
{
    public event Action<DateTime> onDateChanged;
    public void DateChanged(DateTime dateTime)
    {
        onDateChanged?.Invoke(dateTime);
    }

    public event Action<int> onMinuteIncrease;
    public void MinuteIncreased(int minute)
    {
        onMinuteIncrease?.Invoke(minute);
    }
}
