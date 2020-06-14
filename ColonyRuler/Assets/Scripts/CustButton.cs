using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Botton for icon.
/// Increase multiplier twice per deltatime pressed
/// </summary>
public class CustButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary> if the game is paused, buttons should not works </summary>
    public static bool m_isItPaused = false;
    /// <summary> delta time for click </summary>
    const float CDeltaTime = 0.1f;
    /// <summary> delta time for incrising multipier </summary>
    const float CDeltaChangeTime = 0.5f;

    /// <summary> time of beginning current click </summary>
    float _timeBegin = 0;
    /// <summary> time of beginning current increasing cycle </summary>
    float _changeTime = 0;
    /// <summary> is button pressed? </summary>
    bool _isItPressed = false;
    /// <summary> button click event </summary>
    public delegate void OnMyClick();
    public OnMyClick m_onEvent;
    /// <summary> Is it plus or minus workers button </summary>
    public bool m_isItPlus;
    /// <summary> current multiplier </summary>
    int _multiplier = 1;
    /// <summary> link to icon script of this button </summary>
    IconScript _iconScript;

    /// <summary>
    /// Click strated event
    /// Called by Unity
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _timeBegin = Time.time;
        _changeTime = Time.time;
        _isItPressed = true;
        _multiplier = m_isItPlus ? 1 : -1;
    }

    /// <summary>
    /// Click finished event
    /// Called by Unity
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        _isItPressed = false;
    }



    /// <summary>
    ///  Start is called before the first frame update.
    ///  Called by Unity. In multi-threds systems, could be after first frame update.
    /// </summary>
    void Start()
    {
        try
        {
            _iconScript = gameObject.transform.parent.parent.gameObject.GetComponent<IconScript>();
        }
        catch (Exception ex)
        {
            Debug.LogError("CustButton Start exception:" + ex.Message);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// If button pressed, sends click events every c_deltaTime
    /// and icrease multiplier twice per c_deltaChangeTime
    /// </summary>
    void Update()
    {
        try { 
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                if (_isItPressed)
                {
                    float time = Time.time;
                    if (time - CDeltaTime > _timeBegin)
                    {
                        _timeBegin = time;
                        _iconScript.ButtonClick(_multiplier);
                    }

                    if (time - CDeltaChangeTime > _changeTime)
                    {
                        _multiplier *= 2;
                        _changeTime = time;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("CustButton Update exception:"  + ex.Message);
        }
    }
}
