using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SaveGame.
/// Used for save/load game
/// </summary>
[Serializable]
class Game
{
    /// <summary>
    /// Used for store coordinats of IconScript
    /// IconScript cannot be serialized as a children of MonoBehaviour
    /// </summary>
    [Serializable]
    public class GameIcon
    {
        /// <summary> icon position </summary>
        public Vector3 m_pos;
        /// <summary> AbstractObject.m_name </summary>
        public string m_itmName;
    }

    #region All non-abstract data 
    public List<Items> m_allItems = new List<Items>();
    public List<GameMaterial> m_allMaterails = new List<GameMaterial>();
    public List<Science> m_allScience = new List<Science>();
    public List<Buildings> m_allBuildings = new List<Buildings>();
    public List<Army> m_allArmy = new List<Army>();
    public List<Resource> m_allResources = new List<Resource>();
    public List<Process> m_allProcesses = new List<Process>();
    /// <summary> where is ony one population </summary>
    public Population m_population = null;
    #endregion

    /// <summary> list of all icon's coordinats </summary>
    public List<GameIcon> m_allGameIcons = new List<GameIcon>();

    /// <summary> current time speed </summary>
    public float m_speed = 1.0f;
    /// <summary> current day </summary>
    public int m_day = 0;
    /// <summary> current year </summary>
    public int m_year = 0;

    /// <summary>
    /// Save the game to the json string.
    /// String should be written into a file or send to server
    /// </summary>
    /// <returns>game save</returns>
    public string Save()
    {
        foreach (AbstractObject itm in AbstractObject.m_sEverything)
            switch (itm.GetType().FullName)
            {
                case nameof(Science): m_allScience.Add(itm as Science); break;
                case nameof(Population): m_population = itm as Population; break;
                case nameof(Items): m_allItems.Add(itm as Items); break;
                case nameof(GameMaterial): m_allMaterails.Add(itm as GameMaterial); break;
                case nameof(Buildings): m_allBuildings.Add(itm as Buildings); break;
                case nameof(Army): m_allArmy.Add(itm as Army); break;
                case nameof(Resource): m_allResources.Add(itm as Resource); break;
                case nameof(Process): m_allProcesses.Add(itm as Process); break;
            }

        foreach (GameObject go in MainScript.m_sAllItems)
        {
            IconScript ics = go.GetComponent<IconScript>();
            GameIcon gic = new GameIcon();
            gic.m_pos = ics.transform.position;
            gic.m_itmName = ics.m_thisItem.m_name;
            m_allGameIcons.Add(gic);
        }

        TimeScript tsc = Camera.main.GetComponent<TimeScript>();
        m_speed = tsc.m_speed;
        m_day = tsc.m_day;
        m_year = tsc.m_year;

        string json = JsonUtility.ToJson(this);
        return json;
    }

    /// <summary>
    /// Load Game class from json string.
    /// string should be read from file or recived from server.
    /// Also, Game class should be implemented to the game
    /// </summary>
    /// <param name="json"> input string </param>
    /// <returns>Game object. Not implemented!</returns>
    public static Game Load(string json)
    {
        string dataAsJson = json;
        Game gm = JsonUtility.FromJson<Game>(dataAsJson);
        return gm;
    }
}

