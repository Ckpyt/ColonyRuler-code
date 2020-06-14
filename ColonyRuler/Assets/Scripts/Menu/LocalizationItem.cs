using System;
using System.Collections.Generic;

/// <summary>
/// Class for localization item's description
/// </summary>
[Serializable]
public class LocalizationItem
{
    /// <summary> AbstractObject.m_name - cannot be changed </summary>
    public string m_name;
    /// <summary> AbstractObject.m_text - should be localized </summary>
    public string m_text;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">AbstractObject.m_name</param>
    /// <param name="text">AbstractObject.m_text</param>
    public LocalizationItem(string name, string text)
    {
        m_name = name;
        m_text = text;
    }


}

/// <summary>
/// Localize AbstractObject.text
/// </summary>
[Serializable]
public class ItemsLocalization
{
    /// <summary> List of text all items. Could be serialized by Unity </summary>
    public List<LocalizationItem> m_itemList = new List<LocalizationItem>();
    /// <summary> Dictionary for localization. Cannot be serialized by Unity </summary>
    [NonSerialized] public Dictionary<string, string> m_itemDictionary;

    public void Sort()
    {
        m_itemList.Sort((LocalizationItem pair1, LocalizationItem pair2) =>
                 pair1.m_name.CompareTo(pair2.m_name));
    }
}

/// <summary>
/// Holds history text data. For further development
/// </summary>
[Serializable]
public class HistoryLocalization
{

}
/// <summary>
/// List of supported languages
/// </summary>
[Serializable]
public class LanguagesList
{
    public List<string> m_languages = new List<string>();
}



/// <summary>
/// Hold all text data in UI.
/// Data should be be changed to current language
/// </summary>
[Serializable]
public class UiLocalization
{
    #region mainMenu
    public string m_newGame = "New Game";
    public string m_loadGame = "Load Game";
    public string m_saveGame = "Save Game";
    public string m_settings = "Settings";
    public string m_about = "About";
    public string m_exit = "Exit";

    public string m_resume = "Resume game";
    public string m_exitToMainMenu = "Return to main menu";
    public string m_hello = "Hello,";
    public string m_attention = "If you are not authorized on the site, everyone can use your save game..";

    #endregion
    #region save/load
    public string m_saveButton = "Save";
    public string m_loadButton = "Load";
    public string m_deleteButton = "Delete";
    public string m_rewriteButton = "Rewrite save";
    #endregion
    #region settings
    public string m_applyButton = "OK";
    public string m_cancelButton = "Cancel";
    public string m_restoreButton = "Restore defaults";
    public string m_fullTreeText = "Show full production tree";
    public string m_productivityLengthText = "Productivity queue duration \n (longer - shows more flat)";
    public string m_changeLanguageText = "Change language:";
    #endregion
    #region tooltips
    public string m_abstractCount = "How many items you have";
    public string m_abstractProductivity = "Productivity: how many was produced - how many was consumed";
    public string m_abstractDamaged = "not used";
    public string m_itemsDamaged = "How many items was damaged";
    public string m_scienceCount = "research progress";
    public string m_scienceProductivity = "Productivity: the percent of full loading research";
    public string m_resourceCount = "How many resources there";
    public string m_processCount = "How many fields completed \\ growing";


    public string m_emojiBoost = "Your people HAPPY!";
    public string m_emojiHappy = "Happy";
    public string m_emojiMaxHappy = "max Happy";
    public string m_emojiMaxPopulation = "max Population";
    public string m_starving = "Your people are starving! Feed them or they will die soon";
    public string m_timePause = "Pause";
    public string m_timePlus = "increase time speed";
    public string m_timeMinus = "decrease time speed";
    public string m_workersTooltip = "used for production something";
    public string m_populationTooltip = "how many people you have. Make them happy to attract someone new.";
    public string m_lightStorageTooltip = "used for upgrades small items productions";
    public string m_heavyStorageTooltip = "used for upgrade large size productions";
    public string m_livingSpaceTooltip = "How may people you will have";
    public string m_territoryTooltip = "used for construction of houses, depots, etc";
    #endregion
    #region game
    public string m_territoryText = "Territory";
    public string m_heavyText = "Heavy items storage";
    public string m_lightText = "Light items storage";
    public string m_livingText = "Living space";

    public string m_timeText = "Time:";
    public string m_dayText = "day:";
    public string m_yearText = "year:";
    public string m_pausedText = "paused";
    public string m_speedText = "speed";

    public string m_freeWorkersText = "Free workers:";
    public string m_populationText = "Population";
    #endregion
}



