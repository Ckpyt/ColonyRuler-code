using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class for making learning tips.
/// </summary>
class LearningTip: MonoBehaviour
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

    static Dictionary<string, LearningTipData> s_allTips;

    /// <summary> if a player does not want to see tooltips, they should not be shown</summary>
    public static bool s_canShow = true;

    public static string s_lastTip = "";

    LearningTipData m_data;

    /// <summary> for preventing double-clicks </summary>
    bool isItUnclicked = true;
    /// <summary> target object for showing yellow outline. Could be null </summary>
    GameObject m_targetObject = null;
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
        s_allTips = new Dictionary<string, LearningTipData>();

        var uparsedTips = AbstractObject.Load<ExcelLoading.AllTips>(filename);
        foreach(var source in uparsedTips.repetative)
        {
            var tipData = new LearningTipData();
            tipData.m_dependency = source.targetIcon;
            tipData.m_targetObject = source.targetObject;
            tipData.m_position = new Vector2(source.defaultX, source.defaultY);
            tipData.m_name = source.name;
            tipData.m_text = source.description;
            tipData.m_next = source.nextTip;
            tipData.m_isItUI = source.isItUI;

            string[] positionVecor = source.yellowPositionVector == null ? new string[]{""} : source.yellowPositionVector.Split(';');
            string[] sizeVector = source.yellowSizeVector == null ? new string[] { "" } : source.yellowSizeVector.Split(';');
            if (positionVecor.Length > 1 && sizeVector.Length > 1)
                tipData.m_yellowBox = new Rect(
                    AbstractObject.FloatParse(positionVecor[0]),
                    AbstractObject.FloatParse(positionVecor[1]),
                    AbstractObject.FloatParse(sizeVector[0]),
                    AbstractObject.FloatParse(sizeVector[1])
                    );
            s_allTips.Add(tipData.m_name, tipData);
        }

        //making previous link
        foreach(var tipPair in s_allTips)
        {
            var tip = tipPair.Value;
            if (tip.m_next != null && tip.m_next.Length > 0)
            {
                var nextTip = s_allTips[tip.m_next];
                if (nextTip.m_previous == null || nextTip.m_previous.Length == 0)
                    s_allTips[tip.m_next].m_previous = tip.m_name;
                else
                    throw new Exception("tip '" + nextTip + "' has previous tip:" + nextTip.m_previous);

                
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

    static void SetTargetObject(LearningTip obj, string targetObjectName, Transform parent)
    {
        var path = targetObjectName.Split('\\');
        Transform nextObj = parent.Find(path[0]);

        if (path.Length > 1) {
            string newPath = targetObjectName.Split(new[] { '\\' }, 2)[1].Substring(1);
            SetTargetObject(obj, newPath, nextObj ?? parent);
        }
        else
            obj.m_targetObject = nextObj.gameObject;

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
        s_canShow = tg.isOn;

    }

    /// <summary>
    /// Create a tip in the game
    /// </summary>
    /// <param name="name">tip uniquie name</param>
    /// <param name="forseShow">is it should be shown without the "can show" param?</param>
    public static void CreateTip(string name, bool forseShow = false)
    {
        if (s_canShow == false && forseShow == false) return;

        LearningTipData data;
        MainScript ms = Camera.main.GetComponent<MainScript>();

        try
        {
            data = s_allTips[name];
        }
        catch ( KeyNotFoundException)
        {
            Debug.LogError("Tip " + name + " set as the next, but was not found");
            return;
        }

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
            coord += depend.m_thisObject.transform.position;

        coord.z = -3; //learning tips should be higher then icons;

        var LearningTipObj = Instantiate(Camera.main.GetComponent<MainScript>().m_LearingTipPrefab);
        LearningTipObj.name = "Learning Tip: " + data.m_name;
        LearningTipObj.transform.position = coord;

        coord.z = Camera.main.transform.position.z;
        Camera.main.transform.position = coord;

        LearningTip thisObj = LearningTipObj.GetComponent<LearningTip>();
        thisObj.m_data = data;

        var Canvas = LearningTipObj.transform.Find("Canvas tips");

        var txtObj = Canvas.Find("TipText").gameObject;
        var txt = txtObj.GetComponent<TextMeshProUGUI>();
        txt.text = data.m_text;
        

        SetButton(Canvas, "PrevButton", thisObj.OnPrevious, data.m_previous != null);
        SetButton(Canvas, "NextButton", thisObj.OnNext, data.m_next != null && data.m_next.Length > 0);
        SetButton(Canvas, "OkButton",   thisObj.OnClose, !(data.m_next != null && data.m_next.Length > 0));

        var toggle = thisObj.m_showTips.GetComponent<Toggle>();
        toggle.isOn = s_canShow;
        toggle.onValueChanged.AddListener(delegate { thisObj.OnShowTip(); });
        

        if (depend != null || dependency != null)
        {
            //TODO:yellow outline
            if (depend != null)
                SetTargetObject(thisObj, data.m_targetObject, depend.m_thisObject.transform);
            else
                thisObj.m_targetObject = dependency;

            Rect rectB = new Rect();

            if (data.m_yellowBox != null && data.m_yellowBox.width > 0)
            {
                rectB = data.m_yellowBox;
                rectB.center += (Vector2)thisObj.m_targetObject.transform.position;
            }
            else
            {
                if (data.m_isItUI)
                {
                    var rect = thisObj.m_targetObject.transform as RectTransform;
                    rectB = rect.rect;
                    rectB.center = (Vector2)thisObj.m_targetObject.transform.position;
                }
                else
                {
                    var render = thisObj.m_targetObject.GetComponent<Renderer>();
                    rectB.size = (Vector2)render.bounds.size;//* render.transform.localScale;
                    rectB.center = render.bounds.center;
                }
            }

            data.m_outline = Instantiate(ms.m_OutlinePrefab).GetComponent<Outline>();
            data.m_outline.transform.position = new Vector3();
            data.m_outline.m_OutlineRect = rectB;
            data.m_outline.mb_IsItCanvas = thisObj.m_data.m_isItUI;
        }
        //var outline = thisObj.m_targetObject.AddComponent<Outline>();
        //outline.OutlineMode = Outline.Mode.OutlineAll;
        //outline.OutlineColor = Color.yellow;
        //outline.OutlineWidth = 5f;
    }


    public void OnClose()
    {
    
        if (m_data.m_outline != null)
        {
            Destroy(m_data.m_outline.gameObject);
            Destroy(m_targetObject.GetComponent<Outline>());
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Next learning tip
    /// </summary>
    public void OnNext()
    {
        if (isItUnclicked)
        {
            CreateTip(m_data.m_next, true);
            OnClose();
            isItUnclicked = false;
        }
    }

    /// <summary>
    /// previous learning tip
    /// </summary>
    public void OnPrevious()
    {
        if (isItUnclicked)
        {
            CreateTip(m_data.m_previous, true);
            OnClose();
            isItUnclicked = false;
        }
    }
}

