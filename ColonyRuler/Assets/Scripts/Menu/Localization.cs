using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Class for localization. 
/// Holds all changeable text data for one language.
/// </summary>
[Serializable]
public class Localization : ISerializationCallbackReceiver
{
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

    /// <summary> Event of loading new current language </summary>
    public delegate void LanguageChanged();
    public static LanguageChanged m_onLanguageChanged;

    /// <summary> current language list </summary>
    [NonSerialized] public LanguagesList m_languages;
    /// <summary> index of m_languages </summary>
    public int m_currentLanguage;
    /// <summary> current HistoryLocalization </summary>
    [NonSerialized] public HistoryLocalization m_history;
    /// <summary> current ItemsLocalization </summary>
    [NonSerialized] public ItemsLocalization m_items;
    /// <summary> current UI Localization </summary>
    [NonSerialized] public UiLocalization m_ui;

    /// <summary> current language's localization  </summary>
    static Localization SCurrentLocalization { get; set; } = new Localization();

    /// <summary> language list file name. </summary>
    public const string CLangListPath = "langList.json";
    /// <summary> UI localization file name. Should be placed in language directory </summary>
    public const string CUiFileName = "UI";
    /// <summary> Items localization file name. Should be placed in language directory </summary>
    public const string CItemsFileName = "Items";
    /// <summary> History localization file name. Should be placed in language directory </summary>
    public const string CHistoryFileName = "History";

    /// <summary> full path to current localization of UI </summary>
    public static string m_sUiFullPath;
    /// <summary> full path to current localization of items </summary>
    public static string m_sItemsFullPath;
    /// <summary> full path to current localization of history </summary>
    public static string m_sHistoryFullPath;

    /// <summary>
    /// Part of Singleton
    /// </summary>
    /// <returns> current localization </returns>
    public static Localization GetLocalization()
    {
        return SCurrentLocalization;
    }

    /// <summary>
    /// Unity loading at start the game
    /// </summary>
    public static void Awake()
    {
        SCurrentLocalization.FirstLoad();
#if !UNITY_WEBGL && !UNITY_ANDROID
        CombinePath();
#endif
        SCurrentLocalization.Load();

    }

    /// <summary>
    /// Making all full paths
    /// </summary>
    public static void CombinePath()
    {
        m_sHistoryFullPath = Path.Combine(SCurrentLocalization.m_languages.
            m_languages[SCurrentLocalization.m_currentLanguage], CHistoryFileName);
        m_sItemsFullPath = Path.Combine(SCurrentLocalization.m_languages.
            m_languages[SCurrentLocalization.m_currentLanguage], CItemsFileName); 
        m_sUiFullPath = Path.Combine(SCurrentLocalization.m_languages.
            m_languages[SCurrentLocalization.m_currentLanguage], CUiFileName);
    }

    /// <summary>
    /// Only first initialization on the game startup
    /// </summary>
    public void FirstLoad()
    {
#if !UNITY_WEBGL && !UNITY_ANDROID
        if (!File.Exists(CLangListPath))
            CreateFiles();

        m_languages = JsonUtility.FromJson<LanguagesList>(File.ReadAllText(CLangListPath));
#else
        var ms = Camera.main.GetComponent<MainScript>();
        var networking = ms.m_networking;

        if(m_languages == null || m_languages.m_languages == null || m_languages.m_languages.Count == 0)
            networking.GetLocalizationLanguages(delegate (byte[] answer)
            {
                m_languages = JsonUtility.FromJson<LanguagesList>(NetworkManager.ByteToJson(answer));
                Load();
        });

#endif

    }

    /// <summary>
    /// Load current localization
    /// Part of ISerializationCallbackReceiver
    /// </summary>
    public void Load()
    {
        try
        {
#if UNITY_WEBGL || UNITY_ANDROID
            var ms = Camera.main.GetComponent<MainScript>();
            var networking = ms.m_networking;
            string loc = SCurrentLocalization.m_languages.m_languages[SCurrentLocalization.m_currentLanguage];

            networking.GetLocalization(loc, 1, delegate (byte[] answerUi)
            {
                m_ui = JsonUtility.FromJson<UiLocalization>(NetworkManager.ByteToJson(answerUi));
            });
            networking.GetLocalization(loc, 2, delegate (byte[] answerItm)
            {
                m_items = JsonUtility.FromJson<ItemsLocalization>(NetworkManager.ByteToJson(answerItm));
            });
            networking.GetLocalization(loc, 3, delegate (byte[] answerHis)
            {
                m_history = JsonUtility.FromJson<HistoryLocalization>(NetworkManager.ByteToJson(answerHis));
            });
#else
            Console.WriteLine("UnityWebGL does not works");
            var hisAss = Resources.Load<TextAsset>(m_sHistoryFullPath);
            m_history = JsonUtility.FromJson<HistoryLocalization>(hisAss.text);
            var itmAss = Resources.Load<TextAsset>(m_sItemsFullPath);
            m_items = JsonUtility.FromJson<ItemsLocalization>(itmAss.text);
            var uiAss = Resources.Load<TextAsset>(m_sUiFullPath);
            m_ui = JsonUtility.FromJson<UiLocalization>(uiAss.text);
#endif
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    /// <summary>
    /// Create EN localization from current
    /// </summary>
    public void CreateFiles()
    {
        m_languages = new LanguagesList();
        m_languages.m_languages.Add("EN_en");
        m_languages.m_languages.Add("RU_ru");
        File.WriteAllText(CLangListPath, JsonUtility.ToJson(m_languages));
        if (m_history == null)
            m_history = new HistoryLocalization();
        if (m_items == null)
            m_items = new ItemsLocalization();
        if (m_ui == null)
            m_ui = new UiLocalization();

        foreach (AbstractObject itm in AbstractObject.m_sEverything)
            m_items.m_itemList.Add(new LocalizationItem(itm.m_name, itm.m_text));

        CombinePath();
        File.WriteAllText(m_sHistoryFullPath + ".json", JsonUtility.ToJson(m_history));
        File.WriteAllText(m_sItemsFullPath + ".json", JsonUtility.ToJson(m_items));
        File.WriteAllText(m_sUiFullPath + ".json", JsonUtility.ToJson(m_ui));
    }

    /// <summary>
    /// Set default localization
    /// </summary>
    public void DefaultLocalization()
    {
        m_currentLanguage = 0;
    }

    /// <summary>
    /// Change current localization into new language
    /// </summary>
    /// <param name="newLang"> index on language list </param>
    public void ChangeLanguage(int newLang)
    {
        m_currentLanguage = newLang;
        Awake();

        if (m_items?.m_itemList != null)
        {
            m_items.m_itemDictionary = new Dictionary<string, string>();
            foreach (LocalizationItem itm in m_items.m_itemList)
                m_items.m_itemDictionary.Add(itm.m_name, itm.m_text);
        }

        Camera.main.GetComponent<MainScript>().ChangeLanguage();

        m_onLanguageChanged?.Invoke();

    }

    /// <summary>
    /// Part of ISerializationCallbackReceiver
    /// not used
    /// </summary>
    public void OnBeforeSerialize()
    {

    }

    /// <summary>
    /// Part of ISerializationCallbackReceiver
    /// not used
    /// </summary>
    public void OnAfterDeserialize()
    {

    }
}

