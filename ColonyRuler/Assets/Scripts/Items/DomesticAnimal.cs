using System.Collections.Generic;
using UnityEngine;

public class DomesticAnimal : AbstractAnimal
{
    /// <summary> a type of food for an animal </summary>
    public DependencyCount[] m_foodType;
    /// <summary> how many food could get a person for animals </summary>
    public float m_producePerPerson;
    /// <summary> could an animal generate something per day? </summary>
    public DependencyCount[] m_additionalProduction;

    /// <summary> there animal should be placed </summary>
    public ContainerType m_container;


    static List<DomesticAnimal> _sAllDomesticAnimal;

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="mat">target</param>
    /// <param name="rep">source</param>
    /// <returns> parsed item </returns>
    /// <returns></returns>
    public static new AbstractObject Parse(AbstractObject mat, ExcelLoading.AbstractObject rep)
    {
        DomesticAnimal ani = mat as DomesticAnimal;
        ExcelLoading.DomesticAnimalItem aniRep = rep as ExcelLoading.DomesticAnimalItem;
        if (ani != null && aniRep != null)
        {
            ani.m_container = aniRep.storageType.ParseCont();
            ani.m_producePerPerson = aniRep.produce_per_person;
            ani.m_foodType = GameAbstractItem.ParseDependencyCounts(aniRep.foodType);
            ani.m_additionalProduction = GameAbstractItem.ParseDependencyCounts(aniRep.additionalProducts);
            //link to domesticAnimal!!!
            _sAllDomesticAnimal.Add(ani);
        }
        else
            Debug.Log("WildAnimal.Parse: critical parse error");

        return AbstractAnimal.Parse(mat, rep);
    }

    /// <summary>
    /// Return animal's object by name
    /// </summary>
    /// <param name="name">the name for looking</param>
    /// <returns></returns>
    public static DomesticAnimal GetAnimal(string name)
    {
        foreach (var ani in _sAllDomesticAnimal)
            if (ani.m_name == name)
                return ani;

        return null;
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static new void Load(string filename)
    {
        var itms = Load<ExcelLoading.domesticAnimal>(filename);

        if (_sAllDomesticAnimal == null)
            _sAllDomesticAnimal = new List<DomesticAnimal>();

        foreach (var rep in itms.repetative)
        {
            Parse(new DomesticAnimal(), rep);
        }
    }


    /// <summary>
    /// Open item after researching
    /// </summary>
    public override void OpenItem()
    {
        base.OpenItem();
    }

    /// <summary>
    /// TODO: check update condition by animal's size
    /// </summary>
    /// <returns></returns>
    public override bool CheckUpgradeConditions()
    {
        if (Storage.m_storage.Get(m_container, m_currentMax))
            m_currentMax += m_currentMax;

        return base.CheckUpgradeConditions();
    }

    /// <summary>
    /// TODO: check update condition by animal's size
    /// </summary>
    /// <returns></returns>
    public override bool Upgrade()
    {
        return false;
    }

    /// <summary>
    /// these resources can grow on territory. Maximum depends on territory size
    /// TODO: produce additional products
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

}
