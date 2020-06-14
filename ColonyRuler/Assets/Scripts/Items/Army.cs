using System;

/// <summary>
/// Not used now class
/// Will be used on the second stage of development
/// </summary>
[Serializable]
public class Army : GameMaterial
{
    /// <summary> attack, protection(result damage: atack - protection), distance of attack(in metres), hits </summary>
    [NonSerialized]
    public int m_attack, m_protection, m_distance, m_hits;
    /// <summary> speed per second. The round starts at maximum distance, and on every step the armies is approaching </summary>
    [NonSerialized]
    public float m_speedPerSecond;

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        ExcelLoading.ArmyItem rep = (ExcelLoading.ArmyItem)repItm;
        Army arm = (Army)itm;

        arm.m_attack = decimal.ToInt32(rep.attack);
        arm.m_protection = decimal.ToInt32(rep.protection);
        arm.m_distance = decimal.ToInt32(rep.distance);
        arm.m_hits = decimal.ToInt32(rep.hits);
        arm.m_speedPerSecond = rep.speed_per_second;

        return GameMaterial.Parse(itm, repItm);
    }

    public override void Copy(AbstractObject itm)
    {
        Army source = itm as Army;
        m_attack = source.m_attack;
        m_protection = source.m_protection;
        m_distance = source.m_distance;
        m_hits = source.m_hits;
        m_speedPerSecond = source.m_speedPerSecond;
        base.Copy(itm);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static new void Load(string filename)
    {

        var itms = Load<ExcelLoading.army>(filename);

        foreach (var rep in itms.repetative)
        {
            Parse(new Army(), rep);
        }
    }
}

