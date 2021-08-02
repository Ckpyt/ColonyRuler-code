using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Making arrows from one IconScript object to another.
/// Displaying production tree.
/// </summary>
[Serializable]
public class ArrowScript : MonoBehaviour
{
    /// <summary> if the game is paused, arrows should not move </summary>
    public static bool m_isItPaused = false;
    /// <summary> start game object </summary>
    public IconScript m_from = null;
    /// <summary> end game object. Could be the same as m_from </summary>
    public IconScript m_to = null;
    /// <summary> start coordinates </summary>
    Vector3 _fromCoord = new Vector2(0, 0);
    /// <summary> end coordinates </summary>
    Vector3 _toCoord = new Vector2(0, 0);
    /// <summary> the arrow's root body </summary>
    GameObject _body;
    /// <summary> Tools should have another color </summary>
    public bool m_isItTool = false;
    /// <summary> Tools could have the same end and start coordinates </summary>
    bool _isItHimself = false;
    /// <summary> Red color, indicates m_productivity in m_from is below zero </summary>
    public static Color _sArrowMaterialsNotEnough = new Color(1, 0, 0);
    /// <summary> Green color, indicates normal production </summary>
    public static Color _sArrowMaterialsEnough = new Color(0, 1, 0);
    /// <summary> White color, indicates where is no production </summary>
    public static Color _sArrowMaterialsNotUsed = new Color(1, 1, 1);
    /// <summary> Blue color, indicate tools arrow </summary>
    public static Color _sArrowToolsNotEnough = new Color(1, 1, 0);
    /// <summary> Yellow color, indicate where is not enough tools </summary>
    public static Color _sArrowToolsEnough = new Color(0, 0.727f, 1);
    /// <summary> all arrows in the game </summary>
    static List<GameObject> _sAllArrows = new List<GameObject>();
    /// <summary> Arrow render </summary>
    SpriteRenderer _bodyRender = null;
    /// <summary> Arrow body render </summary>
    SpriteRenderer _bodyRender2 = null;
    /// <summary> Ealer rotation </summary>
    Vector3 _ealer = new Vector3(0, 0, 0);
    /// <summary> magnitude between start and end coordinates </summary>
    Vector3 _magnitude = new Vector3(0, 0, 0);
    /// <summary> Quaternion rotation </summary>
    Quaternion _quaternion = new Quaternion();

    /// <summary> Corners of start/end icon </summary>
    Vector3[] _corners = new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
    /// <summary> intersections with borders of start / end icons </summary>
    Vector3[] _cross = new Vector3[4];

    SpriteRenderer _arrowBody2 = null;
    SpriteRenderer _arrowBody3 = null;

    const float XMaxCorner = 2.2f;
    const float XMinCorner = 1.0f;
    const float YMaxCorner = 2.4f;
    const float YMinCorner = 1.3f;

    // diffrence between from coords and to coords
    const float XDif = 0.3f;

    /// <summary>
    /// First frame initialization
    /// </summary>
    void Start()
    {
        try
        {
            _body = transform.Find("ArrowBody").gameObject;
            _sAllArrows.Add(this.gameObject);
            _bodyRender = gameObject.GetComponent<SpriteRenderer>();
            _bodyRender2 = _body.GetComponent<SpriteRenderer>();
        }
        catch (Exception ex)
        {
            Debug.LogError("ArrowScript Start exception:" + ex.Message);
        }
    }


    /// <summary>
    /// Destroy all stored arrows.
    /// Called at the end of the game
    /// </summary>
    public static void DestroyAllObjects()
    {
        foreach (GameObject go in _sAllArrows)
            DestroyImmediate(go);
        _sAllArrows.Clear();
    }

    /// <summary>
    /// Vectors length, where start point has 0,0 coordinates
    /// </summary>
    /// <param name="vec"> finish point </param>
    /// <returns></returns>
    float Lenght(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
    }

    /// <summary>
    /// Used for checking non-value numbers and replace it to finitive value
    /// </summary>
    /// <param name="somecoord"> finintive coordinates</param>
    /// <returns></returns>
    static float CheckInfinity(float check, float somecoord)
    {
        if (float.IsNaN(check) || float.IsInfinity(check))
            check = somecoord + 10000f;
        return check;
    }

    /// <summary>
    /// Calc interception with two lines.
    /// </summary>
    /// <param name="first"> start first line </param>
    /// <param name="second"> finish first line </param>
    /// <param name="firstCor"> strart second line </param>
    /// <param name="secondCor">finish second line</param>
    /// <returns> interception </returns>
    Vector3 CalcCrossVec(Vector3 first, Vector3 second, Vector3 firstCor, Vector3 secondCor)
    {
        var answ = _magnitude;
        answ.z = 0;
        var del = ((firstCor.x - secondCor.x) * (first.y - second.y) -
            (firstCor.y - secondCor.y) * (first.x - second.x));

        answ.x = ((firstCor.x * secondCor.y - firstCor.y * secondCor.x) * (first.x - second.x) -
            (firstCor.x - secondCor.x) * (first.x * second.y - first.y * second.x)) / del;

        answ.y = ((firstCor.x * secondCor.y - firstCor.y * secondCor.x) * (first.y - second.y) -
            (firstCor.y - secondCor.y) * (first.x * second.y - first.y * second.x)) / del;

        answ.x = CheckInfinity(answ.x, second.x);
        answ.y = CheckInfinity(answ.y, second.y);

        return answ;
    }

    /// <summary>
    /// Calculate the closest intersection between the icon and the line
    /// coming from one center and ending at another center of the icons
    /// </summary>
    /// <param name="first"> first icon's center </param>
    /// <param name="second"> second icon's center </param>
    /// <param name="isButtonsDisabled"> if buttons disabled, icons will be smaller </param>
    /// <param name="dif"> differnce of arrow's head </param>
    Vector3 CalcCross(Vector3 first, Vector3 second, bool isButtonsDisabled, float dif = 0)
    {
        float x1 = (isButtonsDisabled ? XMinCorner : XMaxCorner) + dif;
        float x2 = -(isButtonsDisabled ? XMinCorner : XMaxCorner) - dif;
        float y1 = YMinCorner + dif;
        float y2 = -(isButtonsDisabled ? YMinCorner : YMaxCorner) - dif;
        float min = y1 < y2 ? y1 : y2;
        float max = y1 > y2 ? y1 : y2;
        float len = (max - min) / 2;
        float centerY = min + len;
        Vector3 center = first;
        center.y = first.y + centerY;
        y1 = +len;
        y2 = -len;

        _corners[0].x = x1;
        _corners[0].y = y1;
        _corners[1].x = x1;
        _corners[1].y = y2;
        _corners[2].x = x2;
        _corners[2].y = y2;
        _corners[3].x = x2;
        _corners[3].y = y1;

        for (int i = 0; i < 4; i++)
        {
            _corners[i] = center + _corners[i];
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 firstCor = _corners[i];
            Vector3 secondCor = i == 3 ? _corners[0] : _corners[i + 1];

            _cross[i] = CalcCrossVec(firstCor, secondCor, center, second);

        }
        int closestInd1 = Lenght(_cross[0] - center) > Lenght(_cross[1] - center) ? 1 : 0;
        int closestInd2 = Lenght(_cross[2] - center) > Lenght(_cross[3] - center) ? 3 : 2;
        int ind = Lenght(_cross[closestInd1] - second) > Lenght(_cross[closestInd2] - second) ?
            closestInd2 : closestInd1;

        Vector3 answ = CalcCrossVec(_corners[ind], _corners[ind == 3 ? 0 : ind + 1],
            center, second);

        return answ;
    }

    /// <summary>
    /// Place new arrow
    /// </summary>
    /// <param name="from"> source </param>
    /// <param name="to"> goal </param>
    /// <returns> new arrow object </returns>
    public static ArrowScript NewArrowScript(IconScript from, IconScript to)
    {
        try
        {
            MainScript ms = Camera.main.GetComponent<MainScript>();
            GameObject go = GameObject.Instantiate(ms.m_arrowPrefab);
            ArrowScript asc = go.GetComponent<ArrowScript>();
            asc.m_to = to;
            asc.m_from = from;
            return asc;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    /// <summary>
    /// place arrow and save it in IconScript
    /// tools should not been stored
    /// </summary>
    /// <param name="from"> source </param>
    /// <param name="to"> goal </param>
    /// <returns> new arrow object </returns>
    public static ArrowScript NewStoredArrowScript(IconScript from, IconScript to)
    {
        ArrowScript asc = NewArrowScript(from, to);
        try
        {
            from.m_from.Add(asc);
            to.m_to.Add(asc);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return asc;
    }

    /// <summary>
    /// Make arrow, where start and finish objects is the same object
    /// </summary>
    public void MoveHimself()
    {
        if (!_isItHimself)
        {
            _isItHimself = true;
            GameObject body2 = Instantiate(_body);
            GameObject body3 = Instantiate(_body);
            body2.transform.parent = this.transform;
            body3.transform.parent = this.transform;
            body2.transform.localPosition = new Vector2(-1.54f, -0.59f);
            body3.transform.localPosition = new Vector2(-0.58f, -1.14f);
            body2.transform.localScale = new Vector3(1.51f, 1f, 1f);
            body3.transform.localScale = new Vector3(1.51f, 1f, 1f);
            body2.transform.localEulerAngles = new Vector3(0, 0, 90);
            _arrowBody2 = body2.GetComponent<SpriteRenderer>();
            _arrowBody2.color = _sArrowToolsEnough;
            _arrowBody3 = body3.GetComponent<SpriteRenderer>();
            _arrowBody3.color = _sArrowToolsEnough;
            transform.parent = m_from.transform;
            transform.localEulerAngles = new Vector3(0, 0, 90);
            transform.localPosition = new Vector3(-0.5f, -2.41f, -200f);
        }
    }

    /// <summary>
    /// Move arrow
    /// </summary>
    public void MoveArrow()
    {
        try
        {
            if (m_to == m_from)
            {
                MoveHimself();
                return;
            }

            _toCoord = CalcCross(m_to.transform.position, m_from.transform.position,
                m_to.m_thisItem.m_isButtonsDisabled, XDif);
            _fromCoord = CalcCross(m_from.transform.position, m_to.transform.position,
                m_from.m_thisItem.m_isButtonsDisabled);

            Vector3 shift = _fromCoord - _toCoord;
            Vector3 middle = _toCoord;
            middle.z = transform.position.z;
            transform.position = middle;
            float angle = 180 * Mathf.Atan(shift.y / shift.x) / Mathf.PI;
            if (_fromCoord.x > _toCoord.x)
            {
                angle = angle - 180;
            }
            if (_fromCoord.x == _toCoord.x)
                if ((_fromCoord.y < _toCoord.y && angle < 0) || (_fromCoord.y > _toCoord.y && angle > 0))
                    angle = -angle;

            float magni = Lenght(shift);
            _ealer.z = angle;
            _magnitude.y = _body.transform.localScale.y;
            _magnitude.x = magni;

            _body.transform.localScale = _magnitude;
            _magnitude.x = -magni / 2;
            _magnitude.y = 0;
            _body.transform.localPosition = _magnitude;
            _quaternion.eulerAngles = _ealer;
            transform.rotation = _quaternion;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    /// <summary>
    /// Move arrows and change colors
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                //position of one of the end object has changed
                if (m_from != null && m_to != null &&
                (m_from.transform.position != _fromCoord || m_to.transform.position != _toCoord))
                {
                    MoveArrow();
                }

                SpriteRenderer rend = _bodyRender;
                SpriteRenderer rendBody = _bodyRender2;
                GameAbstractItem itm = m_to.m_thisItem as GameAbstractItem;
                GameMaterial matFrom = m_from.m_thisItem as GameMaterial;
                if (m_isItTool)
                {
                    Color clr = rend.color;
                    if(m_from.m_thisItem.m_count > 0)
                        clr = _sArrowToolsEnough;
                    else
                        clr = _sArrowToolsNotEnough;
                    if (!clr.Equals(rend.color))
                    {
                        if (_isItHimself)
                        {
                            _arrowBody2.color = clr;
                            _arrowBody3.color = clr;
                        }
                        else
                            rend.color = clr;
                    }

                }   
                else if (itm != null && (itm.m_workers == 0))
                    rend.color = _sArrowMaterialsNotUsed;
                else if (matFrom != null && matFrom.m_productivity < 0)
                    rend.color = _sArrowMaterialsNotEnough;
                else
                    rend.color = _sArrowMaterialsEnough;


                rendBody.color = rend.color;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ArrowScript update exception:" + ex.Message);
        }
    }
}
