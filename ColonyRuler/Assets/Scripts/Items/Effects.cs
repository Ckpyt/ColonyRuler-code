using System;
using System.Collections.Generic;

/// <summary>
/// type of current effect
/// </summary>
public enum EffectTypes
{
    /// <summary> mistake, it could not be </summary>
    none = 0,
    /// <summary> incrising speed of working </summary>
    speed,
    /// <summary> incrising final result count </summary>
    result,
    /// <summary> it is storage </summary>
    storage
}

/// <summary>
/// Hold all effects in the game
/// </summary>
public class EffectTypeHolder
{
    /// <summary> all effects </summary>
    static Dictionary<string, EffectTypes> _sEffects;

    /// <summary>
    /// return type of effect
    /// </summary>
    /// <param name="name"> effect name </param>
    /// <returns> effect type </returns>
    public static EffectTypes GetType(string name)
    {
        return _sEffects[name];
    }

    /// <summary>
    /// loading all effects from excel map
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static void Load(string filename)
    {
        _sEffects = new Dictionary<string, EffectTypes>();
        var itms = AbstractObject.Load<ExcelLoading.effect>(filename);

        foreach (var rep in itms.repetative)
        {
            EffectTypes type;
            switch (rep.type)
            {
                case "speed":
                    type = EffectTypes.speed;
                    break;
                case "result":
                    type = EffectTypes.result;
                    break;
                case "storage":
                    type = EffectTypes.storage;
                    break;
                default:
                    type = EffectTypes.none;
                    break;
            }
            _sEffects.Add(rep.name, type);
        }
    }
}

/// <summary>
/// Effect class for items, buildings, processes
/// </summary>
[Serializable]
public class ItemsEffect
{
    /// <summary> how effective it is </summary>
    public float m_value;
    /// <summary> effect name </summary>
    public string m_name;
    /// <summary> effect type </summary>
    public EffectTypes m_type;
    /// <summary> item name </summary>
    public string m_toolName;
    /// <summary> how many tools used by workers </summary>
    public long m_toolsCount = 0;
    /// <summary> link into item </summary>
    [NonSerialized]
    public Items m_toolLink;
    /// <summary> link into production where this tool used </summary>
    [NonSerialized]
    public GameAbstractItem m_production;
    /// <summary> current hits of tool </summary>
    public int m_hits = 0;
    /// <summary> current critical hits of tool </summary>
    public int m_criticalHits = 0;
    /// <summary> maximum hits. for reducing CPU loading </summary>
    [NonSerialized]
    public uint m_maxHits = 0;

    /// <summary>
    /// constractor
    /// </summary>
    /// <param name="name"> effect name </param>
    /// <param name="value"> effect value </param>
    /// <param name="tools"> tool with this effect</param>
    public ItemsEffect(string name, float value, Items tool)
    {
        m_name = name;
        m_type = EffectTypeHolder.GetType(name);
        m_value = value;
        m_toolLink = tool;
        m_maxHits = (uint)(1 / m_toolLink.m_bug);
        m_hits = (int)m_maxHits;
        m_criticalHits = (int)m_toolLink.m_critical;
        m_toolName = tool.m_name;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="itm"> source </param>
    public ItemsEffect(ItemsEffect itm)
    {
        m_value = itm.m_value;
        m_name = itm.m_name;
        m_type = itm.m_type;
        m_toolLink = itm.m_toolLink;
        m_maxHits = (uint)(1 / m_toolLink.m_bug);
        m_hits = (int)m_maxHits;
        m_criticalHits = (int)m_toolLink.m_critical;
        m_toolName = itm.m_toolName;
    }

    /// <summary>
    /// Copying effect from source. For save loading
    /// </summary>
    /// <param name="source"> source </param>
    public void Copy(ItemsEffect source)
    {
        m_toolsCount = source.m_toolsCount;
        m_hits = source.m_hits;
        m_criticalHits = source.m_criticalHits;
    }

    /// <summary>
    /// Calculate how many tools was damaged or hits reduced
    /// </summary>
    /// <param name="hits"> in/out hits </param>
    /// <param name="max"> maximum hits </param>
    /// <param name="count"> tools used </param>
    /// <returns></returns>
    long CalcDamag(ref int hits, uint max, long count)
    {
        if (max < 1) return 0;
        long destr = (long)(count / max);
        hits -= (int)(count % max);
        while (hits < 0)
        {
            hits = hits + (int)max;
            destr++;
        }
        return destr;
    }

    /// <summary>
    /// tool was used in the production.
    /// Some tools should be damaged
    /// </summary>
    /// <param name="count"> how many tools was used </param>
    public void ToolUsed(long count)
    {
        m_toolsCount -= CalcDamag(ref m_criticalHits, m_toolLink.m_critical, count);
        long dmg = CalcDamag(ref m_hits, m_maxHits, count);
        m_toolsCount -= dmg;
        m_toolLink.m_damagedCount += dmg;
    }
}

/// <summary>
/// Container effect of items or buildings
/// </summary>
[Serializable]
public class ContainerEffect : ItemsEffect
{
    /// <summary> type of container </summary>
    public ContainerType m_containerType;

    /// <summary>
    /// copy constructor
    /// </summary>
    /// <param name="source"> source </param>
    public ContainerEffect(ContainerEffect source) : base(source)
    {
        m_containerType = source.m_containerType;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">effect name</param>
    /// <param name="value">effect value</param>
    /// <param name="tool">tool link</param>
    public ContainerEffect(string name, float value, Items tool) :
        base(name, value, tool)
    {
        m_containerType = name.ParseCont();
    }
}

