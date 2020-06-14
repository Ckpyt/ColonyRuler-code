using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class for showing how happy population is.
/// </summary>
public class Emoji : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary> then the game is paused, it should not works</summary>
    public static bool m_isItPaused = false;
    /// <summary> current tooltips. </summary>
    Tooltips _tps;
    /// <summary> then mouse inside in the icon, tooltips should be shown</summary>
    bool _isMouseInside = false;
    /// <summary> string for localization. Should be changed into current language</summary>
    string _happy = "Happy";
    /// <summary> string for localization. Should be changed into current language</summary>
    string _maxHappy = "max Happy";
    /// <summary> string for localization. Should be changed into current language</summary>
    string _population = "max Population";

    /// <summary> Text object for localization. Value should be changed into current language</summary>
    public Text m_populationObject;
    /// <summary> Text object for localization. Value should be changed into current language</summary>
    public Text m_freeWorkers;

    /// <summary> Link to population class </summary>
    People _people = null;

    /// <summary>
    /// Method for change text object then current languge has changed
    /// Should be linked into Localization.OnLanguageChanged event
    /// </summary>
    void ChangeLanguage()
    {
        Localization loc = Localization.GetLocalization();
        _happy = loc.m_ui.m_emojiHappy;
        _maxHappy = loc.m_ui.m_emojiMaxHappy;
        _population = loc.m_ui.m_emojiMaxPopulation;
        m_populationObject.text = loc.m_ui.m_populationText;
        m_freeWorkers.text = loc.m_ui.m_freeWorkersText;
    }

    /// <summary>
    /// Event. Mouse enetered inside in the emoji.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseInside = true;
    }

    /// <summary>
    /// Event. Mouse has left the emoji.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseInside = false;
    }

    /// <summary>
    /// GameObject destroyed. Called by Unity
    /// </summary>
    public void OnDestroy()
    {
        Localization.m_onLanguageChanged -= ChangeLanguage;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        try { 
            _tps = GetComponent<Tooltips>();
            _tps.m_tooltipText = "training";
            Localization.m_onLanguageChanged += ChangeLanguage;
            ChangeLanguage();
        }
        catch (Exception ex)
        {
            Debug.LogError("Emoji Start exception" + ex.Message);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// Just for making happy tooltip.
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                if (_isMouseInside)
                {
                    if (_people == null)
                        _people = Camera.main.GetComponent<People>();

                    string text;
                    text = _happy + _people.m_happy.ToString();
                    text += " " + _maxHappy + _people.m_maxHappy;
                    text += " " + _population + _people.CalcMaxPopulation();
                    _tps.m_tooltipText = text;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Emoji Update Exception" + ex.Message);
        }
    }
}
