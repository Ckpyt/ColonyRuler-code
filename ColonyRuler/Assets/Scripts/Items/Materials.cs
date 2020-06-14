using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Material. Cannot be used, damaged or repaired.
/// Mostly used in next productions
/// </summary>
[Serializable]
public class GameMaterial : GameAbstractItem
{
    /// <summary> there item should be stored </summary>
    [NonSerialized]
    public ContainerType m_container;
    /// <summary> size on container </summary>
    [NonSerialized]
    public float m_size = 1;
    /// <summary> storage of materials. count*size <=storageSize. Could be overloaded be children </summary>
    public virtual float StorageSize { get { return m_storageSize; } set { m_storageSize = value; } }
    /// <summary> storage of materials value </summary>
    public float m_storageSize = 50;
    /// <summary> The type of production. Used for getting tools. Could be many types </summary>
    [NonSerialized]
    public List<string> m_productionType = new List<string>();

    /// <summary> tools in this production. Could be only tools </summary>
    public List<ItemsEffect> m_tools = new List<ItemsEffect>();

    /// <summary> Will be called after production will be finished. Once per production cicle</summary>
    public delegate void OnWorkComplite(float finished, float mulEffect);
    /// <summary> Event of finishing production cicle </summary>
    public OnWorkComplite m_onWork;

    /// <summary> 
    /// counts of product - consumed per day at some days 
    /// For understanding production cycles
    /// </summary>
    public Queue<float> m_productQueue = new Queue<float>();
    /// <summary> 
    /// How many days should be stored in m_productQueue 
    /// The less the value, the more flexible m_productivity
    /// The higher the value, the smoother m_productivity
    /// </summary>
    public static int m_sProductQueueLimit = 10;
    /// <summary> Count one day before </summary>
    public float m_lastCount = 0;


    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject itm)
    {
        base.Copy(itm);
        GameMaterial mat = (GameMaterial)itm;
        m_storageSize = mat.m_storageSize;
        m_lastCount = mat.m_lastCount;
        m_productQueue = mat.m_productQueue;

    }

    /// <summary>
    /// Copting ItemsEffects from another material
    /// </summary>
    /// <param name="source">source</param>
    public void CopyTools(GameMaterial source)
    {
        if (m_tools != null && source.m_tools != null)
            foreach (ItemsEffect tool in m_tools)
                foreach (ItemsEffect matTool in source.m_tools)
                {
                    if (tool.m_toolName == matTool.m_toolName)
                    {
                        tool.Copy(matTool);
                        break;
                    }
                }
    }

    /// <summary>
    /// How many workers could work here in this day
    /// </summary>
    /// <param name="effect"> speed effect </param>
    /// <param name="mulEffect"> multiplier for final product </param>
    protected virtual float CountMaxWorkers(float effect, float mulEffect)
    {
        return (((float)StorageSize / m_size) - Count + m_oredered) / (m_producePerPerson[0].m_value[0] * effect * mulEffect);
    }

    /// <summary>
    /// How many items could be produced.
    /// </summary>
    /// <returns> items count </returns>
    public override float CountMaxProduction()
    {
        if (m_workers == 0) return 0;

        float prod = 0;
        float effect = 1;
        float mulEffect = 1;
        long wrks = m_workers;
        long tmpwrks = 0;
        for (int i = 0; m_workers > tmpwrks && i <= m_tools.Count; i++)
        {
            wrks = m_workers - tmpwrks;

            if (i > 0 && i < m_tools.Count && m_tools[i - 1].m_toolsCount == m_tools[i].m_toolsCount)
                continue;

            for (int it = i; it < m_tools.Count; it++)
            {
                var tool = m_tools[it];
                if (tool.m_type == EffectTypes.speed)
                    effect += tool.m_value;
                else if (tool.m_type == EffectTypes.result)
                    mulEffect += tool.m_value;
            }

            //calc max wrks
            if (m_dependencyCount != null)
            {
                for (int iL = 0; iL < m_dependencyCount.Length; iL++)
                {
                    int count = 0;
                    for (int id = 0; id < m_dependencyCount[iL].m_dependency.Count; id++)
                    {
                        var dep = m_dependencyCount[iL].m_dependency[id];
                        count += (int)((dep.Count - dep.m_oredered) / (effect * m_dependencyCount[iL].m_value[id]));
                    }
                    wrks = wrks < count ? wrks : count;
                }
                //order resources
                for (int iL = 0; iL < m_dependencyCount.Length; iL++)
                {
                    long count = wrks;
                    for (int id = 0; id < m_dependencyCount[iL].m_dependency.Count && count > 0; id++)
                    {
                        var dep = m_dependencyCount[iL].m_dependency[id];
                        long tmpOrdr = (long)((dep.Count - dep.m_oredered) / 
                            (effect * m_dependencyCount[iL].m_value[id]));
                        if (tmpOrdr > count)
                        {
                            dep.m_oredered = count * effect * m_dependencyCount[iL].m_value[id];
                            count = 0;
                        }
                        else
                        {
                            dep.m_oredered = tmpOrdr * effect * m_dependencyCount[iL].m_value[id];
                            count -= tmpOrdr;
                        }
                    }
                }
            }
            prod += wrks * m_producePerPerson[0].m_value[0] * effect * mulEffect;
            tmpwrks += wrks;
        }
        return prod;
    }

    /// <summary>
    /// One cycle of working.
    /// If we have 1 tool1, 2 tool2 and 4 workers, it will works 3 times:
    /// 1 - with 1 tool1 and 1 tool2, second - with 1 tool2, third - 2 with no tools
    /// Tools should be sorted by tools count from smaller to bigger count
    /// </summary>
    /// <param name="worked"> how many workers have work here </param>
    /// <param name="startIndex"> witch tools already worked </param>
    /// <param name="producePerPerson"> how many items could produce a worker </param>
    /// <returns> how many items was produced </returns>
    protected long WorkingWithTools(long worked, int startIndex, float producePerPerson)
    {
        float effect = 1;
        float mulEffect = 1;
        while (startIndex < m_tools.Count && (long)m_tools[startIndex].m_toolsCount == worked)
            startIndex++;


        for (int it = startIndex; it < m_tools.Count; it++)
        {
            var tool = m_tools[it];
            if (tool.m_type == EffectTypes.speed)
                effect += tool.m_value;
            else if (tool.m_type == EffectTypes.result)
                mulEffect += tool.m_value;
        }

        float maxWorkers = CountMaxWorkers(effect, mulEffect);

        long wrks = m_workers < maxWorkers ? m_workers : (int)maxWorkers;
        wrks -= worked;
        if (wrks < 1) return worked;

        long depWorks = wrks;

        long toolsMax = wrks;
        if (startIndex < m_tools.Count)
            toolsMax = toolsMax < m_tools[startIndex].m_toolsCount ?
                toolsMax : m_tools[startIndex].m_toolsCount;

        wrks = wrks < toolsMax ? wrks : toolsMax;
        
        if (m_dependencyCount != null && m_dependencyCount.Length > 0 && !m_isItFix)
        {
            for (int iL = 0; iL < m_dependencyCount.Length; iL++)
            {
                int count = 0;
                for (int i = 0; i < m_dependencyCount[iL].m_dependency.Count; i++)
                {
                    var dep = m_dependencyCount[iL].m_dependency[i];
                    count += (int)(dep.Count / (effect * m_dependencyCount[iL].m_value[i]));
                    if(dep.GetType() == typeof(Process))
                    {
                        Process proc = dep as Process;
                        mulEffect = proc.m_mulEffect;
                    }
                        
                }
                wrks = wrks < count ? wrks : count;
            }

            depWorks = wrks > 0 ? wrks : depWorks;

            for (int iL = 0; iL < m_dependencyCount.Length; iL++)
            {
                long depWrks = wrks;
                for (int i = 0; i < m_dependencyCount[iL].m_dependency.Count && depWrks > 0; i++)
                {
                    long tmpwrks = depWrks;
                    var dep = m_dependencyCount[iL].m_dependency[i];
                    int maxWkrs = (int)(dep.Count / (effect * m_dependencyCount[iL].m_value[i]));
                    tmpwrks = tmpwrks < maxWkrs ? tmpwrks : maxWkrs;
                    float consum = effect * tmpwrks * m_dependencyCount[iL].m_value[i];
                    m_dependencyCount[iL].m_dependency[i].Count -= consum;
                    m_dependencyCount[iL].m_dependency[i].m_consumed += consum;
                    depWrks -= tmpwrks;
                }
            }
        }

        for (int i = startIndex; i < m_tools.Count; i++)
            m_tools[i].ToolUsed((long)wrks);

        float effectWorker = effect * wrks;
        //it is fix
        if (producePerPerson != m_producePerPerson[0].m_value[0])
        {

            float prodFinished = effectWorker * producePerPerson;

            if (m_onWork != null)
                m_onWork(prodFinished, 1);
            else
            {
                Count += prodFinished;
                if (Count * m_size > StorageSize)
                    Count = StorageSize / m_size;
            }
        }
        else //this is production
        {
            for (int it = 0; it < m_producePerPerson.Length; it++)
            {
                var dep = m_producePerPerson[it];
                if (it == 0 && m_onWork != null && this == dep.m_dependency[0])
                {
                    m_onWork(effectWorker * producePerPerson, mulEffect);
                }
                else
                {
                    for (int i = 0; i < dep.m_dependency.Count; i++)
                    {

                        dep.m_dependency[i].m_count += 
                            (dep.m_dependency[i].m_name == "empty field" || 
                            dep.m_dependency[i].m_name == "farm field" ?
                            1 : mulEffect) * effectWorker * dep.m_value[i];

                        GameMaterial mat = dep.m_dependency[i] as GameMaterial;
                        if (mat != null)
                        {
                            float maxCount = ((float)mat.StorageSize / mat.m_size);
                            if (dep.m_dependency[i].m_count > maxCount)
                                dep.m_dependency[i].m_count = maxCount;
                        }
                    }
                }
            }
        }

        worked += wrks;
        if ((wrks > 0 || wrks == toolsMax) && worked < m_workers && startIndex < m_tools.Count)
            return WorkingWithTools(worked, ++startIndex, producePerPerson);

        return worked;
    }

    /// <summary>
    /// Count m_productivity
    /// </summary>
    public override void CalcProductivity()
    {

        while (m_productQueue.Count >= m_sProductQueueLimit)
            m_productQueue.Dequeue();

        while (m_productQueue.Count < m_sProductQueueLimit)
            m_productQueue.Enqueue(m_maxProduced - m_consumed);

        m_productivity = (m_productQueue.Sum() / m_sProductQueueLimit);
        m_lastCount = Count;
    }

    /// <summary>
    /// Getting tools from production to ItemEffect as many as workers here
    /// Also, sorting tools from smaller counts to bigger.
    /// </summary>
    protected void GettingTools()
    {
        //getting tools
        for (int i = 0; i < m_tools.Count; i++)
        {
            if (m_tools[i].m_toolsCount < m_workers && m_tools[i].m_toolLink.Count >= 1)
            {
                long count = (int)m_tools[i].m_toolLink.Count;
                long neededCount = m_workers - m_tools[i].m_toolsCount;
                count = count < neededCount ? count : neededCount;
                m_tools[i].m_toolLink.Count -= count;
                m_tools[i].m_toolsCount += count;
            }
        }
        //working
        if (m_tools.Count > 1)
            m_tools.Sort((ItemsEffect pair1, ItemsEffect pair2) =>
                 (int)(pair1.m_toolsCount - pair2.m_toolsCount));
    }

    /// <summary>
    /// working of workers.
    /// </summary>
    /// <param name="worked"> how many workers had work on this day </param>
    public override void Working(long worked)
    {
        if (m_workers > 0 && worked < m_workers)
        {
            if (Count * m_size >= StorageSize)
                return;

            GettingTools();
            WorkingWithTools(worked, 0, m_producePerPerson[0].m_value[0]);
        }
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        ExcelLoading.MaterialItem rep = (ExcelLoading.MaterialItem)repItm;
        GameMaterial mat = (GameMaterial)itm;
        mat.m_size = rep.container;
        mat.m_container = rep.container_type.ParseCont();
        string[] prods = rep.production_type.Split('&');
        foreach (string prod in prods)
        {
            string prodTmp = prod.Trim();
            mat.m_productionType.Add(prodTmp);
            Productions.AddProduction(mat, prodTmp);
        }
        mat.m_onWork = mat.WorkComplite;

        return GameAbstractItem.Parse(mat, rep);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static void Load(string filename)
    {

        var itms = Load<ExcelLoading.materials>(filename);

        foreach (var rep in itms.repetative)
        {
            if (rep.name == "hunting")
                Parse(new Hunting(), rep);
            else
                Parse(new GameMaterial(), rep);
        }
    }

    /// <summary>
    /// Open item after research.
    /// Here: getting tools for production
    /// Could be changed by children
    /// </summary>
    public override void OpenItem()
    {
        List<ItemsEffect> tools = new List<ItemsEffect>();
        foreach (string prod in m_productionType)
            foreach (ItemsEffect effect in Productions.GetTools(prod.Trim()))
                tools.Add(effect);

        foreach (var tool in tools)
        {
            if (tool.m_toolLink.m_isItOpen > 0)
            {
                bool hasItThesame = false;
                foreach (var eff in m_tools)
                    hasItThesame |= eff.m_toolLink.m_name == tool.m_toolLink.m_name;

                if (hasItThesame)
                    continue;
                else
                {
                    try
                    {
                        ItemsEffect neff = new ItemsEffect(tool);
                        m_tools.Add(neff);
                        ArrowScript asc = ArrowScript.NewArrowScript(neff.m_toolLink.m_thisObject, m_thisObject);
                        asc.m_isItTool = true;
                        m_thisObject.m_toolsTo.Add(asc);
                        neff.m_toolLink.m_thisObject.m_toolsFrom.Add(asc);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
            }
        }

        for (int i = 0; i < 10; i++)
            m_productQueue.Enqueue(0);

        base.OpenItem();
    }

    /// <summary>
    /// Event of work finished
    /// </summary>
    /// <param name="productsFinished"> how many items was produced/repaired </param>
    /// <param name="mulEffect"> multiplier to final product.</param>
    public virtual void WorkComplite(float productsFinished, float mulEffect)
    {
        Count += productsFinished;
    }

    /// <summary>
    /// Could be storageSize upgraded?
    /// </summary>
    public override bool CheckUpgradeConditions()
    {
        return Storage.m_storage.GetValue(m_container) > m_storageSize;
    }

    /// <summary>
    /// Double the storageSize
    /// </summary>
    /// <returns></returns>
    public override bool Upgrade()
    {
        if (Storage.m_storage.Get(m_container, m_storageSize))
        {
            m_storageSize *= 2;
            return true;
        }
        else
            return false;
    }
}

