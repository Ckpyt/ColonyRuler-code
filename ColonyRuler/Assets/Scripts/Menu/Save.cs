using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Save / load menu
/// </summary>
public class Save : SubMenu
{
    /// <summary> Link to main script </summary>
    MainScript _ms;
    /// <summary> button for selecting save name </summary>
    public Button m_fileListButton;
    /// <summary> save name input </summary>
    public InputField m_inputFileName;
    /// <summary> Link to main Menu </summary>
    public MainMenu m_mainMenu;
    /// <summary> Save / load button </summary>
    public Button m_saveButton;
    /// <summary> delete save button </summary>
    public Button m_deleteButton;
    /// <summary> all buttons with save names </summary>
    List<Button> _allButtons = new List<Button>();

    /// <summary> Is it save game menu? if not, it is load game menu</summary>
    public bool m_isItSave = true;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _ms = Camera.main.GetComponent<MainScript>();
        }
        catch (Exception ex)
        {
            Debug.LogError("Save Start Exception" + ex.Message);
        }
    }

    /// <summary>
    /// Set file name as text to button in saves file names list
    /// </summary>
    /// <param name="path"> file name </param>
    /// <param name="btn"> button with file name text </param>
    void SetFilename(string path, Button btn)
    {
        GameObject textObj = btn.transform.Find("Text").gameObject;
        string filename = Path.GetFileName(path);
        filename = " " + filename.Split('.')[0];
        textObj.GetComponent<Text>().text = filename;
    }

    /// <summary>
    /// Set default file name to input field
    /// </summary>
    void SetDefaultValue()
    {
        if (m_isItSave)
            m_inputFileName.text = DateTime.Now.ToString()
                .Replace(':', '_').Replace('.', '_').Replace(',', '_');
        else
        {
            if (_allButtons.Count > 0)
                m_inputFileName.text = _allButtons[0].transform.Find("Text")
                    .GetComponent<Text>().text;
            else
                m_inputFileName.text = "";
        }
    }

    /// <summary>
    /// Showing current menu and hide previous
    /// </summary>
    /// <param name="previous"></param>
    public override void ShowMenu(GameObject previous)
    {
        base.ShowMenu(previous);
        ShowLoad(previous);
    }

    /// <summary>
    /// Showing the menu as load game menu
    /// </summary>
    /// <param name="previous"> previous game menu </param>
    public void ShowLoad(GameObject previous)
    {
        Localization loc = Localization.GetLocalization();
        m_inputFileName.readOnly = true;
        MakeList();
        m_isItSave = false;
        m_saveButton.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_loadButton;
        m_deleteButton.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_deleteButton;
    }

    /// <summary>
    /// Showing the menu as save game menu
    /// </summary>
    public void ShowSave()
    {
        MainScript ms = Camera.main.GetComponent<MainScript>();
        Localization loc = Localization.GetLocalization();
        m_inputFileName.readOnly = false;
        MakeList();
        m_isItSave = true;
        m_saveButton.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_saveButton;
        m_deleteButton.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_deleteButton;
    }

    /// <summary>
    /// Correct number of buttons with file names into file name list
    /// </summary>
    /// <param name="size"> new size </param>
    void CorrectButtonList(int size)
    {
        if (_allButtons.Count == 0)
            _allButtons.Add(m_fileListButton);

        if (size > _allButtons.Count)
        {
            int i = _allButtons.Count - 1;
            Vector3 pos = _allButtons[i].transform.position;
            while (i < size)
            {
                pos.y -= 30;
                Button newBtn = Instantiate(m_fileListButton);
                newBtn.transform.parent = m_fileListButton.transform.parent;
                newBtn.transform.position = pos;
                _allButtons.Add(newBtn);
                i++;
            }
        }
        else
        {
            size--;
            int i = _allButtons.Count - 1;
            while (i > size)
            {
                _allButtons[i].gameObject.SetActive(false);
                i--;
            }
        }
    }

    /// <summary>
    /// make list of file names in save/load menu
    /// </summary>
    /// <param name="filenames"> all file names </param>
    void MakeList(string[] filenames)
    {
        int len = 0;
        if (filenames.Length > 0)
        {
            foreach (string fls in filenames)
                if (fls.Length > 0) // it could be 0!
                    len++;
            CorrectButtonList(len);

            for (int i = 0; filenames.Length > i; i++)
            {
                if (filenames[i].Length > 0)
                {
                    _allButtons[i].gameObject.SetActive(true);
                    SetFilename(filenames[i], _allButtons[i]);
                }
            }

        }
    }

    /// <summary>
    /// make list of file names in save/load menu
    /// </summary>
    public void MakeList()
    {
        string[] filenames = null;
#if UNITY_WEBGL
        var networking = Camera.main.GetComponent<MainScript>().m_networking;

        networking.GetText(MainMenu.m_sUserName, 2, ResiveFilename);
#else
        SetDefaultValue();
        filenames = Directory.GetFiles(Application.persistentDataPath, "*.sav");
        MakeList(filenames);
#endif


    }

    /// <summary>
    /// receive file names from server.
    /// </summary>
    /// <param name="res"> answer </param>
    void ResiveFilename(string res)
    {
        string[] filenames = null;
        string txt = res.Replace("\"", "");
        filenames = txt.Split(',');
        MakeList(filenames);
    }

    /// <summary>
    /// Delete current choosed save.
    /// </summary>
    public void DeleteFile()
    {
#if UNITY_WEBGL
        NetworkManager.SetText(MainMenu.m_sUserName, 2, "  ");
#else
        File.Delete(Application.persistentDataPath + "/" + m_inputFileName.text + ".sav");
#endif
        MakeList();
        m_saveButton.transform.Find("Text").GetComponent<Text>().text = m_isItSave ?
            Localization.GetLocalization().m_ui.m_saveButton :
            Localization.GetLocalization().m_ui.m_loadButton;
    }

    /// <summary>
    /// Select file name from list
    /// Event, called by Unity
    /// </summary>
    /// <param name="sender"> who send Event </param>
    public void SelectItem(Button sender)
    {
        m_inputFileName.text = sender.transform.Find("Text").gameObject.GetComponent<Text>().text;
        if (m_isItSave)
            m_saveButton.transform.Find("Text").GetComponent<Text>().text =
                Localization.GetLocalization().m_ui.m_rewriteButton;
    }

    /// <summary>
    /// Copy items from loaded to already exist in the game
    /// </summary>
    /// <typeparam name="T"> class of item, AbstractObject child  </typeparam>
    /// <param name="list"> item's list for copying </param>
    void CopyItems<T>(List<T> list) where T : AbstractObject
    {
        foreach (AbstractObject lItm in list)
        {
            foreach (AbstractObject itm in AbstractObject.m_sEverything)
                if (lItm.m_name == itm.m_name && itm.GetType() == lItm.GetType())
                {
                    itm.Copy(lItm);
                    break;
                }
        }
    }

    /// <summary>
    /// Copy items effect from loaded save game to already exists effects
    /// </summary>
    /// <typeparam name="T"> Item. GameMaterial or child </typeparam>
    /// <param name="list"> Item's list for copying </param>
    void CopyTools<T>(List<T> list) where T : GameMaterial
    {
        foreach (GameMaterial lItm in list)
        {
            foreach (AbstractObject itm in AbstractObject.m_sEverything)
            {
                GameMaterial mat = itm as GameMaterial;
                if (mat != null && lItm.m_name == itm.m_name && itm.GetType() == lItm.GetType())
                {
                    mat.CopyTools(lItm);
                    break;
                }
            }
        }

    }

    /// <summary>
    /// load game from json string
    /// !!! move it to Game !!!
    /// </summary>
    void Load(string json)
    {
        Game gm = Game.Load(json);
        MainScript ms = Camera.main.GetComponent<MainScript>();
        ms.ClearEverything();
        ms.Loading();

        CopyItems(gm.m_allArmy);
        CopyItems(gm.m_allBuildings);
        CopyItems(gm.m_allItems);
        CopyItems(gm.m_allMaterails);
        CopyItems(gm.m_allScience);
        CopyItems(gm.m_allResources);
        CopyItems(gm.m_allProcesses);
        CopyItems(gm.m_allWildAnimals);
        CopyItems(gm.m_allDomestic);

        AbstractObject.m_sEverything[AbstractObject.m_sEverything.Count - 1].
            Copy(gm.m_population);

        ms.PlaceOpenedItems(AbstractObject.GetOpennedItems());

        CopyTools(gm.m_allArmy);
        CopyTools(gm.m_allBuildings);
        CopyTools(gm.m_allItems);
        CopyTools(gm.m_allMaterails);
        CopyTools(gm.m_allProcesses);

        foreach (GameObject go in MainScript.m_sAllItems)
        {
            try
            {
                IconScript ics = go.GetComponent<IconScript>();
                Game.GameIcon gic = null;
                foreach (var itm in gm.m_allGameIcons)
                    if (itm.m_itmName == ics.m_thisItem.m_name)
                    {
                        gic = itm;
                        break;
                    }
                if (gic == null) continue;//throw new System.Exception("Object not found");
                gm.m_allGameIcons.Remove(gic);
                ics.transform.position = gic.m_pos;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

#if !UNITY_WEBGL
        Localization.GetLocalization().ChangeLanguage(Settings.GetSettings().m_localization.m_currentLanguage);
#endif
        TimeScript tsc = Camera.main.GetComponent<TimeScript>();
        tsc.m_day = gm.m_day;
        tsc.m_year = gm.m_year;
        tsc.m_speed = gm.m_speed;

        LearningTip.s_canShow = gm.m_canShowTooltips;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Load game from server answer
    /// </summary>
    void Load(byte[] answer)
    {
        if (answer != null && answer.Length > 2)
        {
            string json = Encoding.UTF8.GetString(answer);
            json = json.Replace("\\", "");
            json = json.Remove(json.Length - 2, 2).Remove(0, 1);
            Load(json);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// load the game
    /// </summary>
    void Load()
    {
#if UNITY_WEBGL
        NetworkManager networking = _ms.m_networking;
        networking.GetText(MainMenu.m_sUserName, m_inputFileName.text.TrimStart(), Load);
#else
        string path = Application.persistentDataPath + "/" + m_inputFileName.text + ".sav";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Load(json);
        }
#endif

    }

    /// <summary>
    /// Event, called from Unity.
    /// Save/Load game
    /// </summary>
    public void SaveGame()
    {
        if (m_isItSave)
        {
            Game gm = new Game();
            string json = gm.Save();
            string path = "";
#if UNITY_WEBGL
            path = m_inputFileName.text;
            NetworkManager.SetText(MainMenu.m_sUserName, path, json);
#else
            path = Application.persistentDataPath + "/" + m_inputFileName.text + ".sav";
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, json);
#endif

        }
        else
        {
            if (!MainScript.m_isItInitialized)
                _ms.StartGame();
            Load();
        }
        MainScript.m_sIsButtonPressed = false;
        Resume();
        _ms.CloseMainMenu();
        if (m_previous != null && !ReferenceEquals(m_previous.gameObject, null))
            m_previous.SetActive(false);
    }
}
