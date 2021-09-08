using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Class for showing/applying settings
/// </summary>
[Serializable]
public class Settings
{
    /// <summary> Is it full production tree should be displayed or only one step? </summary>
    public bool m_isItFullTree = false;
    /// <summary> How many days should be remembered by production queue </summary>
    public int m_productQueueLimit = 10;
    /// <summary> current localization </summary>
    public Localization m_localization;

    /// <summary> default value for m_isItFullTree </summary>
    const bool CIsItFullTreeDefault = false;
    /// <summary> default value for m_productQueueLimit </summary>
    const int CProductQueueLimitDefault = 10;

    /// <summary> filename of settings </summary>
    static string _sFilePath = "/settings.json";
    /// <summary> singleton </summary>
    static Settings _sSettings = new Settings();

    /// <summary>
    /// A first initialization, called by Unity
    /// </summary>
    static public void Awake()
    {
        _sFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        Localization.Awake();
    }

    /// <summary>
    /// Part of singleton. 
    /// Can be only one settings object in the game
    /// </summary>
    public static Settings GetSettings()
    {
        return _sSettings;
    }

    /// <summary>
    /// Save settings to file/server
    /// </summary>
    public void SettingsSave()
    {
        _sSettings.m_localization = Localization.GetLocalization();
        string json = JsonUtility.ToJson(this);
#if UNITY_WEBGL

        NetworkManager.SetText(MainMenu.m_sUserName, 1, json);
        
#else
        File.WriteAllText(_sFilePath, json);
#endif
    }

    /// <summary>
    /// load settings from json string
    /// </summary>
    static void SettingsLoad(string json)
    {
        if (json.Length > 4)
        {
            _sSettings = JsonUtility.FromJson<Settings>(json);
            Localization.GetLocalization().ChangeLanguage(_sSettings.m_localization.m_currentLanguage);
            _sSettings.m_localization = Localization.GetLocalization();
        }
        else
        {
            Localization.GetLocalization().DefaultLocalization();
            _sSettings.m_localization = Localization.GetLocalization();
        }

        _sSettings.ApplySettings();
    }

    /// <summary>
    /// Event of reciving answer from server
    /// </summary>
    /// <param name="answer"> answered string for loading settings </param>
    static void Load(string answer)
    {
        SettingsLoad(NetworkManager.ByteToJson(answer));
    }

    /// <summary>
    /// Load settings from file/server
    /// </summary>
    /// <param name="menu">previous menu</param>
    public static void SettingsLoad(GameObject menu)
    {
        string json = "";
#if UNITY_WEBGL
        MainScript ms = Camera.main.GetComponent<MainScript>();
        var networking = ms.m_networking;

        networking.GetText(MainMenu.m_sUserName, 1, Load);
#else
        if (File.Exists(_sFilePath))
        {
            json = File.ReadAllText(_sFilePath);
        }
        SettingsLoad(json);
#endif

    }

    /// <summary>
    /// restore default settings
    /// </summary>
    public void RestoreDefaults()
    {
        m_isItFullTree = CIsItFullTreeDefault;
        m_productQueueLimit = CProductQueueLimitDefault;
        m_localization.DefaultLocalization();

        SettingsSave();
    }

    /// <summary>
    /// Apply settings and close menu
    /// </summary>
    public void ApplySettings()
    {
        IconScript.UnselectSelectedIcon();
        GameMaterial.m_sProductQueueLimit = m_productQueueLimit;
        IconScript.m_sShowFullTree = m_isItFullTree;
        IconScript.SelectSelectedIcon();
        SettingsSave();
    }


}

