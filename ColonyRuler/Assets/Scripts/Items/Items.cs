using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tools, happy items, traps, etc.
/// Has items effects and could be used and damaged.
/// </summary>
[Serializable]
public class Items : GameMaterial
{
    /// <summary> used as hits in Effects </summary>
    [NonSerialized]
    public float m_bug;
    /// <summary> how many items could be repaired per day per worker </summary>
    [NonSerialized]
    public float m_bugFixingPerPerson;
    /// <summary> used as critical hits in Effects </summary>
    [NonSerialized]
    public uint m_critical;
    /// <summary> how many items damaged </summary>
    public float m_damagedCount = 0;
    /// <summary> used, if it is a container </summary>
    [NonSerialized]
    public ContainerEffect m_containerEffect = null;
    /// <summary> used, if it is a happy item </summary>
    [NonSerialized]
    public ItemsEffect m_happyEffect = null;
    /// <summary> how many items was used in upgrades </summary>
    public float m_blocked = 0;

    /// <summary> how many items could be stored here </summary>
    public override float StorageSize { get { return m_storageSize - m_blocked; } }

    /// <summary> how many items here </summary>
    public override float Count { get { return m_count; } set { m_count = value; } }

    /// <summary> items effects, with happy and container effect </summary>
    public Dictionary<string, ItemsEffect> m_effects = new Dictionary<string, ItemsEffect>();

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject source)
    {
        base.Copy(source);
        Items itms = (Items)source;
        m_blocked = itms.m_blocked;
        m_damagedCount = itms.m_damagedCount;
    }

    /// <summary>
    /// How many damaged items here in text mode. Could be overloaded by children
    /// </summary>
    public override string GetDamagedString()
    {
        return m_damagedCount.ToString("F");
    }

    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
        m_tooltipDamaged = Localization.GetLocalization().m_ui.m_itemsDamaged;
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public new static AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {

        ExcelLoading.ItemItem rep = (ExcelLoading.ItemItem)repItm;
        Items itms = (Items)itm;

        itms.m_bug = rep.bug;
        itms.m_bugFixingPerPerson = rep.bug_fixing_per_second;
        itms.m_isItDestroyable = true;
        itms.m_tooltipDamaged = Localization.GetLocalization().m_ui.m_itemsDamaged;

        string[] crtc = rep.critical.Split(' ');
        itms.m_critical = uint.Parse(crtc[2]);
        string[] effTps = rep.effect_type.Split(';');
        foreach (string effect in effTps)
        {
            try
            {
                string[] parts = effect.Split('*');
                string effName = parts[0].Trim();
                float effValue = FloatParse(parts[1].Trim());
                ItemsEffect itemsEffect = effName.CheckType() ?
                    new ContainerEffect(effName, effValue, itms)
                    : new ItemsEffect(effName, effValue, itms);

                itms.m_effects.Add(effName, itemsEffect);
                Productions.AddTools(itemsEffect, effName);

                if (itemsEffect.GetType() == typeof(ContainerEffect))
                {
                    itms.m_containerEffect = itemsEffect as ContainerEffect;
                    Camera.main.GetComponent<Storage>().AddStorageItem(itms);
                }
                if (itemsEffect.m_name == "happy" || itemsEffect.m_name == "maxHappy")
                    itms.m_happyEffect = itemsEffect;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        return GameMaterial.Parse(itm, repItm);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public new static void Load(string filename)
    {
        var itms = Load<ExcelLoading.items>(filename);

        foreach (var rep in itms.repetative)
        {
            Parse(new Items(), rep);
        }
    }

    /// <summary>
    /// Open item after research.
    /// Could be changed by children
    /// Here: setting production as tools wherever it used
    /// </summary>
    public override void OpenItem()
    {

        foreach (var eff in m_effects)
        {
            var products = Productions.GetProductions(eff.Value.m_name);
            foreach (GameAbstractItem prod in products)
            {
                if (prod.m_isItOpen > 0)
                {
                    GameMaterial mat = (GameMaterial)prod;
                    bool hasItTheSame = false;
                    foreach (var effect in mat.m_tools)
                        hasItTheSame |= (effect.m_toolLink.m_name == m_name && effect.m_name == eff.Value.m_name);

                    if (hasItTheSame)
                        continue;
                    else
                    {
                        ItemsEffect neff = new ItemsEffect(eff.Value);
                        neff.m_production = mat;
                        mat.m_tools.Add(neff);
                        ArrowScript asc = ArrowScript.NewArrowScript(neff.m_toolLink.m_thisObject, mat.m_thisObject);
                        asc.m_isItTool = true;
                        mat.m_thisObject.m_toolsTo.Add(asc);
                        m_thisObject.m_toolsFrom.Add(asc);
                    }
                }
            }
        }
        base.OpenItem();
    }

    /// <summary>
    /// How many workers could work here in this day
    /// </summary>
    /// <param name="effect"> speed effect </param>
    /// <param name="mulEffect"> multiplier for final product </param>
    protected override float CountMaxWorkers(float effect, float mulEffect)
    {
        if (m_isItFix)
        {
            float cnt = m_damagedCount / (m_bugFixingPerPerson * effect);
            float maxcnt = (((float)StorageSize / m_size) - Count) / (m_bugFixingPerPerson * effect);
            return cnt < maxcnt ? cnt : maxcnt;
        }
        else
            return base.CountMaxWorkers(effect, mulEffect);
    }

    /// <summary>
    /// Work of workers at the end of the day
    /// first step - repairing damaged items, second - producing new items
    /// </summary>
    /// <param name="worked"> how many workers have work here </param>
    public override void Working(long worked)
    {

        if (m_workers > 0 && worked < m_workers && m_damagedCount > 0)
        {
            m_productivity = 0;
            m_isItFix = true;
            GettingTools();
            worked = WorkingWithTools(worked, 0, m_bugFixingPerPerson);
            m_isItFix = false;
        }
        if ((m_damagedCount + m_count) * m_size < StorageSize)
            base.Working(worked);
    }

    /// <summary>
    /// Event of work finished
    /// </summary>
    /// <param name="productsFinished"> how many items was produced/repaired </param>
    /// <param name="mulEffect"> multiplier to final product. Should not be used on fixing </param>
    public override void WorkComplite(float productsFinished, float mulEffect)
    {
        if (m_isItFix)
        {
            m_damagedCount -= productsFinished;
            if (m_damagedCount < 0)
            {
                Count += productsFinished + m_damagedCount;
                m_damagedCount = 0;
            }
            else
            {
                Count += productsFinished;
            }
        }
        else
        {
            base.WorkComplite(productsFinished, mulEffect);
        }
    }
}

