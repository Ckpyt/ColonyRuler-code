using System;

/// <summary>
/// Resources that cannot be produced by the player and can grow.
/// </summary>
[Serializable]
public class Resource : MineralResource
{
    /// <summary> Percent for growing per day. </summary>
    public float m_growingPercent = 0;

    /// <summary> maximum in territory, depends on territory size </summary>
    public int m_currentMax = 0;

    /// <summary>
    /// How many items could be produced.
    /// </summary>
    /// <returns> items count </returns>
    public override float CountMaxProduction()
    {
        return m_count * m_growingPercent;
    }

    /// <summary>
    /// these resources can grow on territory. Maximum depends on territory size
    /// </summary>
    /// <param name="worked"> not used </param>
    public override void Working(long worked = 0)
    {
        m_currentMax = ((int)m_maxCount * Storage.m_storage.m_territory) / Storage.m_storage.m_territoryMax;
        float newCount = m_count + m_count * m_growingPercent;
        if (newCount < m_currentMax)
            m_count = newCount;
        else
            m_count = m_currentMax;

        if (m_count < 0.1)
            m_count = 0.1f;
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        ExcelLoading.Resource repRes = repItm as ExcelLoading.Resource;
        Resource res = itm as Resource;
        res.m_growingPercent = repRes.growing_percent;
        res.m_currentMax = (int)res.m_maxCount;

        return MineralResource.Parse(itm, repItm);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public new static void Load(string filename)
    {

        var itms = Load<ExcelLoading.resource>(filename);

        foreach (var rep in itms.repetative)
        {
            Parse(new Resource(), rep);
        }
    }
}

