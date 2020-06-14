using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wild animals. Can rise on the empty territory. Attack hunters.
/// </summary>
public class WildAnimal : AbstractAnimal
{
    /// <summary> attack </summary>
    public float m_attack = 0;
    /// <summary> protection. Full attack: attack - protection </summary>
    public float m_protection = 0;
    /// <summary> speed per turn in meters </summary>
    public float m_speed = 0;
    /// <summary> how many animals must remain before they try to leave </summary>
    public float m_scary = 0;
    /// <summary> maximum beasts in one hunting try </summary>
    public int m_maxInSquadre = 0;
    /// <summary> minimum beasts in one hunting try </summary>
    public int m_minInSquadre = 0;

    /// <summary> all wild animals </summary>
    static List<WildAnimal> _sAllWildAnimal = new List<WildAnimal>();

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="mat">target</param>
    /// <param name="rep">source</param>
    /// <returns> parsed item </returns>
    /// <returns></returns>
    public static new AbstractObject Parse(AbstractObject mat, ExcelLoading.AbstractObject rep)
    {
        WildAnimal ani = mat as WildAnimal;
        ExcelLoading.WildAnimalItem aniRep = rep as ExcelLoading.WildAnimalItem;
        if (ani != null && aniRep != null)
        {
            ani.m_attack = aniRep.attack;
            ani.m_protection = aniRep.protection;
            ani.m_speed = aniRep.speed;
            ani.m_scary = aniRep.scary;
            ani.m_minInSquadre = decimal.ToInt32(aniRep.minimal_in_squadre);
            ani.m_maxInSquadre = decimal.ToInt32(aniRep.maximum_in_squadre);
            _sAllWildAnimal.Add(ani);
        }
        else
            Debug.Log("WildAnimal.Parse: critical parse error");

        return AbstractAnimal.Parse(mat, rep);
    }

    public static WildAnimal FindSomeone(out int count)
    {
        float val = Random.value * (float)_sAllWildAnimal.Count;
        int position = (int)(val);
        WildAnimal ani = _sAllWildAnimal[position];
        val -= position;
        val *= ani.m_maxInSquadre;

        //no one was found
        if (val > ani.m_count)
        {
            count = 0;
            return ani;
        }

        count = (int)(((float)ani.m_maxInSquadre - (float)ani.m_minInSquadre) * Random.value) + ani.m_minInSquadre;
        return ani.m_count < count ? FindSomeone(out count) : ani;
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static new void Load(string filename)
    {
        var itms = Load<ExcelLoading.wildAnimal>(filename);

        if (_sAllWildAnimal == null)
            _sAllWildAnimal = new List<WildAnimal>();

        foreach (var rep in itms.repetative)
        {
             Parse(new WildAnimal(), rep);
        }
    }
}
