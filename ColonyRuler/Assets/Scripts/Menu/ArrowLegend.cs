using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for changing color and text into the color legend
/// </summary>
public class ArrowLegend : MonoBehaviour
{
    public GameObject m_arrowMaterialsNotUsed;
    public GameObject m_arrowMaterialsEnough;
    public GameObject m_arrowMaterialsNotEnough;
    public GameObject m_arrowToolsEnough;
    public GameObject m_arrowToolsNotEnough;

    public Text m_textMaterialsNotUsed;
    public Text m_textMaterialsEnough;
    public Text m_textMaterialsNotEnough;
    public Text m_textToolsEnough;
    public Text m_textToolsNotEnough;

    // Start is called before the first frame update
    void Start()
    {
        SetColor(m_arrowMaterialsNotUsed, ArrowScript._sArrowMaterialsNotUsed);
        SetColor(m_arrowMaterialsEnough, ArrowScript._sArrowMaterialsEnough);
        SetColor(m_arrowMaterialsNotEnough, ArrowScript._sArrowMaterialsNotEnough);
        SetColor(m_arrowToolsEnough, ArrowScript._sArrowToolsEnough);
        SetColor(m_arrowToolsNotEnough, ArrowScript._sArrowToolsNotEnough);
        Localization.m_onLanguageChanged += OnLanguageChanged;
    }

    void SetColor(GameObject arrow, Color color)
    {
        var child = arrow.transform.Find("Arrow");
        child.GetComponent<Image>().color = color;
        arrow.GetComponent<Image>().color = color;
    }

    public void OnLanguageChanged()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
