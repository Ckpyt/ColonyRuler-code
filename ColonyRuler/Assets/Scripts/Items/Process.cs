using System;

/// <summary>
/// Production reguered some time with no workers.
/// For example, raw brick requre 1 week for drying 
/// </summary>
[Serializable]
class Process : GameMaterial
{
    /// <summary>
    /// Items for waiting
    /// </summary>
    [Serializable]
    public class ProcessItems
    {
        /// <summary> how many items should wait </summary>
        public float m_itemsCount = 0;
    }

    /// <summary> Queue on array. Count = duration </summary>
    public ProcessItems[] m_itemsStarted;
    /// <summary> multiplicator for some final products </summary>
    public float m_mulEffect = 1;
    /// <summary> how many times take the process </summary>
    int _duration = 1;

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject source)
    {
        Process from = source as Process;
        if (from == null) return;

        base.Copy(source);

        m_mulEffect = from.m_mulEffect;
        if (m_itemsStarted == null) m_itemsStarted = new ProcessItems[_duration + 1];
        for (int i = 0; i <= _duration; i++)
        {
            m_itemsStarted[i].m_itemsCount = from.m_itemsStarted[i].m_itemsCount;
        }

    }

    /// <summary>
    /// Process could not be upgraded
    /// </summary>
    public override bool CheckUpgradeConditions()
    {
        return false;
    }

    /// <summary>
    /// Return finished items from queue and make new items
    /// </summary>
    /// <param name="worked"> not used </param>
    public override void Working(long worked)
    {
        StorageSize = 100000;
        if (_duration > 0)
        {
            int indx = TimeScript.GetTotalDays() % _duration;
            ProcessItems itm = m_itemsStarted[indx];
            m_count += itm.m_itemsCount;
            itm.m_itemsCount = 0;
        }
        base.Working(worked);
        StorageSize = m_count;
    }

    /// <summary>
    /// How many items in the waiting queue
    /// </summary>
    float TotalInWaiting()
    {
        float inProc = 0;
        for (int i = 0; i < _duration; i++)
            inProc += m_itemsStarted[i].m_itemsCount;
        return inProc;
    }

    /// <summary>
    /// Specify m_count to string format as "count\\waiting"
    /// </summary>
    /// <returns></returns>
    public override string GetCountString()
    {
        float inProc = TotalInWaiting();
        return m_count.ToString("F") + (inProc > 0 ? "\\" + inProc.ToString("F") : "");
    }

    /// <summary>
    /// Event of work finished
    /// </summary>
    /// <param name="productsFinished"> how many items was produced/repaired </param>
    /// <param name="mulEffect"> multiplier to final product after waiting.</param>
    public override void WorkComplite(float productsFinished, float mulEffect)
    {
        float inProc = TotalInWaiting();

        if (productsFinished > 0)
        {
            m_mulEffect = (m_mulEffect + mulEffect) / 2;
        }
        else if (inProc < 0.1)
            m_mulEffect = 0;

        if (_duration > 0)
        {
            int indx = TimeScript.GetTotalDays() % _duration;
            ProcessItems itm = m_itemsStarted[indx];
            itm.m_itemsCount += productsFinished;
        }
        else
            m_count += productsFinished;
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        ExcelLoading.Process rep = (ExcelLoading.Process)repItm;
        Process proc = (Process)itm;
        proc.m_onWork = proc.WorkComplite;

        proc._duration = rep.duration;
        proc.m_itemsStarted = new ProcessItems[proc._duration + 1];
        for (int i = 0; i <= proc._duration; i++)
            proc.m_itemsStarted[i] = new ProcessItems();

        return GameMaterial.Parse(itm, repItm);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static new void Load(string filename)
    {
        var itms = Load<ExcelLoading.process>(filename);
        foreach (var rep in itms.repetative)
        {
            Parse(new Process(), rep);
        }
    }

    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
        m_tooltipCount = Localization.GetLocalization().m_ui.m_processCount;
    }

}

