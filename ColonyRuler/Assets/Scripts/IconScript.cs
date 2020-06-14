using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Icon script. 
/// Controller for AbstractObjects:
/// Can manage workers and make updates from players clicks for GameAbstractItems
/// Displaying all information for AbstractObjects
/// </summary>
[Serializable]
public class IconScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region debug checks
    public static bool m_isItPaused = false;
    public static bool m_isItPaused1 = false;
    public static bool m_isItPaused2 = false;
    public static bool m_isItPaused3 = false;
    public static bool m_isItPaused4 = false;
    #endregion
    /// <summary> link to main script </summary>
    public MainScript m_ms = null;
    /// <summary> link to people. For getting/sending back workers </summary>
    public People m_people = null;
    /// <summary> current item </summary>
    public AbstractObject m_thisItem = null;
    /// <summary> text for workers count </summary>
    public TextMesh m_workers;
    /// <summary> text for items count </summary>
    public TextMesh m_txtCount;
    /// <summary> text for productivity </summary>
    public TextMesh m_productText;
    /// <summary> text for damaged </summary>
    public TextMesh m_damagedText;

    #region scales
    /// <summary> storage count scale. Moved from left to right </summary>
    SpriteRenderer _storageCount;
    /// <summary> damaged count scale. Moved from left to right </summary>
    SpriteRenderer _damagedCount;
    /// <summary> productivity positive scale. Moved from center to right </summary>
    SpriteRenderer _productivityPlus;
    /// <summary> productivity negative scale. Moved from center to left </summary>
    SpriteRenderer _productivityMinus;
    /// <summary> constant. start x position for productivity scales </summary>
    const float XMiddlePositionProductivityPlus = 0.45f;
    /// <summary> start x position for count and damaged scales </summary>
    const float XStartPosition = -0.9f;
    /// <summary> y scale for all scales </summary>
    const float YScale = 0.2f;
    /// <summary> maximum length for all scales </summary>
    const float XMaxScale = 1.8f;
    #endregion
    /// <summary> all output arrows </summary>
    public List<ArrowScript> m_from = new List<ArrowScript>();
    /// <summary> all input arrows </summary>
    public List<ArrowScript> m_to = new List<ArrowScript>();
    /// <summary> all input tools arrows </summary>
    public List<ArrowScript> m_toolsTo = new List<ArrowScript>();
    /// <summary> all output tools arrows </summary>
    public List<ArrowScript> m_toolsFrom = new List<ArrowScript>();

    bool _isButtonPlusEnabled = false;
    bool _isButtonMinusEnabled = false;
    bool _isUpgradeButtonEnabled = false;
    /// <summary> Is icon pressed? used for moving icon </summary>
    bool _isPreesed = false;
    /// <summary> mouse position then icon start to move </summary>
    Vector3 _mousePos;
    /// <summary> for calculation and reducing allocations </summary>
    Vector3 _tmpPos = new Vector3(0, 0, 0);
    /// <summary> current rect of icon </summary>
    Rect _rect = new Rect(0, 0, 0, 0);

    /// <summary> used for detecting click and moving icon </summary>
    bool _isCursourInside = false;

    /// <summary> which icon is selected? </summary>
    static IconScript _sSelectedIcon = null;

    /// <summary> how long should be displayed production tree? </summary>
    public static bool m_sShowFullTree = false;

    public Button m_plus;
    public Button m_minus;
    public Button m_up;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            m_people = Camera.main.GetComponent<People>();
            if (m_thisItem != null)
            {
                TextMesh obj = GetComponentInChildren<TextMesh>();
                obj.text = m_thisItem.m_text;
            }
            CheckButtons();

            m_txtCount = transform.Find("ItemCount").GetComponent<TextMesh>();
            m_productText = transform.Find("Productivity").GetComponent<TextMesh>();
            _productivityMinus = transform.Find("ProdMinus").GetComponent<SpriteRenderer>();
            _productivityPlus = transform.Find("ProdPlus").GetComponent<SpriteRenderer>();
            _storageCount = transform.Find("Storage").GetComponent<SpriteRenderer>();
            _damagedCount = transform.Find("Damaged").GetComponent<SpriteRenderer>();
            SelectIcon();
        }
        catch (Exception ex)
        {
            Debug.LogError("IconScript Start exception:" + ex.Message);
        }

    }

    /// <summary>
    /// Make a place new IconScript for AbstractObject.
    /// If AbstractObject has his own coordinats != 1, the game will use them
    /// </summary>
    /// <param name="x"> coordinat of free space </param>
    /// <param name="y"> coordinat of free space </param>
    /// <param name="itm"> source </param>
    /// <returns> new IconScript object </returns>
    public static GameObject PlaceNewGameobject(float x, float y, AbstractObject itm)
    {
        MainScript ms = Camera.main.GetComponent<MainScript>();
        GameObject go = GameObject.Instantiate(ms.m_iconPrefab);
        go.name = itm.m_name;
        IconScript isc = go.GetComponent<IconScript>();
        isc.m_ms = ms;
        if (isc != null)
            isc.m_thisItem = itm;

        isc._tmpPos.z = go.transform.position.z;
        //if coordinates does not set in excel, it will be random
        if (itm.m_defaultX == 1 && itm.m_defaultY == 1)
        {
            isc._tmpPos.x = x;
            isc._tmpPos.y = y;
        }
        else
        {
            isc._tmpPos.x = itm.m_defaultX;
            isc._tmpPos.y = itm.m_defaultY;
        }
        go.transform.position = isc._tmpPos;

        isc.m_thisItem.m_thisObject = isc;
        isc.m_thisItem.ChangeProductionType(
            isc.transform.Find("smallIcon").GetComponent<SpriteRenderer>(), isc);
        MainScript.m_sAllItems.Add(go);

        return go;
    }

    /// <summary>
    /// Change all text data into current language
    /// </summary>
    public void ChangeLanguage()
    {
        TextMesh obj = GetComponentInChildren<TextMesh>();
        obj.text = m_thisItem.m_text;
    }

    /// <summary>
    /// Change upgrade button sprites. Active sprite and disabled sprite.
    /// </summary>
    /// <param name="objPath"> Path to Active sprite </param>
    /// <param name="disabledObjPath"> path to Disavbled sprite</param>
    /// <param name="enabled"> Is button enabled by default? </param>
    public void ChangeSprite(string objPath, string disabledObjPath, bool enabled = false)
    {
        Texture2D obj = Resources.Load(objPath) as Texture2D;
        Texture2D disabledObj = Resources.Load(disabledObjPath) as Texture2D;
        var ss = m_up.spriteState;
        _rect.width = obj.width;
        _rect.height = obj.height;
        _tmpPos.x = 0.5f;
        _tmpPos.y = 0.5f;
        Sprite enb = Sprite.Create(obj, _rect, _tmpPos, 100.0f);
        Sprite dsb = Sprite.Create(disabledObj, _rect, _tmpPos, 100.0f);
        ss.highlightedSprite = enb;
        ss.pressedSprite = enb;
        ss.selectedSprite = enb;
        ss.disabledSprite = dsb;
        m_up.spriteState = ss;
        m_up.image.sprite = enb;
        m_up.interactable = enabled;
    }

    /// <summary>
    /// Check + and - buttons conditions: should it be enabled or not?
    /// </summary>
    void CheckButtons()
    {
        if (m_people.WorkersNumber > 0 && m_thisItem.m_isItIterable && !_isButtonPlusEnabled)
        {
            m_plus.interactable = true;
            _isButtonPlusEnabled = true;
        }

        GameAbstractItem itm = m_thisItem as GameAbstractItem;

        if (itm != null && itm.m_workers > 0 && m_thisItem.m_isItIterable && !_isButtonMinusEnabled)
        {
            m_minus.interactable = true;
            _isButtonMinusEnabled = true;
        }
    }

    /// <summary>
    /// + and - buttons click event.
    /// </summary>
    /// <param name="workers"></param>
    public void ButtonClick(int workers)
    {
        if (workers > 0)
        {
            while (workers > 0)
            {
                workers--;
                ButtonPlusClick();
            }
        }
        else
        {
            while (workers < 0)
            {
                workers++;
                ButtonMinusClick();
            }
        }

    }

    /// <summary>
    /// Button + click function. Set only one worker
    /// </summary>
    public void ButtonPlusClick()
    {
        GameAbstractItem itm = m_thisItem as GameAbstractItem;

        if (itm != null && m_people.GetWorker())
        {
            itm.m_workers++;

            if (itm.m_workers == 1)
            {
                m_minus.interactable = true;
                _isButtonMinusEnabled = true;
            }

            m_workers.text = itm.m_workers.ToString();
        }
    }

    /// <summary>
    /// Button - function. Set only one worker
    /// </summary>
    public void ButtonMinusClick()
    {
        GameAbstractItem itm = m_thisItem as GameAbstractItem;
        if (itm != null)
        {
            itm.m_workers--;
            if (itm.m_workers < 0)
            {
                itm.m_workers = 0;
                return;
            }

            if ((itm.m_workers == 0 || !m_thisItem.m_isItIterable) && _isButtonMinusEnabled)
            {
                m_minus.interactable = false;
                _isButtonMinusEnabled = false;

            }

            m_workers.text = itm.m_workers.ToString();
            m_people.ReturnWorker();
        }
    }

    /// <summary>
    /// Upgrade button click.
    /// Called by Unity
    /// </summary>
    public void ButtonUpClick()
    {
        if (m_thisItem.Upgrade())
        {
            if (m_thisItem.GetType() == typeof(Science))
            {
                m_people.MakeBoost();


                GameAbstractItem itm = m_thisItem as GameAbstractItem;
                if (itm != null)
                    while (itm.m_workers > 0)
                    {
                        ButtonMinusClick();
                    }

                foreach (ArrowScript asc in m_from)
                {
                    asc.m_to.GetComponent<IconScript>().m_to.Remove(asc);
                    Destroy(asc.gameObject);
                }
                foreach (ArrowScript asc in m_to)
                {
                    asc.m_from.GetComponent<IconScript>().m_from.Remove(asc);
                    Destroy(asc.gameObject);
                }


                MainScript.m_sAllItems.Remove(gameObject);
                MainScript.m_sIsButtonPressed = false;
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Change z order for all arrows in the list
    /// </summary>
    /// <param name="ascl"> arrows list </param>
    /// <param name="z"> position z. Could be -200 or -1. -200 is invisible position </param>
    void MoveArrows(List<ArrowScript> ascl, float z)
    {
        foreach (ArrowScript asc in ascl)
        {
            Vector3 pos = asc.gameObject.transform.position;
            pos.z = z;
            asc.transform.position = pos;
        }
    }

    /// <summary>
    /// Show/hide full production tree for all arrows in arrow list
    /// </summary>
    /// <param name="ascl"> arrow list for moving </param>
    /// <param name="z"> z order </param>
    void MoveArrowsFull(List<ArrowScript> ascl, float z)
    {
        if (m_sShowFullTree)
        {
            try
            {
                foreach (ArrowScript asc in ascl)
                    if (asc.m_from != null && asc.m_from.m_to != null)
                        asc.m_from.MoveArrowsFull(asc.m_from.m_to, z);
            }
            catch (Exception ex)
            {
                Debug.Log(m_thisItem.m_name + " " + ex.Message);
            }
        }
        MoveArrows(ascl, z);
    }

    /// <summary>
    /// Select current icon.
    /// All arrows should be displayed
    /// </summary>
    void SelectIcon()
    {
        if (_sSelectedIcon != null)
            _sSelectedIcon.UnselectIcon();
        _sSelectedIcon = this;
        MoveArrows(m_from, -1f);
        MoveArrowsFull(m_to, -1f);
        MoveArrows(m_toolsTo, -1.1f);
        MoveArrows(m_toolsFrom, -1.1f);
    }

    /// <summary>
    /// unselect current icon.
    /// All arrows should be hidden
    /// </summary>
    void UnselectIcon()
    {
        MoveArrows(m_from, -200);
        MoveArrowsFull(m_to, -200);
        MoveArrows(m_toolsTo, -200);
        MoveArrows(m_toolsFrom, -200);
    }

    /// <summary>
    /// Used for changing production tree displayed type.
    /// </summary>
    public static void UnselectSelectedIcon()
    {
        if (_sSelectedIcon != null)
            _sSelectedIcon.UnselectIcon();
    }

    /// <summary>
    /// Used for returning to the game from main menu
    /// </summary>
    public static void SelectSelectedIcon()
    {
        if (_sSelectedIcon != null)
            _sSelectedIcon.SelectIcon();
    }

    /// <summary>
    /// Click to icon. While icon pressed, it could be moved
    /// </summary>
    void IconClick()
    {
        if (!MainScript.m_sIsButtonPressed)
        {
            MainScript.m_sIsButtonPressed = true;
            _mousePos = Input.mousePosition;
            _isPreesed = true;
            SelectIcon();
        }
        else
        {
            transform.position = transform.position +
                Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                Camera.main.ScreenToWorldPoint(_mousePos);
            _mousePos = Input.mousePosition;
        }

        foreach (ArrowScript sc in m_from)
            sc.MoveArrow();
        foreach (ArrowScript sc in m_to)
            sc.MoveArrow();
    }

    /// <summary>
    /// Move scale of count
    /// </summary>
    /// <param name="current"> current count </param>
    /// <param name="max"> max count </param>
    void MoveStorageScale(float current, float max)
    {
        MoveScale(current, max, _storageCount.gameObject);
    }

    /// <summary>
    /// Move a scale
    /// </summary>
    /// <param name="current"> current count </param>
    /// <param name="max"> max count </param>
    /// <param name="scaleObj"> scale object </param>
    void MoveScale(float current, float max, GameObject scaleObj)
    {
        float percent = current * 100 / max;
        if (percent > 100)
            percent = 100;
        else if (float.IsNaN(percent) || float.IsInfinity(percent))
            percent = 0;

        _tmpPos.z = 0;
        _tmpPos.x = XMaxScale * percent / 100;
        _tmpPos.y = YScale;

        scaleObj.transform.localScale = _tmpPos;

        float pos = XStartPosition + (-XStartPosition * percent / 100);
        _tmpPos = scaleObj.transform.localPosition;
        _tmpPos.x = pos;

        scaleObj.transform.localPosition = _tmpPos;
    }

    /// <summary>
    /// Move all scales
    /// </summary>
    void ScaleScales()
    {
        //split types
        if (m_thisItem.GetType() == typeof(Science))
        {
            Science sc = (Science)m_thisItem;
            _tmpPos.x = 0;
            _tmpPos.y = 0;
            _tmpPos.z = 0;
            _productivityMinus.transform.localScale = _tmpPos;
            MoveStorageScale(sc.Count, sc.m_maxCount);
            MoveScale(sc.m_productivity, 100, _productivityPlus.gameObject);
        }
        else
        {
            GameMaterial mat = m_thisItem as GameMaterial;
            if (mat != null)
            {
                MoveStorageScale(mat.Count * mat.m_size, mat.StorageSize);

                if (m_thisItem.m_isItDestroyable)
                {
                    Items itm = m_thisItem as Items;
                    MoveScale(itm.m_damagedCount * mat.m_size, mat.StorageSize, _damagedCount.gameObject);
                }


                float prodPer = 100 * 2 * mat.m_productivity / mat.StorageSize;
                if (prodPer > 100)
                    prodPer = 100;
                if (prodPer >= 0 && prodPer < 1f) prodPer = 1;
                if (prodPer < -100)
                    prodPer = -100;
                if (prodPer <= 0 && prodPer > -1f) prodPer = -1;
                if (float.IsNaN(prodPer))
                    prodPer = 1;
                float prodScale = /*two times shorter*/(XMaxScale / 2) * (prodPer / 100f);
                if (m_thisItem.m_productivity > 0)
                {
                    _tmpPos.x = 0;
                    _tmpPos.y = YScale;
                    _tmpPos.z = 0;
                    _productivityMinus.transform.localScale = _tmpPos;

                    _tmpPos.x = prodScale;
                    _productivityPlus.transform.localScale = _tmpPos;

                    _tmpPos = _productivityPlus.transform.localPosition;
                    _tmpPos.x = XMiddlePositionProductivityPlus * prodPer / 100f;

                    _productivityPlus.transform.localPosition = _tmpPos;

                }
                else
                {
                    _tmpPos.x = 0;
                    _tmpPos.y = YScale;
                    _tmpPos.z = 0;
                    _productivityPlus.transform.localScale = _tmpPos;

                    _tmpPos.x = prodScale;
                    _productivityMinus.transform.localScale = _tmpPos;
                    _tmpPos = _productivityMinus.transform.localPosition;
                    _tmpPos.x = XMiddlePositionProductivityPlus * prodPer / 100f;
                    _productivityMinus.transform.localPosition = _tmpPos;
                }
            }
        }
    }

    /// <summary>
    /// check mouse button conditions. Or screen touches
    /// </summary>
    void CheckInput()
    {
        if (Input.anyKey)
        {
            if (!CameraScript.m_isTwoKeyPressed)
            {
                Vector2 pos = transform.position;
                Vector2 mousePos = CameraScript.m_main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 shift = pos - mousePos;
                if (_isPreesed || shift.magnitude < (m_thisItem.m_isButtonsDisabled ? 0.9 : 2.0) && !MainScript.m_sIsButtonPressed)
                {
                    IconClick();
                }
            }
        }
        else
        {
            if (MainScript.m_sIsButtonPressed && _isPreesed)
            {
                MainScript.m_sIsButtonPressed = false;
                _isPreesed = false;
            }
        }
    }

    /// <summary>
    /// Enable/disable buttons
    /// </summary>
    void ChangeButtonsConditions()
    {
        if (!m_thisItem.m_isItIterable)
        {
            _isButtonPlusEnabled = false;
            m_plus.interactable = false;
        }
        else if (m_people != null)
            if (m_people.WorkersNumber == 0 && _isButtonPlusEnabled)
            {
                m_plus.interactable = false;
                _isButtonPlusEnabled = false;
            }
            else if (m_people.WorkersNumber > 0 && !_isButtonPlusEnabled)
            {
                _isButtonPlusEnabled = true;
                m_plus.interactable = true;
            }

        GameAbstractItem itm = m_thisItem as GameAbstractItem;
        if (itm != null && itm.m_workers == 0 && _isButtonMinusEnabled)
        {
            _isButtonMinusEnabled = false;
            m_minus.interactable = false;
        }

        if (m_thisItem.CheckUpgradeConditions() && m_thisItem.m_isItIterable)
        {
            if (!_isUpgradeButtonEnabled)
                m_up.interactable = true;
            _isUpgradeButtonEnabled = true;
        }
        else
        {
            if (_isUpgradeButtonEnabled)
                m_up.interactable = false;
            _isUpgradeButtonEnabled = false;
        }
    }

    /// <summary>
    /// Numbers outputs
    /// </summary>
    void PrintTextValue()
    {
        m_txtCount.text = m_thisItem.GetCountString();
        GameAbstractItem itm = m_thisItem as GameAbstractItem;
        if (itm != null && m_txtCount != null)
        {
            m_workers.text = itm.m_workers.ToString();
            m_productText.text = m_thisItem.GetProductivityString();
            if (m_thisItem.m_isItDestroyable)
                m_damagedText.text = ((Items)m_thisItem).GetDamagedString();
        }
    }

    /// <summary>
    /// Then mouse is hovered on scales, tooltips should be shown
    /// </summary>
    void ShowTooltips()
    {
        var cursorPosition = Input.mousePosition;
        cursorPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

        bool showTooltip = false;
        string tooltip = "";
        cursorPosition -= transform.position;
        if (cursorPosition.y < -1.2 || cursorPosition.x > 1 || cursorPosition.x < -1)
        {
            showTooltip = false;
            Tooltips.m_sIsSomeoneShow = false;
        }
        else if (cursorPosition.y < -0.963)
        {
            tooltip = m_thisItem.m_tooltipDamaged;
            showTooltip = true;
            Tooltips.m_sIsSomeoneShow = true;
        }
        else if (cursorPosition.y < -0.775)
        {
            tooltip = m_thisItem.m_tooltipProductivity;
            showTooltip = true;
            Tooltips.m_sIsSomeoneShow = true;
        }
        else if (cursorPosition.y < -0.5)
        {
            tooltip = m_thisItem.m_tooltipCount;
            showTooltip = true;
            Tooltips.m_sIsSomeoneShow = true;
        }

        Vector3 toolTipPos = Input.mousePosition;
        toolTipPos.x += 10 + m_ms.m_toolTipText.rectTransform.rect.width / 2; ;
        toolTipPos.y -= 25;
        toolTipPos.z = m_ms.m_toolTipText.transform.position.z;

        if (showTooltip)
        {
            m_ms.m_toolTipText.transform.position = toolTipPos;
            m_ms.m_toolTipText.text = tooltip;
        }
        else
        {
            //ms.m_ToolTipText.transform.position = new Vector3(0, -100);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                if (m_thisItem.CheckUpgradeConditions() && !_isUpgradeButtonEnabled)
                {
                    m_up.interactable = true;
                    _isUpgradeButtonEnabled = true;
                }
                if (!m_isItPaused1)
                    ChangeButtonsConditions();
                if (!m_isItPaused2)
                    PrintTextValue();
                if (!m_isItPaused3)
                    ScaleScales();
                if (!m_isItPaused4)
                    CheckInput();
                if (_isCursourInside)
                    ShowTooltips();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("IconScript Update Exception:" + ex.Message);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isCursourInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tmpPos.x = 0;
        _tmpPos.y = -100;
        _isCursourInside = false;
        m_ms.m_toolTipText.transform.position = _tmpPos;
    }
}
