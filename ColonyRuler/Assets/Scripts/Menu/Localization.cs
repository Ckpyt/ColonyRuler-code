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
        SCurrentLocalization.Load();
#endif
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

        if (m_languages == null || m_languages.m_languages == null)
        {
            var ms = Camera.main.GetComponent<MainScript>();
            var networking = ms.m_networking;

            networking.GetLocalizationLanguages(delegate (string answer)
            {
                m_languages = JsonUtility.FromJson<LanguagesList>(NetworkManager.ByteToJson(answer));
                Load();
            });
        }

#endif

    }

    /// <summary>
    /// filling m_items.m_itemDictionary from description list
    /// </summary>
    void FillItemDictionary()
    {
        m_items.m_itemDictionary = new Dictionary<string, string>();
        foreach (LocalizationItem itm in m_items.m_itemList)
            m_items.m_itemDictionary.Add(itm.m_name, itm.m_text);
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
            networking.GetLocalization(loc, 1, delegate (string answerUi)
            {
                m_ui = JsonUtility.FromJson<UiLocalization>(NetworkManager.ByteToJson(answerUi));
            });
            networking.GetLocalization(loc, 2, delegate (string answerItm)
            {
                m_items = new ItemsLocalization();
                m_items = JsonUtility.FromJson<ItemsLocalization>(NetworkManager.ByteToJson(answerItm));

                if (m_items?.m_itemList != null)
                {
                    FillItemDictionary();
                }
            });
            networking.GetLocalization(loc, 3, delegate (string answerHis)
            {
                m_history = JsonUtility.FromJson<HistoryLocalization>(NetworkManager.ByteToJson(answerHis));
                m_onLanguageChanged?.Invoke();
            });
#else
            Console.WriteLine("UnityWebGL does not works");
            var hisAss = Resources.Load<TextAsset>(m_sHistoryFullPath);
            m_history = JsonUtility.FromJson<HistoryLocalization>(hisAss.text);
            var itmAss = Resources.Load<TextAsset>(m_sItemsFullPath);
            m_items = JsonUtility.FromJson<ItemsLocalization>(itmAss.text);
            FillItemDictionary();
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

        m_items.m_itemDictionary = new Dictionary<string, string>();
        foreach (LocalizationItem itm in m_items.m_itemList)
            m_items.m_itemDictionary.Add(itm.m_name, itm.m_text);

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
        if (newLang != m_currentLanguage)
        {
            m_currentLanguage = newLang;
            Awake();
            Load();

            

            Camera.main.GetComponent<MainScript>().ChangeLanguage();

#if UNITY_WEBGL || UNITY_ANDROID
#else
            m_items.m_itemDictionary = new Dictionary<string, string>();
            foreach (LocalizationItem itm in m_items.m_itemList)
                m_items.m_itemDictionary.Add(itm.m_name, itm.m_text);
            m_onLanguageChanged?.Invoke();
#endif

        }
        else
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

