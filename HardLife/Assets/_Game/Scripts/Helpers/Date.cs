﻿using System;
using UnityEngine;

[Serializable]
public struct Date
{
    private static int Seasons = 4;
    private static int Days = 40;
    private static int Hours = 24;
    private static int Minutes = 15;

    public static int Year = 14400; //Seconds in a year 14400
    public static int Season = 3600;
    public static int Day = 360;
    public static int Hour = 15;

    public float time;
    public float deltaTime;
    public int year;
    public int season;
    public int day;
    public int hour;
    private float minute;

    public Date(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathf.FloorToInt(_time / Season);
        day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;
        minute = _time;
    }

    public void AddTime(float _time)
    {
        deltaTime = _time;
        time += _time;

        minute += _time;
        if (minute > Minutes) //Setting the Add Time parts
        {
            hour += Mathf.FloorToInt(minute / Minutes);
            minute = minute % Minutes;

            if (hour > Hours)
            {
                day += Mathf.FloorToInt(hour / Hours);
                hour = hour % Hours;

                season = Mathf.FloorToInt(day / 10f);

                if (day > Days)
                {
                    year += Mathf.FloorToInt(day / Days);
                    day = day % Days;
                    
                }
            }
        }



    }

    internal void UpdateDate(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathf.FloorToInt(_time / Season);
        day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;
    }

    public string GetDate()
    {
        return year + " Year(s), " + (day + 1) + " Day(s)";
    }

    public string GetDateTime()
    {
        return GetSeason() + " - " + hour.ToString().PadLeft(2, '0') + " h " + Mathf.FloorToInt(minute).ToString().PadLeft(2, '0') + " m\n" + "Day " + (day + 1) + ", Year " + year;
    }
    //public string GetDate(float _time)
    //{
    //    float year = Mathf.FloorToInt(_time / Year);
    //    _time = _time % Year;
    //    float season = Mathf.FloorToInt(_time / Season);
    //    float day = Mathf.FloorToInt(_time / Day);
    //    _time = _time % Day;
    //    float hour = Mathf.FloorToInt(_time / Hour);
    //    _time = _time % Hour;

    //    return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    //}

    private string GetSeason()
    {
        string[] seasonNames = new string[4] { "Spring", "Summer", "Fall", "Winter" };
        return seasonNames[season];
    }

    public static Date operator -( Date date1, Date date2)
    {
        float diff = date1.time - date2.time;

        Date finalDate = new Date(diff);

        return finalDate;

    }

    public static bool operator >(Date date1, Date date2)
    {
        return (date1.time > date2.time) ? true:false;


    }

    public static bool operator <(Date date1, Date date2)
    {
        return (date1.time < date2.time) ? true : false;
    }

    public static bool operator >=(Date date1, Date date2)
    {
        return (date1.time >= date2.time) ? true : false;
    }
    public static bool operator <=(Date date1, Date date2)
    {
        return (date1.time <= date2.time) ? true : false;
    }
}