using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESeason
{
    Spring,
    Summer,
    Autumn,
    Winter
}

public enum EWeather
{
    Rain,
    Snow,
    Wind
}

[System.Serializable]
public class EnvironmentalStatus
{
    [SerializeField] string _dateTime;
    [SerializeField] private ESeason _seasonStatus;

    public DateTime DateTime
    { get { return Convert.ToDateTime(_dateTime); } }

    public ESeason SeasonStatus
    { get { return _seasonStatus; } }

    public EnvironmentalStatus()
    {
        _dateTime = new DateTime(1999, 1, 1, 8, 30, 00).ToString("O");
        _seasonStatus = ESeason.Spring;
    }

    public void SetSeasonStatus(ESeason eSeason)
    {
        _seasonStatus = eSeason;
    }

    public void SetDateTime(DateTime dateTime)
    {
        _dateTime = dateTime.ToString("O");
    }

    public void IncreaseDate(int minutesToIncrease)
    {
        DateTime dt = Convert.ToDateTime(_dateTime).AddMinutes(minutesToIncrease);
        _dateTime = dt.ToString("O");
    }
}
