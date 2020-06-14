using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for for operating settings.
/// </summary>
public class SettingsMenu : SubMenu
{
    /// <summary> link to settings object. For saving/loading </summary>
    public Settings m_settings;
    /// <summary> slider for setting how many days should be in productivity queue </summary>
    public Slider m_sliderProductivity;
    /// <summary> input field for setting how many days should be in productivity queue </summary>
    public InputField m_inputProductivity;
    /// <summary> check box for showing full productivity tree or not </summary>
    public Toggle m_fullTree;
    /// <summary> set one language from all supported </summary>
    public Dropdown m_languageSelector;
    /// <summary> applying changes </summary>
    public Button m_apply;
    /// <summary> set default settings </summary>
    public Button m_setDefaults;
    /// <summary> discard changes </summary>
    public Button m_cancel;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// show current menu
    /// </summary>
    public override void ShowMenu(GameObject previous = null)
    {
        base.ShowMenu(previous);
        m_settings = Settings.GetSettings();
        SetMenuItems();
    }

    /// <summary>
    /// change all text data when a language has changed
    /// </summary>
    public void OnLanguageChanged()
    {
        int pos = m_languageSelector.value;
        Debug.Log(pos);
    }

    /// <summary>
    /// show correct information in all menu items
    /// </summary>
    void SetMenuItems()
    {
        Localization loc = Localization.GetLocalization();

        m_sliderProductivity.value = m_settings.m_productQueueLimit;
        m_fullTree.isOn = m_settings.m_isItFullTree;
        m_languageSelector.ClearOptions(); 
        m_languageSelector.AddOptions(loc.m_languages.m_languages);

        m_languageSelector.value = loc.m_currentLanguage;

        m_apply.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_applyButton;
        m_cancel.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_cancelButton;
        m_setDefaults.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_restoreButton;
        m_fullTree.transform.Find("Text").GetComponent<Text>().text = loc.m_ui.m_fullTreeText;
        m_sliderProductivity.transform.Find("Text").GetComponent<Text>().text =
            loc.m_ui.m_productivityLengthText;
        m_languageSelector.transform.Find("Text").GetComponent<Text>().text =
            loc.m_ui.m_changeLanguageText;
    }

    /// <summary>
    /// restoring defaults. Button's event
    /// </summary>
    public void RestoreDefaults()
    {
        m_settings.RestoreDefaults();
        SetMenuItems();
    }

    /// <summary>
    /// Event of moving productivity scale
    /// </summary>
    public void OnProductSliderChanged()
    {
        m_inputProductivity.text = ((int)m_sliderProductivity.value).ToString();
    }

    /// <summary>
    /// Ok button was pressed. Event
    /// </summary>
    public void OnOkPressed()
    {
        Resume();
        m_settings.m_productQueueLimit = (int)m_sliderProductivity.value;
        m_settings.m_isItFullTree = m_fullTree.isOn;
        Localization.GetLocalization().ChangeLanguage(m_languageSelector.value);
        m_settings.ApplySettings();
    }

    /// <summary>
    /// input field for productivity was changed
    /// </summary>
    public void OnProductTextChanged()
    {
        int value = int.Parse(m_inputProductivity.text);
        if (value > m_sliderProductivity.maxValue)
        {
            value = (int)m_sliderProductivity.maxValue;
            m_inputProductivity.text = value.ToString();
        }
        m_sliderProductivity.value = value;

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
