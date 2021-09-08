using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main menu class.
/// Two objects: main menu after start game and main menu from the game, constructed by Unity
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary> Link to mainScript </summary>
    MainScript _ms;
    /// <summary> Save Menu panel. Opens/Closes Save/Load Menu</summary>
    public GameObject m_savePanel;
    /// <summary> Settings Menu panel. Opens/Closes Settings Menu </summary>
    public GameObject m_settingsPanel;
    /// <summary> Link to start New Game button. For localization </summary>
    public Button m_newGame;
    /// <summary> Link to resume button. For localization </summary>
    public Button m_resumeGame;
    /// <summary> Link to Save Game button. For localization </summary>
    public Button m_saveGame;
    /// <summary> Link to Load Game button. For localization </summary>
    public Button m_loadGame;
    /// <summary> Link to Settings button. For localization </summary>
    public Button m_settings;
    /// <summary> Link to About button. For localization </summary>
    public Button m_about;
    /// <summary> Link to Return to main menu button. For localization </summary>
    public Button m_exitToMainMenu;
    /// <summary> Link to Exit button. For localization </summary>
    public Button m_exit;
    /// <summary> Username in MainMenu </summary>
    public static string m_sUserName = "Anonimus";
    /// <summary> User attention. Used on webGL version </summary>
    public Text m_userAttention;
    /// <summary> Currently active menu </summary>
    public static GameObject m_sActiveMenu;

    /// <summary>
    /// Setter for s_userName
    /// </summary>
    public void SetUserName(string username)
    {
        m_sUserName = username;
    }

    /// <summary>
    /// Close mainMenu and return to the game
    /// </summary>
    void Resume()
    {
        _ms.CloseMainMenu();
        m_sActiveMenu = null;
    }

    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public void ChangeLanguage()
    {
        m_loadGame.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_loadGame;
        m_settings.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_settings;

        if (name == "MainMenu")
        {
            m_newGame.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_newGame;
            m_about.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_about;
            m_exit.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_exit;
        }
        else
        {
            m_resumeGame.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_resume;
            m_exitToMainMenu.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_exitToMainMenu;
            m_saveGame.transform.Find("Text").gameObject.GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_saveGame;
        }
    }

    /// <summary>
    /// Open save menu panel
    /// </summary>
    public void Save()
    {
        gameObject.SetActive(false);
        m_savePanel.GetComponent<Save>().ShowMenu(gameObject);
        m_savePanel.GetComponent<Save>().ShowSave();
    }

    /// <summary>
    /// Open save menu panel as load menu
    /// Used in Unity button
    /// </summary>
    public void MainMenuLoad()
    {
        m_savePanel.GetComponent<Save>().m_isItSave = false;
        m_savePanel.GetComponent<Save>().ShowMenu(gameObject);
    }

    /// <summary>
    /// Open save menu panel as load menu
    /// </summary>
    public void Load()
    {
        m_savePanel.GetComponent<Save>().m_isItSave = false;
        m_savePanel.GetComponent<Save>().ShowMenu(gameObject);
    }

    /// <summary>
    /// Close the application.
    /// Used in Unity
    /// </summary>
    public void ExitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }

    /// <summary>
    /// Called in Unity before loading.
    /// First initialization
    /// </summary>
    void Awake()
    {
        m_sActiveMenu = gameObject;
        Settings.Awake();
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Second initialization
    /// </summary>
    void Start()
    {
        m_sActiveMenu = gameObject;
        try
        {
#if UNITY_WEBGL
            Console.WriteLine("UnityWebGL works");
#else
            Console.WriteLine("UnityWebGL does not works");
#endif
            SetUserName(NewBehaviourScript.GetUserName());
            _ms = Camera.main.GetComponent<MainScript>();
            Transform rsm = transform.Find("Resume");
            if (rsm != null)
            {
                m_resumeGame = rsm.GetComponent<Button>();
                m_resumeGame.onClick.AddListener(Resume);
                m_exit = transform.Find("Exit").GetComponent<Button>();
                m_exit.onClick.AddListener(_ms.FinishGame);
                m_saveGame = transform.Find("SaveGame").GetComponent<Button>();
                m_saveGame.onClick.AddListener(Save);
                m_loadGame = transform.Find("LoadGame").GetComponent<Button>();
                m_loadGame.onClick.AddListener(Load);

            }
            Settings.SettingsLoad(gameObject);
        }
        catch (Exception ex)
        {
            Debug.LogError("MainMenu Start exception:" + ex.Message);
        }
    }


    /// <summary>
    /// Making user attention
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        m_sActiveMenu = this.gameObject;
        try
        {
            if (m_userAttention != null)
            {
#if UNITY_WEBGL
                Localization loc = Localization.GetLocalization();
                if (loc.m_ui != null)
                {
                    string attention = "";
                    if (m_sUserName.CompareTo("Not authorized") == 0)
                        attention = loc.m_ui.m_attention;

                    m_userAttention.text = loc.m_ui.m_hello + " " + m_sUserName + "\n" + attention;
                    m_userAttention.gameObject.SetActive(true);
                }
#else
                m_userAttention.text = "";
#endif
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MainMenu Update exception:" + ex.Message);
        }

    }
}
