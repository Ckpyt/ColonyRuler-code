using UnityEngine;

/// <summary>
/// Submenu class.
/// </summary>
public class SubMenu : MonoBehaviour
{
    /// <summary> previous menu </summary>
    protected GameObject m_previous = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        MainMenu.m_sActiveMenu = this.gameObject;
    }

    /// <summary>
    /// Show this menu and close previous
    /// </summary>
    /// <param name="previous"></param>
    public virtual void ShowMenu(GameObject previous = null)
    {
        if (previous == null && MainMenu.m_sActiveMenu != null && MainMenu.m_sActiveMenu.activeSelf) 
            previous = MainMenu.m_sActiveMenu;
        m_previous = previous;
        previous?.SetActive(false);
        gameObject.SetActive(true);
        
    }

    /// <summary>
    /// Close this menu and show previous
    /// </summary>
    public virtual void Resume()
    {
        gameObject.SetActive(false);
        m_previous?.SetActive(true);
        MainMenu.m_sActiveMenu = m_previous;
        m_previous = null;
    }
}
