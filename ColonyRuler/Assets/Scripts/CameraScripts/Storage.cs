using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for counting storages.
/// Territory -> living space
/// territory -> heavy items -> light items
/// </summary>
[Serializable]
public class Storage : MonoBehaviour
{
    /// <summary> then the game is paused, it should not works</summary>
    public static bool m_isItPaused = false;
    /// <summary> for showing how much territory colony has </summary>
    public Text m_territoryText;
    /// <summary> for showing how many heavy storage space avaliable </summary>
    public Text m_heavyText;
    /// <summary> for showing how many light storage space avaliable </summary>
    public Text m_lightText;
    /// <summary> for showing how many living rooms avaliable </summary>
    public Text m_livingText;

    /// <summary> territory text object. for localization text "territory" </summary>
    public Text m_territoryObject;
    /// <summary> heavy storage text object. for localization text "heavy storage" </summary>
    public Text m_heavyObject;
    /// <summary> light storage text object. for localization text "light storage" </summary>
    public Text m_lightObject;
    /// <summary> livng space text object. for localization text "living space" </summary>
    public Text m_livingObject;

    /// <summary> territory count </summary>
    public int m_territory = 100000;
    /// <summary> max terrotory count </summary>
    public int m_territoryMax = 100000;
    /// <summary> heavy storage count </summary>
    int _heavy = 0;
    /// <summary> light storage count </summary>
    int _light = 0;
    /// <summary> living space count </summary>
    int _living = 0;
    /// <summary> list of all heavy storages  </summary>
    List<Items> _heavyStorages = new List<Items>();
    /// <summary> list of all light storages </summary>
    List<Items> _lightStorages = new List<Items>();
    /// <summary> list of all building storages </summary>
    List<Items> _buildingStorages = new List<Items>();
    /// <summary> list of all houses </summary>
    public List<Items> m_livingStorages = new List<Items>();
    /// <summary> link to people </summary>
    public People m_people;
    /// <summary> link to only one storage </summary>
    public static Storage m_storage = null;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Localization.m_onLanguageChanged += ChangeLanguage;
            ChangeLanguage();
            m_storage = this;
        }
        catch (Exception ex)
        {
            Debug.LogError("Storage Start exception" + ex.Message);
        }
    }

    /// <summary>
    /// GameObject destroyed. Called by Unity
    /// </summary>
    public void OnDestroy()
    {
        Localization.m_onLanguageChanged -= ChangeLanguage;
    }

    /// <summary>
    /// Method for change text object then current languge has changed
    /// Should be linked into Localization.OnLanguageChanged event
    /// </summary>
    void ChangeLanguage()
    {
        Localization loc = Localization.GetLocalization();
        m_territoryObject.text = loc.m_ui.m_territoryText;
        m_heavyObject.text = loc.m_ui.m_heavyText;
        m_lightObject.text = loc.m_ui.m_lightText;
        m_livingObject.text = loc.m_ui.m_livingText;
    }

    /// <summary>
    /// get value of current storage type
    /// </summary>
    /// <param name="type"> type of storage </param>
    public int GetValue(ContainerType type)
    {
        switch (type)
        {
            case ContainerType.territory:
                return m_territory;
            case ContainerType.heavy:
                return _heavy;
            case ContainerType.light:
                return _light;
            case ContainerType.people:
                return _living;
            default:
                throw new System.Exception("Incorrect container type");
        }
    }

    /// <summary>
    /// Get value from container
    /// </summary>
    /// <param name="container"> list of containers </param>
    /// <returns> false, if value is bigger than all containers is the list</returns>
    bool CheckAndSet(List<Items> container, float value)
    {
        if (CalcStorage(container) < value)
            return false;

        for (int i = 0; i < container.Count && value > 0; i++)
        {
            float contValue = container[i].Count * container[i].m_containerEffect.m_value;
            if (contValue > value)
            {
                float tmpvalue = value / (int)container[i].m_containerEffect.m_value;
                container[i].Count -= tmpvalue;
                container[i].m_blocked += (tmpvalue);
                return true;
            }
            else
            {
                container[i].m_blocked += (contValue / (int)container[i].m_containerEffect.m_value);
                container[i].Count = 0;
                value -= contValue;
            }
        }
        return false;
    }

    /// <summary>
    /// Get value from container
    /// </summary>
    /// <param name="container"> container. Could be territory </param>
    /// <returns> false, if value is bigger than all containers is the list</returns>
    bool CheckAndSet(ref int container, float value)
    {
        if (container >= value)
        {
            container -= (int)value;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Get value from container
    /// </summary>
    /// <param name="type"> type of container </param>
    /// <returns> false, if value is bigger than all containers is the list</returns>
    public bool Get(ContainerType type, float value)
    {
        switch (type)
        {
            case ContainerType.territory:
                return CheckAndSet(ref m_territory, value);
            case ContainerType.heavy:
                return CheckAndSet(_heavyStorages, value);
            case ContainerType.light:
                return CheckAndSet(_lightStorages, value);
        }
        return false;
    }

    /// <summary>
    /// Add value to current container type
    /// </summary>
    public void AddValue(ContainerType type, int value)
    {
        switch (type)
        {
            case ContainerType.territory:
                m_territory += value;
                break;
            case ContainerType.heavy:
                _heavy += value;
                break;
            case ContainerType.light:
                _light += value;
                break;
        }
    }

    /// <summary>
    /// Add item into container list
    /// </summary>
    /// <param name="itm"> item for managing </param>
    public void AddStorageItem(Items itm)
    {
        try
        {
            if (itm.m_container == ContainerType.territory)
            {
                _buildingStorages.Add(itm);
                return;
            }
            switch (itm.m_containerEffect.m_containerType)
            {
                case ContainerType.heavy:
                    _heavyStorages.Add(itm);
                    return;
                case ContainerType.light:
                    _lightStorages.Add(itm);
                    return;
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Storage AddStorageItem exception" + ex.Message);
        }
    }

    /// <summary>
    /// calc how big value into current storage list
    /// </summary>
    /// <param name="itms"> storage list </param>
    /// <param name="isItTerritory"> should be true, if it is building </param>
    /// <returns></returns>
    int CalcStorage(List<Items> itms, bool isItTerritory = false)
    {
        int storage = 0;
        foreach (Items itm in itms)
            if (itm.m_containerEffect != null)
            {
                if (isItTerritory)
                {
                    storage += (int)((itm.Count + itm.m_blocked + itm.m_damagedCount) * itm.m_size);
                }
                else
                    storage += (int)(itm.Count * itm.m_containerEffect.m_value);
            }

        return storage;
    }

    /// <summary>
    /// initialization of living storage
    /// </summary>
    void InitLivingStorage()
    {
        foreach (var abs in Productions.GetTools("living"))
            m_livingStorages.Add(abs.m_toolLink as Items);
    }

    /// <summary>
    /// settle people in the best houses avaliable.
    /// If better house avaliable, a man will be moved into it
    /// </summary>
    void SettlePeople()
    {
        long population = m_people.PeopleNumber;
        for (int i = m_livingStorages.Count - 1; i > -1; i--)
        {
            Buildings bld = m_livingStorages[i] as Buildings;
            if (bld.m_living == null || bld.m_isItOpen < 1) continue;
            long maxPeople = (long)(bld.Count * bld.m_living.m_value);
            if (maxPeople > population)
            {
                bld.m_peopleLive = population;
                population = 0;
            }
            else
            {
                population -= maxPeople;
                bld.m_peopleLive = maxPeople;
            }
            float cnt = bld.Count;
            bld.m_living.m_toolsCount = (long)(bld.m_peopleLive / bld.m_living.m_value);
            if (bld.m_living.m_toolsCount * bld.m_living.m_value < bld.m_peopleLive)
                bld.m_living.m_toolsCount++;
            bld.Count = cnt;
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        try { 
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                if (m_livingStorages.Count == 0)
                    InitLivingStorage();

                _heavy = CalcStorage(_heavyStorages);
                _light = CalcStorage(_lightStorages);
                m_territory = m_territoryMax - CalcStorage(_buildingStorages, true);
                _living = CalcStorage(m_livingStorages);

                m_territoryText.text = m_territory.ToString();
                m_heavyText.text = _heavy.ToString();
                m_lightText.text = _light.ToString();
                m_livingText.text = _living.ToString();
                SettlePeople();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Storage update exception" + ex.Message);
        }
    }
}
