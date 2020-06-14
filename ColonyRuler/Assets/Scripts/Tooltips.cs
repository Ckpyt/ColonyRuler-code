using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tooltips + some debug buttons
/// </summary>
public class Tooltips : MonoBehaviour
{
    /// <summary> if game paused, tooltips should not move </summary>
    public static bool m_isItPaused = false;

    /// <summary>
    /// One tooltip for an object
    /// </summary>
    public class Tooltip
    {
        /// <summary> tooltip text </summary>
        public string m_text;
        /// <summary> there tooltip should be shown </summary>
        public Rect m_rect;

        public Tooltip(Rect rt, string text)
        {
            m_rect = rt;
            m_text = text;
        }
    }

    /// <summary> the text of selected tooltip </summary>
    public string m_tooltipText = null;
    /// <summary> all tooltips of an object or UI </summary>
    List<Tooltip> _allTooltips = new List<Tooltip>();
    /// <summary> Is tooltip showing? </summary>
    bool _isShow = false;
    /// <summary> Could the tooltip text change? </summary>
    bool _isTextChangable = false;
    /// <summary> tooltip that has a rectangle with a cursor inside </summary>
    Tooltip _current = null;
    /// <summary> is any tooltip shown? </summary>
    public static bool m_sIsSomeoneShow = false;
    /// <summary> outside position. Used for moving tooltip outside of the screen </summary>
    static Vector2 _outside = new Vector2(0, -100);

    /// <summary> Text object for tooltip </summary>
    public Text m_tooltipTextObject = null;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            if (m_tooltipText != null)
            {
                RectTransform rttr = transform as RectTransform;
                Rect rt = new Rect();
                rt.size = rttr.sizeDelta;
                rt.center = transform.position;
                _allTooltips.Add(new Tooltip(rt, m_tooltipText));
                _isTextChangable = true;
            }
            MainScript ms = Camera.main.GetComponent<MainScript>();
            m_tooltipTextObject = ms.m_toolTipText;
            Localization.m_onLanguageChanged += ChangeLanguage;
            ChangeLanguage();
        }
        catch (Exception ex)
        {
            Debug.LogError("Tooltips Start exception" + ex.Message);
        }
    }

    /// <summary>
    /// Event of destroing object. Called by Unity
    /// </summary>
    public void OnDestroy()
    {
        Localization.m_onLanguageChanged -= ChangeLanguage;
    }

    /// <summary>
    /// Change all text data into current language
    /// </summary>
    public void ChangeLanguage()
    {
        Localization loc = Localization.GetLocalization();
        switch (name)
        {
            case "EmojiBoost(Clone)":
                m_tooltipText = loc.m_ui.m_emojiBoost;
                break;
            case "Hungry":
                m_tooltipText = loc.m_ui.m_starving;
                break;
            case "TimePause":
                m_tooltipText = loc.m_ui.m_timePause;
                break;
            case "TimePlus":
                m_tooltipText = loc.m_ui.m_timePlus;
                break;
            case "TimeMinus":
                m_tooltipText = loc.m_ui.m_timeMinus;
                break;
            case "WorkersValue":
                m_tooltipText = loc.m_ui.m_workersTooltip;
                break;
            case "PopulationValue":
                m_tooltipText = loc.m_ui.m_populationTooltip;
                break;
            case "LightStorageValue":
                m_tooltipText = loc.m_ui.m_lightStorageTooltip;
                break;
            case "HeavyStorageValue":
                m_tooltipText = loc.m_ui.m_heavyStorageTooltip;
                break;
            case "LivingSpaceValue":
                m_tooltipText = loc.m_ui.m_livingSpaceTooltip;
                break;
            case "TerritoryValue":
                m_tooltipText = loc.m_ui.m_territoryTooltip;
                break;

        }
    }

    /// <summary>
    /// Set tooltip to the object
    /// </summary>
    /// <returns> new tooltip </returns>
    public Tooltip Set(Vector2 rightUpAngle, Vector2 leftDownAngle, string text)
    {
        Rect rt = new Rect();
        rt.max = rightUpAngle;
        rt.min = leftDownAngle;
        return Set(rt, text);
    }

    /// <summary>
    /// clear all tooltips of object
    /// </summary>
    public void Clear()
    {
        _allTooltips.Clear();
    }

    /// <summary>
    /// Set tooltip to the object
    /// </summary>
    /// <returns> new tooltip </returns>
    public Tooltip Set(Rect rect, string text)
    {
        Tooltip tp = new Tooltip(rect, text);
        _allTooltips.Add(tp);
        return tp;
    }

    /// <summary>
    /// Debug button pressed. Paused only one script
    /// </summary>
    public void PauseDebugButtonPressed()
    {
        GC.Collect();
        string name = gameObject.name;
        switch (name)
        {
            case "1":
                IconScript.m_isItPaused1 = !IconScript.m_isItPaused1;
                break;
            case "2":
                IconScript.m_isItPaused2 = !IconScript.m_isItPaused2;
                break;
            case "3":
                IconScript.m_isItPaused3 = !IconScript.m_isItPaused3;
                break;
            case "4":
                IconScript.m_isItPaused4 = !IconScript.m_isItPaused4;
                break;
            case "5":
                TimeScript.m_sIsItPaused = !TimeScript.m_sIsItPaused;
                break;
            case "6":
                ArrowScript.m_isItPaused = !ArrowScript.m_isItPaused;
                break;
            case "7":
                CustButton.m_isItPaused = !CustButton.m_isItPaused;
                break;
            case "8":
                IconScript.m_isItPaused = !IconScript.m_isItPaused;
                break;
            case "9":
                Tooltips.m_isItPaused = !Tooltips.m_isItPaused;
                break;
            case "0":
                break;
        }
    }


    /// <summary>
    /// Update is called once per frame
    /// showing/hiding tooltips
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                Vector3 pos = Input.mousePosition;
                pos.z = m_tooltipTextObject.transform.position.z;

                foreach (Tooltip tip in _allTooltips)
                    if (tip.m_rect.Contains(pos))
                    {
                        _isShow = true;
                        _current = tip;
                        m_sIsSomeoneShow = true;
                        break;
                    }

                if (_isShow)
                {
                    if (_current.m_rect.Contains(pos))
                    {
                        if (_isTextChangable)
                            _current.m_text = m_tooltipText;
                        pos.z = -1;
                        pos.x += 10 + m_tooltipTextObject.rectTransform.rect.width / 2;
                        if (pos.x > Camera.main.pixelWidth)
                            pos.x -= m_tooltipTextObject.rectTransform.rect.width;
                        pos.y -= 25;
                        m_tooltipTextObject.transform.position = pos;

                        m_tooltipTextObject.text = _current.m_text;

                    }
                    else
                    {
                        _isShow = false;
                        _current = null;
                        m_sIsSomeoneShow = false;
                    }
                }

                if (!_isShow && !m_sIsSomeoneShow)
                    m_tooltipTextObject.transform.position = _outside;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Tooltips Update Exception" + ex.Message);
        }
    }
}
