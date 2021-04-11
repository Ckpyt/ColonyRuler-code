using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container types. Used in container effect
/// </summary>
[Serializable]
public enum ContainerType
{
    /// <summary> mistake </summary>
    none,
    /// <summary> houses, living places </summary>
    people,
    /// <summary> light storage </summary>
    light,
    /// <summary> heavy storage </summary>
    heavy,
    territory,
    /// <summary> small animal container </summary>
    smallAnimals,
    /// <summary> big animal container </summary>
    bigAnimals
}

/// <summary>
/// Extend string type on some methods for ContainerType
/// </summary>
public static class ContainerTypeMethods
{
    /// <summary>
    /// Convert Container type to string
    /// </summary>
    /// <param name="type"> source </param>
    public static string ToString(this ContainerType type)
    {
        switch (type)
        {
            case ContainerType.people:
                return "living";
            case ContainerType.light:
                return "light_storage";
            case ContainerType.heavy:
                return "heavy_storage";
            case ContainerType.territory:
                return "territory";
            case ContainerType.bigAnimals:
                return "big_animal";
            case ContainerType.smallAnimals:
                return "small_animal";
            default:
                throw new Exception("ContainerType::ToString: unexpected value");
        }
    }

    /// <summary>
    /// Is it a storage type?
    /// </summary>
    /// <param name="type"> source </param>
    public static bool CheckType(this string type)
    {
        switch (type.Length)
        {
            case 6: return type == "living";
            case 13: return type == "light_storage" || type == "heavy_storage";
            case 12: return type == "small_animal";
            case 10: return type == "big_animal";
            case 9: return type == "territory";
            default: return false;
        }
    }

    /// <summary>
    /// Convert string to ContainerType
    /// </summary>
    /// <param name="type"> source </param>
    public static ContainerType ParseCont(this string type)
    {
        type = type.ToLower();
        switch (type)
        {
            case "living":
                return ContainerType.people;
            case "light_storage":
                return ContainerType.light;
            case "heavy_storage":
                return ContainerType.heavy;
            case "territory":
                return ContainerType.territory;
            case "big_animal":
                return ContainerType.bigAnimals;
            case "small_animal":
                return ContainerType.smallAnimals;
            default:
                throw new Exception("ContainerType::parse: unexpected value:" + type);
        }
    }
}

/// <summary>
/// Used for making saves in Unity3D. 
/// </summary>
[Serializable]
class MyDictionary : Dictionary<string, Productions>
{

}

/// <summary>
/// link between tools, productions and effects
/// </summary>
[Serializable]
class Productions
{
    /// <summary> production of this effect </summary>
    public List<GameAbstractItem> m_productions = new List<GameAbstractItem>();
    /// <summary> tools with this effect </summary>
    public List<ItemsEffect> m_tools = new List<ItemsEffect>();

    /// <summary> productions and tools sorted by effect name </summary>
    public static MyDictionary m_sAllProductions = new MyDictionary();

    /// <summary>
    /// Return productions with effect name
    /// </summary>
    static Productions GetProd(string effectName)
    {
        Productions prods = null;
        try
        {
            prods = m_sAllProductions[effectName];
        }
        catch (KeyNotFoundException ex)
        {
            prods = new Productions();
            m_sAllProductions[effectName] = prods;
            //Debug.Log("New product for" + effectName + " message:" + ex.Message);
        }
        return prods;
    }

    /// <summary>
    /// Add production into dictonary by ItemsEffect.name
    /// </summary>
    /// <param name="prod"> production </param>
    public static void AddProduction(GameAbstractItem prod, string effectName)
    {
        GetProd(effectName).m_productions.Add(prod);
    }

    /// <summary>
    /// Add Tool effect. One tool can have many effects.
    /// For example, stone knife can used for butchering and carpentry
    /// </summary>
    /// <param name="tool"> tool effect </param>
    /// <param name="effectName"> ItemsEffect.name </param>
    public static void AddTools(ItemsEffect tool, string effectName)
    {
        GetProd(effectName).m_tools.Add(tool);
    }

    /// <summary>
    /// return all tools with this effect
    /// </summary>
    /// <returns> list of tools </returns>
    public static List<ItemsEffect> GetTools(string effectName)
    {
        return GetProd(effectName).m_tools;
    }

    /// <summary>
    /// return all productions wherever production type
    /// </summary>
    /// <param name="effectName"> type of production </param>
    /// <returns> list of productions </returns>
    public static List<GameAbstractItem> GetProductions(string effectName)
    {
        return GetProd(effectName).m_productions;
    }

    /// <summary>
    /// Clear dictionary.
    /// Should be used after exiting from game or before loading
    /// </summary>
    public static void Clear()
    {
        m_sAllProductions.Clear();
    }
}

