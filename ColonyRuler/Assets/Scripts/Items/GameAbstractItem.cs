using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dependencies for making a recipe.
/// A recipe could have a formula:
/// (ing1 || ing 2) && (ing3), but could not be (ing1 && ing2) || ing3.
/// </summary>
[Serializable]
public class DependencyCount
{
    /// <summary> list of ingredients </summary>
    [NonSerialized]
    public List<AbstractObject> m_dependency = new List<AbstractObject>();

    /// <summary> 
    /// list of ingredients' count. 
    /// Could be changed only on Science.
    /// Should be divided because of serialization 
    /// </summary>
    public List<float> m_value = new List<float>();
}

/// <summary>
/// Abstract item. 
/// Contain a recipe and final results.
/// All the exceptions could happens because of wrong formula in the excel file.
/// So, exceptions should be shown only in the debugger.
/// </summary>
[Serializable]
public class GameAbstractItem : AbstractObject
{
    /// <summary> final result per person </summary>
    [NonSerialized]
    public DependencyCount[] m_producePerPerson;

    /// <summary> recipe </summary>
    [SerializeField]
    public DependencyCount[] m_dependencyCount;
    /// <summary> how many workers here </summary>
    [SerializeField] public long m_workers = 0;

    /// <summary> Is it fixing of damaged tools? </summary>
    [NonSerialized]
    public bool m_isItFix = false;

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject mat, ExcelLoading.AbstractObject rep)
    {
        return AbstractObject.Parse(mat, rep);
    }

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject source)
    {
        base.Copy(source);
        GameAbstractItem itm = source as GameAbstractItem;
        m_workers = itm.m_workers;
    }

    /// <summary>
    /// Parsing recipe formula.
    /// It has format:
    /// (([\w]{1,20}([*][\d]{0,3}){0,1})([|]([\w]{1,20}[*][\d]{0,3})){0,};){1,}
    /// itemName*count | itemName; itemName
    /// </summary>
    /// <param name="dependencyStr">recipe</param>
    /// <returns> parced recipe </returns>
    public static DependencyCount[] ParseDependencyCounts(string dependencyStr)
    {
        string[] dependency = dependencyStr.Split(';');
        DependencyCount[] dependencyCount = new DependencyCount[dependency.Length];

        for (int i = 0; i < dependency.Length; i++)
        {
            string[] conditions = dependency[i].Split('|');
            for (int condI = 0; condI < conditions.Length; condI++)
            {
                string str = conditions[condI];
                float count = 1;
                string[] cnt = str.Split('*');

                if (cnt.Length > 1)
                {
                    try
                    {
                        count = FloatParse(cnt[1]);
                        str = cnt[0];
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Object float parsing error:" + dependency[i] + " " + ex.Message);
                        return null;
                    }
                }

                try
                {
                    while (str.Length > 1 && str[0] == ' ')
                        str = str.Substring(1);

                }
                catch (Exception ex)
                {
                    Debug.Log("Object splitting substrings error:" + dependency[i] + " " + ex.Message);
                    return null;
                }

                foreach (AbstractObject dependMat in m_sEverything)
                {
                    if (dependMat.m_name == str)
                    {
                        if (dependencyCount[i] == null)
                            dependencyCount[i] = new DependencyCount();

                        dependencyCount[i].m_dependency.Add(dependMat);
                        dependencyCount[i].m_value.Add(count);
                        break;
                    }
                }
            }
        }

        return dependencyCount;
    }

    /// <summary>
    /// Parse all recipe
    /// </summary>
    public static void ParseDependency()
    {
        for (int it = 0; it < m_sEverything.Count; it++)
        {
            var rep = m_sUnparsed[it] as ExcelLoading.AbstractItem;
            var mat = m_sEverything[it] as GameAbstractItem;
            WildAnimal wild = m_sEverything[it] as WildAnimal;
            if (wild != null)
                wild.ParseDependency(m_sUnparsed[it]);

            if (mat == null) continue;
            var prod = rep.produce_per_person;

            try
            {
                if (rep.dependency.Length > 2)
                    mat.m_dependencyCount = ParseDependencyCounts(rep.dependency);
                //prod could have two different formats: single float or dependencyCount format
                if (prod[0] >= '0' && prod[0] <= '9')
                {
                    mat.m_producePerPerson = new DependencyCount[1];
                    mat.m_producePerPerson[0] = new DependencyCount();
                    mat.m_producePerPerson[0].m_dependency.Add(mat);
                    mat.m_producePerPerson[0].m_value.Add(FloatParse(prod));
                }
                else
                    mat.m_producePerPerson = ParseDependencyCounts(prod);
            }
            catch (Exception ex) //error in the excel file.
            {
                Debug.Log("Object parsing error:" + mat.m_name + " " + ex.Message);
            }
        }
    }
}


