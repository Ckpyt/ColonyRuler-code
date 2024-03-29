﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class for making learning tips.
/// </summary>
class LearningTip : MonoBehaviour
{
    [Serializable]
    class LearningTipData
    {
        public string m_name;
        public bool m_isItShown = false;

        [NonSerialized]
        public string m_text;
        [NonSerialized]
        public string m_next = null;
        [NonSerialized]
        public string m_previous = null;
        [NonSerialized]
        public Vector2 m_position;
        [NonSerialized]
        public string m_dependency;
        [NonSerialized]
        public string m_targetObject;
        [NonSerialized]
        public Rect m_yellowBox;
        [NonSerialized]
        public Outline m_outline = null;
        [NonSerialized]
        public bool m_isItUI;
    }

    static Dictionary<string, LearningTipData> _sAllTips;
    /// <summary> the name of the next tips after closing current one</summary>
    static List<string> _sNextTips = new List<string>();
    static LearningTip _sCurrentTip = null;

    /// <summary> property for checking is a learning tip shown or not </summary>
    public static bool m_sIsSomethingShown { get { return !(_sCurrentTip == null); } }

    /// <summary> if a player does not want to see tooltips, they should not be shown</summary>
    public static bool m_sCanShow = true;
    /// <summary> name of the last tip</summary>
    public static string m_sLastTip = "";

    LearningTipData _data;

    /// <summary> for preventing double-clicks </summary>
    bool _isItUnclicked = true;

    /// <summary> target object for showing yellow outline. Could be null </summary>
    GameObject _targetObject = null;

    /// <summary> Toggle for preventing showing tips </summary>
    public GameObject m_showTips = null;

    /// <summary>
    /// Called from Unity.
    /// </summary>
    public void Start()
    {

    }

    /// <summary>
    /// Loading all tooltips from xml file
    /// </summary>
    public static void Load(string filename)
    {
        _sAllTips = new Dictionary<string, LearningTipData>();

        var uparsedTips = AbstractObject.Load<ExcelLoading.AllTips>(filename);
        foreach (var source in uparsedTips.repetative)
        {
            var tipData = new LearningTipData();
            tipData.m_dependency = source.targetIcon;
            tipData.m_targetObject = source.targetObject;
            tipData.m_position = new Vector2(source.defaultX, source.defaultY);
            tipData.m_name = source.name;
            tipData.m_text = source.description;
            tipData.m_next = source.nextTip;
            tipData.m_isItUI = source.isItUI;

            string[] positionVecor = source.yellowPositionVector == null ? new string[] { "" } : source.yellowPositionVector.Split(';');
            string[] sizeVector = source.yellowSizeVector == null ? new string[] { "" } : source.yellowSizeVector.Split(';');
            if (positionVecor.Length > 1 && sizeVector.Length > 1)
                tipData.m_yellowBox = new Rect(
                    AbstractObject.FloatParse(positionVecor[0]),
                    AbstractObject.FloatParse(positionVecor[1]),
                    AbstractObject.FloatParse(sizeVector[0]),
                    AbstractObject.FloatParse(sizeVector[1])
                    );
            _sAllTips.Add(tipData.m_name, tipData);
        }

        //making previous link
        foreach (var tipPair in _sAllTips)
        {
            var tip = tipPair.Value;
            if (tip.m_next != null && tip.m_next.Length > 0)
            {
                try
                {
                    var nextTip = _sAllTips[tip.m_next];
                    if (nextTip.m_previous == null || nextTip.m_previous.Length == 0)
                        _sAllTips[tip.m_next].m_previous = tip.m_name;
                    else
                        throw new Exception("tip '" + nextTip + "' has previous tip:" + nextTip.m_previous);
                }
                catch (Exception)
                {
                    Debug.Log("Key not found" + tip.m_name);
                }

            }
        }
    }

    static void SetButton(Transform canvas, string name, UnityAction Event, bool active)
    {
        var obj = canvas.Find(name).gameObject;
        var button = obj.GetComponent<Button>();
        button.onClick.AddListener(Event);
        obj.SetActive(active);
    }

    static bool SetTargetObject(LearningTip obj, string targetObjectName, Transform parent)
    {
        var path = targetObjectName.Split('\\');
        Transform nextObj = parent.Find(path[0]);

        if (path.Length > 1)
        {
            string newPath = targetObjectName.Split(new[] { '\\' }, 2)[1];
            SetTargetObject(obj, newPath, nextObj ?? parent);
        }
        else
            obj._targetObject = nextObj.gameObject;

        return path[0].Contains("Canvas");

    }

    /// <summary>
    /// Does not need to show already shown tips
    /// </summary>
    public static void ShowOldTip()
    {

    }

    /// <summary>
    /// Toggle "Show Tip" Event on Learining Tip panel
    /// </summary>
    public void OnShowTip()
    {
        var tg = m_showTips.GetComponent<Toggle>();
        m_sCanShow = tg.isOn;

    }

    /// <summary>
    /// Create a tip in the game
    /// </summary>
    /// <param name="name">tip uniquie name</param>
    /// <param name="forseShow">is it should be shown without the "can show" param?</param>
    public static void CreateTip(string name, bool forseShow = false)
    {
        if ((m_sCanShow == false && forseShow == false) || name.Length == 0) return;

        if (_sCurrentTip != null)
        {
            if (_sCurrentTip._data.m_name == name) 
                return;
            if (_sCurrentTip._data.m_name.CompareTo(name) > 0)
            {
                var curData = _sCurrentTip._data;
                _sNextTips.Add(curData.m_name);
                _sCurrentTip.OnClose(true);
                curData.m_isItShown = false;
            }
            else
            {
                _sNextTips.Add(name);
                return;
            }
        }

        LearningTipData data;
        MainScript ms = Camera.main.GetComponent<MainScript>();

        try
        {

            data = _sAllTips[name];
        }
        catch (KeyNotFoundException ex)
        {
            Debug.LogError("Tip " + name + " set as the next, but was not found:" + ex.Message);
            return;
        }

        if (data.m_isItShown && forseShow == false) return;

        AbstractObject depend = null;
        GameObject dependency = null;
        if (data.m_dependency != null && data.m_dependency.Length > 0)
        {
            depend = AbstractObject.GetByName(data.m_dependency);
            if (depend == null)
                dependency = GameObject.Find(data.m_dependency);
        }

        Vector3 coord = data.m_position;
        if (depend != null)
        {
            coord += depend.m_thisObject.transform.position;
            depend.m_thisObject.SelectIcon();
        }

        coord.z = -3; //learning tips should be higher then icons;

        var LearningTipObj = Instantiate(Camera.main.GetComponent<MainScript>().m_LearingTipPrefab);
        LearningTipObj.name = "Learning Tip: " + data.m_name;
        LearningTipObj.transform.position = coord;

        coord.z = Camera.main.transform.position.z;
        Camera.main.transform.position = coord;

        LearningTip thisObj = LearningTipObj.GetComponent<LearningTip>();
        thisObj._data = data;

        var Canvas = LearningTipObj.transform.Find("Canvas tips");

        var txtObj = Canvas.Find("TipText").gameObject;
        var txt = txtObj.GetComponent<TextMeshProUGUI>();
        txt.text = data.m_text;

        bool showNext = ((data.m_next != null && data.m_next.Length > 0) || _sNextTips.Count > 0);
        SetButton(Canvas, "PrevButton", thisObj.OnPrevious, data.m_previous != null);
        SetButton(Canvas, "NextButton", thisObj.OnNext, showNext);
        SetButton(Canvas, "OkButton", thisObj.OnClose, !(showNext));

        var toggle = thisObj.m_showTips.GetComponent<Toggle>();
        toggle.isOn = m_sCanShow;
        toggle.onValueChanged.AddListener(delegate { thisObj.OnShowTip(); });


        if (depend != null || dependency != null)
        {
            bool hasCanvas = false;

            if (depend != null)
                hasCanvas = SetTargetObject(thisObj, data.m_targetObject, depend.m_thisObject.transform);
            else
                thisObj._targetObject = dependency;

            Rect rectB = new Rect();

            if (data.m_yellowBox != null && data.m_yellowBox.width > 0)
            {
                rectB = data.m_yellowBox;
                rectB.center += (Vector2)thisObj._targetObject.transform.position;
            }
            else
            {
                if (data.m_isItUI || hasCanvas)
                {
                    var rect = thisObj._targetObject.transform as RectTransform;
                    rectB = rect.rect;
                    rectB.center = thisObj._targetObject.transform.position;

                }
                else
                {
                    var render = thisObj._targetObject.GetComponent<Renderer>();
                    rectB.size = render.bounds.size;
                    rectB.center = render.bounds.center;
                }
            }

            data.m_outline = Instantiate(ms.m_OutlinePrefab).GetComponent<Outline>();
            data.m_outline.transform.position = new Vector3();
            data.m_outline.m_OutlineRect = rectB;
            data.m_outline.mb_IsItCanvas = thisObj._data.m_isItUI;
        }

        _sCurrentTip = thisObj;
        _sNextTips.Remove(data.m_name);
    }

    /// <summary> the Unity delegate for OnClose event </summary>
    public void OnClose() { OnClose(false); }

    /// <summary>
    /// Realization of closing event
    /// </summary>
    /// <param name="showNext">should be sshown the next tip from the queue?</param>
    public void OnClose(bool showNext = true)
    {
        _sCurrentTip = null;
        _data.m_isItShown = true;

        if (_sNextTips.Count > 0 && !showNext)
        {
            _sNextTips.Clear();
        }

        if (_data.m_outline != null)
        {
            Destroy(_data.m_outline.gameObject);
            //Destroy(_targetObject.GetComponent<Outline>());
        }
        gameObject.SetActive(false);

        Destroy(gameObject);
    }

    /// <summary>
    /// Next learning tip
    /// </summary>
    public void OnNext()
    {
        if (_isItUnclicked)
        {
            OnClose(true);
            if(_data.m_next != null && _data.m_next.Length > 0)
                CreateTip(_data.m_next, true);
            else
            {
                _sNextTips.Sort();
                string name = _sNextTips[0];
                _sNextTips.Remove(name);
                CreateTip(name);
            }
            _isItUnclicked = false;
        }
    }

    /// <summary>
    /// previous learning tip
    /// </summary>
    public void OnPrevious()
    {
        if (_isItUnclicked)
        {
            OnClose(true);
            CreateTip(_data.m_previous, true);
            _isItUnclicked = false;
        }
    }
}

