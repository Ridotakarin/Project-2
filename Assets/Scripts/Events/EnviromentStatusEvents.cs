using System;
using UnityEngine;

public class EnviromentStatusEvents
{
    public event Action<ESeason> onChangeSeason;
    public void CHangeSeason(ESeason season)
    {
        onChangeSeason?.Invoke(season);
    }

    public event Action<int> onTimeIncrease;
    public void TimeIncrease(int time)
    {
        onTimeIncrease?.Invoke(time);
    }
}
