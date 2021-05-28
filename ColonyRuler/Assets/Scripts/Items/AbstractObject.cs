using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// Root of all game data.
/// AbstractObject -> resource -> Territory
/// AbstractObject -> resource -> AbstractAnimal -> WildAnimal
/// AbstractObject -> resource -> AbstractAnimal -> DomesticAnimal
/// AbstractObject -> GameAbstractItem -> Science
/// AbstractObject -> GameAbstractItem -> GameMaterial -> Process
/// AbstractObject -> GameAbstractItem -> GameMaterial -> Science
/// AbstractObject -> GameAbstractItem -> GameMaterial -> Population
/// AbstractObject -> GameAbstractItem -> GameMaterial -> Items -> Buildings
/// </summary>
[Serializable]
public class AbstractObject
{
    /// <summary> Can it be managed by player? </summary>
    public bool m_isItIterable = true;
    /// <summary> Is it researched? </summary>
    public int m_isItOpen = 1;
    /// <summary> Can it disappear after research? </summary>
    [NonSerialized]
    public bool m_isItDestroyable = false;

    /// <summary> Is all buttons in IconScript disabled? </summary>
    [NonSerialized]
    public bool m_isButtonsDisabled = false;
    /// <summary> icon name. Should be unique. Can't be changed </summary>
    public string m_name;
    /// <summary> icon name on iconScript. Should be localized </summary>
    [NonSerialized]
    public string m_text;
    /// <summary> default position in the screen </summary>
    [NonSerialized]
    public float m_defaultX, m_defaultY;
    /// <summary> current icon on the screen. Part of MVC </summary>
    [NonSerialized]
    public IconScript m_thisObject = null;
    /// <summary> product per day - consume per day. For understanding of process </summary>
    [NonSerialized]
    public float m_productivity = 0;
    /// <summary> how many items here </summary>
    public float m_count = 0;

    
    #region tooltips
    /// <summary> Tooltips for count. Could be changed by children. Should be localized </summary>
    [NonSerialized]
    public string m_tooltipCount = "How many items you have";
    /// <summary> Tooltips for productivity. Could be changed by children. Should be localized  </summary>
    [NonSerialized]
    public string m_tooltipProductivity = "Productivity: how many was produced - how many was consumed";
    /// <summary> Tooltips for damaged items count. Could be changed by children. Should be localized  </summary>
    [NonSerialized]
    public string m_tooltipDamaged = "not used";
    #endregion

    /// <summary> all game data in the game </summary>
    [NonSerialized]
    public static List<AbstractObject> m_sEverything = new List<AbstractObject>();
    /// <summary> all unparsed data from excel in the game. After parsing, it should be cleared </summary>
    [NonSerialized]
    protected static List<ExcelLoading.AbstractObject> m_sUnparsed = new List<ExcelLoading.AbstractObject>();
    /// <summary> how many items could be produced in this day </summary>
    [NonSerialized]
    public float m_maxProduced = 0;
    /// <summary> how many items was consumed </summary>
    [NonSerialized]
    public float m_consumed = 0;
    /// <summary> how many items was ordered by other productions </summary>
    [NonSerialized]
    public float m_oredered = 0;

    /// <summary> current count. could be changed by childrens </summary>
    public virtual float Count { get { return m_count; } set { m_count = value; } }

    /// <summary>
    /// Deconstructor. Called by Unity
    /// </summary>
    public void OnDestroy()
    {
        Localization.m_onLanguageChanged -= ChangeLanguage;
    }

    /// <summary>
    /// disable all buttons on IconScript
    /// </summary>
    /// <param name="render"> icon render </param>
    public void DisableAllButtons(SpriteRenderer render)
    {
        GameObject canvas = render.transform.parent.Find("Canvas").gameObject;
        canvas.transform.Find("Button_up").gameObject.SetActive(false);
        canvas.transform.Find("Button_plus").gameObject.SetActive(false);
        canvas.transform.Find("Button_minus").gameObject.SetActive(false);
        m_isButtonsDisabled = true;
    }

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public virtual void Copy(AbstractObject source)
    {
        m_isItOpen = source.m_isItOpen;
        m_count = source.m_count;
        m_isItIterable = source.m_isItIterable;
    }

    /// <summary>
    /// Change all text data into current language.
    /// Should be overloaded wherever tooltips have changed
    /// </summary>
    public virtual void ChangeLanguage()
    {
        if (m_name[0] == '-') return; //blocked item
        
        try
        {
            Localization loc = Localization.GetLocalization();
            m_tooltipCount = loc.m_ui.m_abstractCount;
            m_tooltipProductivity = loc.m_ui.m_abstractProductivity;
            m_tooltipDamaged = loc.m_ui.m_abstractDamaged;
            m_text = loc.m_items.m_itemDictionary[m_name];
            if (m_text.Length == 0)
                m_text = m_name;
        }
        catch (Exception ex)
        {
            m_text = m_name;
            Debug.LogError("AbstractObject \"" + m_name + "\":ChangeLanguage exception: " + ex.Message);
        }

        if (m_thisObject != null)
            m_thisObject.ChangeLanguage();
    }

    /// <summary>
    /// clearing unparsed excel data 
    /// </summary>
    public static void ClearUnparsed()
    {
        m_sUnparsed.Clear();
    }

    /// <summary>
    /// Clear all game data
    /// </summary>
    public static void ClearEvrething()
    {
        m_sEverything.Clear();
        m_sUnparsed.Clear();
        Productions.Clear();
    }

    /// <summary>
    /// working of workers. Should be overloaded wherever workers could work
    /// </summary>
    /// <param name="worked"> how many workers had work on this day </param>
    public virtual void Working(long worked = 0)
    {
        //throw new Exception("error: unrealised working into " + m_name);
    }

    /// <summary>
    /// How many items could be produced.
    /// Should be overloaded wherever workers could work
    /// </summary>
    /// <returns> items count </returns>
    public virtual float CountMaxProduction()
    {
        return 0;
    }

    /// <summary>
    /// All items work
    /// </summary>
    public static void DoWork()
    {
        foreach (AbstractObject itm in m_sEverything)
        {
            //clearing variables before calculating
            try
            {
                itm.m_productivity = 0;
                itm.m_oredered = 0;
                itm.m_consumed = 0;
                itm.m_maxProduced = 0;
                if (itm is GameMaterial)
                {
                    GameMaterial mat = (GameMaterial)itm;
                    mat.m_maxProduced = mat.CountMaxProduction();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("AbstractObject: DoWork 1:" + ex.Message);
            }
        }

        for (int i = 0; i < m_sEverything.Count; i++)
        {
            try
            {
                AbstractObject mat = m_sEverything[i];
                mat.Working();
            }
            catch (Exception ex)
            {
                Debug.Log("AbstractObject: DoWork 2:" + ex.Message);
                Camera.main.GetComponent<TimeScript>().m_timeTxt = ex.Message;
            }
        }

        foreach (var itm in m_sEverything)
            if (itm.m_isItOpen > 0)
                itm.CalcProductivity();
    }

    /// <summary>
    /// returns null if there is no name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AbstractObject GetByName(string name)
    {
        foreach (var obj in m_sEverything)
            if (obj.m_name == name)
                return obj;
        return null;
    }

    /// <summary>
    /// Calc m_productivity.
    /// Should be overloaded.
    /// </summary>
    public virtual void CalcProductivity()
    {

    }

    /// <summary>
    /// Could this items be upgraded?
    /// Should be overloaded wherever this conditions changed
    /// </summary>
    public virtual bool CheckUpgradeConditions()
    {
        return false;
    }

    /// <summary>
    /// Upgrade this item.
    /// Could increase max_storage, research item or destroy building
    /// </summary>
    /// <returns> true if successful </returns>
    public virtual bool Upgrade()
    {
        return false;
    }

    /// <summary>
    /// return list of all researched items
    /// </summary>
    public static List<AbstractObject> GetOpennedItems()
    {
        List<AbstractObject> itms = new List<AbstractObject>();

        foreach (var itm in m_sEverything)
            if (itm.m_isItOpen > 0)
                itms.Add(itm);

        return itms;
    }

    /// <summary>
    /// How many items here in text mode. Could be overloaded by children
    /// </summary>
    /// <returns> m_count </returns>
    public virtual string GetCountString() { return m_count.ToString("F"); }

    /// <summary>
    /// Productivity of this item in text mode. Could be overloaded by children
    /// </summary>
    /// <returns> m_productivity </returns>
    public virtual string GetProductivityString() { return m_productivity.ToString("F"); }

    /// <summary>
    /// How many damaged items here in text mode. Should be overloaded by children where it used
    /// </summary>
    /// <returns></returns>
    public virtual string GetDamagedString() { return ""; }

    /// <summary>
    /// change production icon in IconScript.
    /// Should be overloaded by children where it used.
    /// </summary>
    /// <param name="render"> render </param>
    /// <param name="isc"> target </param>
    public virtual void ChangeProductionType(SpriteRenderer render, IconScript isc)
    {

    }

    /// <summary>
    /// Open item after research.
    /// Should be changed by children
    /// </summary>
    public virtual void OpenItem()
    {

    }

    /// <summary>
    /// Parsing float. Without cultural information(could be different on different PC)
    /// and with separator(depends on who write it - I or tool)
    /// </summary>
    /// <param name="str"> float in text mode </param>
    /// <param name="separator"> a separator between integer and the fractional part  </param>
    /// <returns> parsed float </returns>
    public static float FloatParse(string str, string separator = ",")
    {
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        nfi.NumberDecimalSeparator = separator;
        return float.Parse(str, nfi);
    }

    /// <summary>
    /// Load excel data from text file
    /// Should be parsed
    /// </summary>
    /// <typeparam name="T"> excel class </typeparam>
    /// <param name="filename"> excel map name </param>
    /// <returns> container of all items in the file </returns>
    public static T Load<T>(string filename) where T : new()
    {
        TextAsset resorce = Resources.Load(filename) as TextAsset;
        if (resorce == null)
            throw new Exception("the resource is not available:" + filename);

        XmlSerializer x = new XmlSerializer(typeof(T));
        TextReader reader = new StringReader(resorce.text);

        return (T)x.Deserialize(reader);
    }

    /// <summary>
    /// Parsing excel data into current class
    /// execution chain for building looks:
    /// Buildings -> Items -> GameMaterial -> GameAbstractItem -> AbstractObject
    /// </summary>
    /// <param name="rep"> source </param>
    /// <param name="mat"> target </param>
    public static AbstractObject Parse(AbstractObject mat, ExcelLoading.AbstractObject rep)
    {
        mat.m_name = rep.name;
        mat.m_text = rep.description;
        mat.m_defaultX = rep.defaultX;
        mat.m_defaultY = rep.defaultY;

        if (mat.m_name[0] == '-')
            mat.m_isItOpen--;

        m_sEverything.Add(mat);
        m_sUnparsed.Add(rep);

        Localization.m_onLanguageChanged += mat.ChangeLanguage;
        return mat;
    }
}

