using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Science and research
/// </summary>
[Serializable]
class Science : GameAbstractItem
{
    /// <summary> Items, what will appear after research </summary>
    [NonSerialized]
    public AbstractObject[] m_itemsForOpening;
    /// <summary> Excel data object. Used for making in-game data </summary>
    [NonSerialized]
    public ExcelLoading.ScienceItem m_repItem = null;
    /// <summary> How many materials reguared to complite research. used for understanding research procegress </summary>
    [NonSerialized]
    public float m_maxCount = 0;

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject source)
    {
        base.Copy(source);
        Science scs = (Science)source;

        for (int i = 0; i < m_dependencyCount.Length; i++)
            for(int ii = 0; ii < m_dependencyCount[i].m_value.Count; ii++)
                m_dependencyCount[i].m_value[ii] = scs.m_dependencyCount[i].m_value[ii];

    }

    /// <summary>
    /// make a production icon as science icon and upgrade icon as research
    /// </summary>
    /// <param name="render"> render </param>
    /// <param name="isc"> target IconScript </param>
    public override void ChangeProductionType(SpriteRenderer render, IconScript isc)
    {
        Vector3 pos = render.transform.position;
        pos.z = -1;
        render.transform.position = pos;
        Texture2D obj = Resources.Load("Pictures/science") as Texture2D;
        render.sprite = Sprite.Create(obj, new Rect(0.0f, 0.0f, obj.width, obj.height), new Vector2(0.5f, 0.5f), 100.0f);
        isc.ChangeSprite("Pictures/Button_up_research", "Pictures/Button_up_research_gray");
    }

    /// <summary>
    /// Researching.
    /// Does not used tools yet, DependecyCount decreasing to zero durong the process
    /// </summary>
    /// <param name="worked"> not used </param>
    public override void Working(long worked)
    {
        float tmpcount = m_producePerPerson[0].m_value[0] * m_workers;
        float cnt = tmpcount;
        float mxcnt = 0;
        m_productivity = 0;
        for (int iL = 0; iL < m_dependencyCount.Length; iL++)
        {
            for (int i = 0; i < m_dependencyCount[iL].m_value.Count; i++)
            {
                mxcnt += m_dependencyCount[iL].m_value[i];
                if (tmpcount > 0)
                {
                    float depCount = tmpcount < m_dependencyCount[iL].m_value[i] ?
                        tmpcount : m_dependencyCount[iL].m_value[i];

                    m_dependencyCount[iL].m_dependency[i].m_productivity -= tmpcount;
                    if (m_dependencyCount[iL].m_dependency[i].Count > depCount)
                    {
                        m_dependencyCount[iL].m_dependency[i].Count -= depCount;
                    }
                    else
                    {
                        depCount = m_dependencyCount[iL].m_dependency[i].Count;
                        m_dependencyCount[iL].m_dependency[i].Count = 0;
                    }
                    m_dependencyCount[iL].m_value[i] -= depCount;
                    tmpcount -= depCount;
                }
            }
        }


        m_productivity = cnt > 0 ? 100 - (100 * tmpcount / cnt) : 0;

        Count += cnt - tmpcount;
        if (mxcnt > m_maxCount)
            m_maxCount = mxcnt;
    }

    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
        Localization loc = Localization.GetLocalization();
        m_tooltipCount = loc.m_ui.m_scienceCount;
        m_tooltipProductivity = loc.m_ui.m_scienceProductivity;
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        Localization loc = Localization.GetLocalization();
        ExcelLoading.ScienceItem rep = (ExcelLoading.ScienceItem)repItm;
        Science scs = (Science)itm;
        scs.m_repItem = rep;
        scs.m_tooltipCount = loc.m_ui.m_scienceCount;
        scs.m_tooltipProductivity = loc.m_ui.m_scienceProductivity;
        Productions.AddProduction(scs, "science");

        return GameAbstractItem.Parse(itm, repItm);
    }

    /// <summary>
    /// Hiding items what should be appear after research.
    /// Called on game initialization
    /// </summary>
    public static void CloseItems()
    {
        foreach (AbstractObject itm in m_sEverything)
        {
            if (itm.GetType() == typeof(Science))
            {
                Science scs = (Science)itm;

                string[] opens = scs.m_repItem.opens.Split(',');
                scs.m_itemsForOpening = new AbstractObject[opens.Length];

                for (int i = 0; i < opens.Length; i++)
                {
                    string open = opens[i];
                    open = open.Trim();

                    foreach (AbstractObject sitm in m_sEverything)
                    {
                        if (sitm.m_name == open)
                        {
                            scs.m_itemsForOpening[i] = sitm;
                            sitm.m_isItOpen--;
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static void Load(string filename)
    {

        var itms = Load<ExcelLoading.science>(filename);

        foreach (var rep in itms.repetative)
        {
            Parse(new Science(), rep);
        }

        CloseItems();
    }

    /// <summary>
    /// Could be this subject science researched?
    /// </summary>
    public override bool CheckUpgradeConditions()
    {
        bool isItResearched = true;
        foreach (var countList in m_dependencyCount)
        {
            bool isAnyDone = false;
            foreach (float count in countList.m_value)
                isAnyDone |= count <= 0;
            isItResearched &= isAnyDone;
        }
        return isItResearched;
    }

    /// <summary>
    /// Complite research process
    /// </summary>
    public override bool Upgrade()
    {
        List<AbstractObject> itms = new List<AbstractObject>();

        foreach (var itm in m_itemsForOpening)
        {
            itm.m_isItOpen++;
            itms.Add(itm);
        }
        CameraScript.m_main.GetComponent<MainScript>().PlaceOpenedItems(itms);

        m_isItOpen = -1000;
        return true;
    }
}