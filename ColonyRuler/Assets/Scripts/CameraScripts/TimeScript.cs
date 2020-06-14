using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class for managing time: game speed, number of day, year
/// Only one in the game
/// </summary>
[Serializable]
public class TimeScript : MonoBehaviour
{
    /// <summary> game speed </summary>
    public float m_speed = 1.0f;
    /// <summary> current day of year </summary>
    public int m_day = 0;
    /// <summary> current year </summary>
    public int m_year = 0;
    /// <summary> how many days in the year </summary>
    int _yearLenght = 300;
    /// <summary> time of starting this day </summary>
    float _thisDayTime;
    /// <summary> gameObject for showing time </summary>
    public Text m_timeText;
    /// <summary> text before time </summary>
    public string m_timeTxt;
    /// <summary> global pause </summary>
    public static bool m_isItPaused = false;
    /// <summary> pause for debugging </summary>
    public static bool m_sIsItPaused = false;
    /// <summary> current instance </summary>
    static TimeScript _instnace = null;

    /// <summary>
    /// current year
    /// </summary>
    public int Year
    {
        get { return m_year; }
    }

    /// <summary>
    /// total days in the game
    /// </summary>
    public int TotalDays
    {
        get { return m_year * _yearLenght + m_day; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _thisDayTime = Time.time;
        _instnace = this;
    }

    /// <summary>
    /// Enable/disable pause.
    /// For button click
    /// </summary>
    public void Pause()
    {
        if (m_isItPaused)
        {
            m_isItPaused = false;
            _thisDayTime = Time.time;
        }
        else
        {
            m_isItPaused = true;
            _thisDayTime = float.MaxValue;
        }
    }

    /// <summary>
    /// total days in the game
    /// </summary>
    public static int GetTotalDays()
    {
        if (_instnace == null)
            _instnace = Camera.main.GetComponent<TimeScript>();
        return _instnace.TotalDays;
    }

    /// <summary>
    /// Find button on a panel and set click
    /// </summary>
    /// <param name="panel"> panel, where button located </param>
    /// <param name="name"> button name </param>
    /// <param name="clc"> button.click method </param>
    void FindButtonAndSetClick(GameObject panel, string name, UnityAction clc)
    {
        Button btn = panel.transform.Find(name).GetComponent<Button>();
        btn.onClick.AddListener(clc);
    }

    /// <summary>
    /// Initialization of time script
    /// </summary>
    /// <param name="panel"> panel, where time buttons and time text located </param>
    public void Initialization(GameObject panel)
    {
        m_timeTxt += m_timeText.GetComponent<Text>().text;
        FindButtonAndSetClick(panel, "TimePlus", TimePlus);
        FindButtonAndSetClick(panel, "TimeMinus", TimeMinus);
        FindButtonAndSetClick(panel, "TimePause", Pause);
    }

    /// <summary>
    /// Increase game speed
    /// </summary>
    public void TimePlus()
    {
        if (m_isItPaused)
            Pause();
        else if (m_speed < 512)
            m_speed *= 2;
    }

    /// <summary>
    /// Descrease game speed
    /// </summary>
    public void TimeMinus()
    {
        if (m_isItPaused)
            Pause();
        else if (m_speed > 0.128)
            m_speed /= 2;
    }

    /// <summary>
    /// Starting new day, also, start all works in all productions
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        try { 
            if (!m_isItPaused && !m_sIsItPaused)
            {
                Localization loc = Localization.GetLocalization();
                Text txt = m_timeText;
                string timetxt = loc.m_ui.m_timeText;
                if (m_year > 0)
                {
                    timetxt += " " + loc.m_ui.m_yearText + m_year.ToString();
                }
                timetxt += " " + loc.m_ui.m_dayText + m_day.ToString();
                if (m_isItPaused) { timetxt += " " + loc.m_ui.m_pausedText; }
                else
                {
                    timetxt += " " + loc.m_ui.m_speedText + m_speed.ToString() + "x";
                }
                txt.text = timetxt;

                if ((Time.time - _thisDayTime) * m_speed >= 1.0f)
                {
                    int days = (int)((Time.time - _thisDayTime) * m_speed);
                    while (days > 0)
                    {
                        m_day++;
                        if (m_day > _yearLenght)
                        {
                            m_day -= _yearLenght;
                            m_year++;
                        }
                        AbstractObject.DoWork();
                        days--;
                    }
                    _thisDayTime = Time.time;

                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("TimeScript Update exception" + ex.Message);
        }
    }
}
