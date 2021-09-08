using System;
using UnityEngine;

/// <summary>
/// all animals in the game
/// </summary>
public class AbstractAnimal : Resource
{
    /// <summary> list of butchering materials </summary>
    [NonSerialized]
    public DependencyCount[] m_butcheringPerPerson;

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="mat">target</param>
    /// <param name="rep">source</param>
    /// <returns> parsed item </returns>
    /// <returns></returns>
    public static new AbstractObject Parse(AbstractObject mat, ExcelLoading.AbstractObject rep)
    {
        AbstractAnimal ani = mat as AbstractAnimal;
        ExcelLoading.AbstractAnimal aniRep = rep as ExcelLoading.AbstractAnimal;
        if (ani != null && aniRep != null)
            ani.m_butcheringPerPerson = GameAbstractItem.ParseDependencyCounts(aniRep.butchering_per_person);
        else
            Debug.Log("AbstractAnimal.Parse: critical parse error");

        return Resource.Parse(mat, rep);
    }
}
