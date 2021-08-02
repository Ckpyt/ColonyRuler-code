using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// 
/// Here and everywhere else, I'm using some naming rules:
/// m_ - public member
/// _ - private member
/// C - constant
/// _s - static

/// <summary>
/// The main script of the game. Holds some specific data, starting initialization,
/// </summary>
public class MainScript : MonoBehaviour
{
    /// <summary> Link to network manager </summary>
    public NetworkManager m_networking;
    /// <summary> Is any button on icons pressed? </summary>
    public static bool m_sIsButtonPressed = false;
    /// <summary> Link to icon prefab. For making new icons</summary>
    public GameObject m_iconPrefab = null;
    /// <summary> Link to arrow prefab. For making new arrows</summary>
    public GameObject m_arrowPrefab = null;
    /// <summary> Link to left panel prefab. For creating left panel</summary>
    public GameObject m_leftGamePanelPrefab = null;
    /// <summary> Link to right panel prefab. For creating right panel</summary>
    public GameObject m_rightGamePanelPrefab = null;
    /// <summary> Link to arrow legend panel prefab.</summary>
    public GameObject m_arrowLegendPanelPrefab = null;
    /// <summary> Link to Learning Tip prefab. For creating learning tips</summary>
    public GameObject m_LearingTipPrefab = null;
    /// <summary> Link to outline prefab. For creating rectangle around a gameObject</summary>
    public GameObject m_OutlinePrefab = null;
    /// <summary> Link to Canvas of Main camera. Only one in the game</summary>
    public Canvas m_mainCanvas = null;
    /// <summary> collection of all game objects of icons</summary>
    public static List<GameObject> m_sAllItems;
    /// <summary> Link to tooltip object. Only one in the game</summary>
    public Text m_toolTipText = null;
    /// <summary> Range there new icons, with does not has default coordinates, could be placed. Could be increased</summary>
    float _range = 30f;
    /// <summary> Link to left panel. Contain information about population</summary>
    Image _leftPanel = null;
    /// <summary> Link to right panel. Contain information about depots</summary>
    Image _rightPanel = null;
    /// <summary> Link to arrow legend panel. Contain information about arrow colors</summary>
    Image _arrowPanel = null;
    /// <summary> Link to second menu panel</summary>
    public Image m_gameMenuPanel = null;
    /// <summary> camera position. For saving/loading</summary>
    Vector3 _savedPos = new Vector3(0, 0, -10);
    /// <summary> Link to main menu panel</summary>
    public GameObject m_mainMenuPanel = null;
    /// <summary> is game started and initialized? </summary>
    public static bool m_isItInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Localization.m_onLanguageChanged += ChangeLanguage;
    }

    /// <summary>
    /// Method for change text object then current language has been changed
    /// Should be linked into Localization.OnLanguageChanged event
    /// </summary>
    public void ChangeLanguage()
    {
        m_gameMenuPanel.GetComponent<MainMenu>().ChangeLanguage();
        m_mainMenuPanel.GetComponent<MainMenu>().ChangeLanguage();
    }

    /// <summary>
    /// Event of main menu closing.
    /// Should be switched to game panels
    /// </summary>
    public void CloseMainMenu()
    {
        _leftPanel.gameObject.SetActive(true);
        _rightPanel.gameObject.SetActive(true);
        m_gameMenuPanel.gameObject.SetActive(false);
        Camera.main.GetComponent<TimeScript>().Pause();
        transform.position = _savedPos;
    }

    /// <summary>
    /// Destroy all icons and arrows in the game
    /// </summary>
    public void ClearEverything()
    {
        AbstractObject.ClearEvrething();
        if (m_sAllItems != null)
        {
            foreach (var go in m_sAllItems)
                DestroyImmediate(go);
            m_sAllItems.Clear();
        }
        else
        {
            m_sAllItems = new List<GameObject>();
        }
        ArrowScript.DestroyAllObjects();
    }

    /// <summary>
    /// game finalization.
    /// All game objects, all game components should be destroyed
    /// </summary>
    public void FinishGame()
    {
        ClearEverything();
        Destroy(GetComponent<People>());
        Destroy(GetComponent<TimeScript>());
        Destroy(GetComponent<CameraScript>());
        Destroy(GetComponent<Storage>());
        Destroy(_leftPanel.gameObject);
        Destroy(_rightPanel.gameObject);
        Destroy(_arrowPanel.gameObject);
        m_gameMenuPanel.gameObject.SetActive(false);
        m_mainMenuPanel.SetActive(true);
        m_isItInitialized = false;
    }

    /// <summary>
    /// loading data from xml files
    /// </summary>
    public void Loading()
    {

        //order is important! from parents to children, science the last one
        EffectTypeHolder.Load("effect_map");
        MineralResource.Load("mineralResource_Map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Resource.Load("resource_Map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        GameMaterial.Load("materials_Map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Items.Load("items_Map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Buildings.Load("buildings_Map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Army.Load("army_map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Process.Load("process_map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        Science.Load("science_map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        DomesticAnimal.Load("domesticAnimal_map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());
        WildAnimal.Load("wildAnimal_map");
        //Debug.Log("all items amount:" + GameAbstractItem.ItemsCount());

        LearningTip.Load("AllTips_Map");

        GameAbstractItem.ParseDependency();
        Population ppl = new Population {m_people = GetComponent<People>(), m_isItOpen = 1};
        AbstractObject.m_sEverything.Add(ppl);
        Localization.GetLocalization().FirstLoad();
        Localization.GetLocalization().ChangeLanguage(Localization.GetLocalization().m_currentLanguage);
        AbstractObject.ClearUnparsed();

        Debug.Log("loading finished");

    }


    /// <summary>
    /// Checking components of main camera.
    /// If component is not exist, it will be added
    /// </summary>
    /// <typeparam name="T"> component class </typeparam>
    /// <returns> link to component </returns>
    public T CheckComponent<T>() where T : Component
    {
        var comp = GetComponent<T>();
        if (comp == null)
        {
            comp = gameObject.AddComponent(typeof(T)) as T;
        }
        return comp;
    }

    /// <summary>
    /// Showing second main menu.
    /// Game panels should be closed, camera should be moved to empty space
    /// </summary>
    public void ShowMainMenu()
    {
        _leftPanel.gameObject.SetActive(false);
        _rightPanel.gameObject.SetActive(false);
        m_gameMenuPanel.gameObject.SetActive(true);
        Camera.main.GetComponent<TimeScript>().Pause();
        var pos = new Vector3(1000, 1000) {z = transform.position.z};
        _savedPos = transform.position;
        transform.position = pos;
    }

    /// <summary>
    /// Game initialization.
    /// Loading data, placing objects
    /// </summary>
    public void GameInitialization()
    {

        if (m_sAllItems == null)
            m_sAllItems = new List<GameObject>();
        m_mainMenuPanel.SetActive(false);
        MainMenu.m_sActiveMenu = null;

        transform.position = new Vector3(0, 0, -10);
        _range = 30f;
        GameObject arrowPanel = Instantiate(m_arrowLegendPanelPrefab);
        arrowPanel.transform.SetParent(m_mainCanvas.transform);
        _arrowPanel = arrowPanel.GetComponent<Image>();
        _arrowPanel.rectTransform.anchoredPosition = new Vector2(-53, -32f);

        GameObject rightPanel = Instantiate(m_rightGamePanelPrefab);
        GameObject leftPanel = Instantiate(m_leftGamePanelPrefab);
        rightPanel.transform.SetParent(m_mainCanvas.transform);
        GameObject rootRightPanel = rightPanel.transform.Find("Root").gameObject;

        leftPanel.transform.SetParent(m_mainCanvas.transform);
        m_toolTipText = leftPanel.transform.Find("TooltipText").gameObject.GetComponent<Text>();

        People pl = CheckComponent<People>();

        pl.m_publicEmoji = leftPanel.transform.Find("Emoji").GetComponent<Image>();
        pl.m_hungry = leftPanel.transform.Find("Hungry").gameObject.GetComponent<Image>();
        var leftColorPanel = leftPanel.transform.Find("ImageTimeText");
        pl.m_peopleText = leftColorPanel.transform.Find("PopulationValue").
            gameObject.GetComponent<Text>();
        pl.m_workersText = leftColorPanel.transform.Find("WorkersValue").
            gameObject.GetComponent<Text>();

        CameraScript csc = CheckComponent<CameraScript>();
        TimeScript tsc = CheckComponent<TimeScript>();
        TimeScript.m_isItPaused = true;
        tsc.m_timeText = leftColorPanel.transform.Find("TimeText").GetComponent<Text>();
        tsc.Initialization(leftPanel);
        pl.m_timeScr = tsc;

        Storage str = CheckComponent<Storage>();
        str.m_territoryText = rootRightPanel.transform.Find("TerritoryValue").
            gameObject.GetComponent<Text>();
        str.m_heavyText = rootRightPanel.transform.Find("HeavyStorageValue").
            gameObject.GetComponent<Text>();
        str.m_lightText = rootRightPanel.transform.Find("LightStorageValue").
            gameObject.GetComponent<Text>();
        str.m_livingText = rootRightPanel.transform.Find("LivingSpaceValue").
            gameObject.GetComponent<Text>();

        str.m_territoryObject = rootRightPanel.transform.Find("TerritoryText").
            gameObject.GetComponent<Text>();
        str.m_heavyObject = rootRightPanel.transform.Find("HeavyStorageText").
            gameObject.GetComponent<Text>();
        str.m_lightObject = rootRightPanel.transform.Find("LightStorageText").
            gameObject.GetComponent<Text>();
        str.m_livingObject = rootRightPanel.transform.Find("LivingSpaceText").
            gameObject.GetComponent<Text>();
        str.m_people = pl;
        pl.m_storage = str;

        Image lft = leftPanel.GetComponent<Image>();
        lft.rectTransform.anchoredPosition = new Vector2(191.45f, -86f);
        Image rht = rightPanel.GetComponent<Image>();
        rht.rectTransform.anchoredPosition = new Vector2(-125.1f, -35f);
        _leftPanel = lft;
        _rightPanel = rht;

        Button settings = lft.transform.Find("Settings").gameObject.GetComponent<Button>();
        settings.onClick.AddListener(ShowMainMenu);
        TimeScript.m_isItPaused = false;

    }

    /// <summary>
    /// Starting new game
    /// </summary>
    public void StartGame()
    {
        m_isItInitialized = false;
        GameInitialization();
        Loading();
        PlaceOpenedItems(AbstractObject.GetOpennedItems());
        m_isItInitialized = true;

        LearningTip.CreateTip("tip1");
    }

    /// <summary>
    /// Find empty space for new icon
    /// </summary>
    /// <param name="x"> return x coordinate </param>
    /// <param name="y"> return y coordinate</param>
    public void FindEmptySpace(out float x, out float y)
    {
        float distance = 5f;
        bool distOk;
        do
        {
            distOk = true;
            float tmpRange = _range * _range;
            x = Mathf.Sqrt(UnityEngine.Random.value * tmpRange) - _range / 2;
            y = Mathf.Sqrt(UnityEngine.Random.value * tmpRange) - _range / 2;

            foreach (GameObject gob in m_sAllItems)
                distOk &= distance < Vector2.Distance(gob.transform.position,
                    new Vector3(x, y, gob.transform.position.z));

            distance -= 0.1f;
        } while (!distOk);

        if (distance < 4.5f)
            _range += 10f;
    }

    /// <summary>
    /// place new icons on the screen and make arrows
    /// </summary>
    /// <param name="items"> new icons collection </param>
    public void PlaceOpenedItems(List<AbstractObject> items)
    {
        foreach (var itm in items)
        {
            if (itm.m_isItOpen < 1)
                continue;

            FindEmptySpace(out var x, out var y);
            IconScript.PlaceNewGameobject(x, y, itm);
        }
        //make arrows. All the objects should be placed
        foreach (var aitm in items.Where(aitm => aitm.m_isItOpen > 0))
        {
            aitm.OpenItem();
            var itm = aitm as GameAbstractItem;

            if (itm?.m_dependencyCount == null) continue;
            foreach (var depList in itm.m_dependencyCount)
            foreach (var dep in depList.m_dependency.Where(
                dep => dep.m_isItOpen > 0))
                ArrowScript.NewStoredArrowScript(dep.m_thisObject, itm.m_thisObject);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

    }
}
