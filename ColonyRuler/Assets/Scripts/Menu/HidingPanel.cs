using System;
using UnityEngine;

/// <summary>
/// Class for hide/unhide information panels.
/// Works for the color legend panel, the storage panel(top-right) and the population panel(top-left)
/// </summary>
public class HidingPanel : MonoBehaviour
{

    /// <summary> panel to hide / unhide </summary>
    public GameObject m_panel;
    /// <summary> moving еру button from hidden to unhidden position of the panel  </summary>
    public Vector2 m_deltaPosition;
    public bool m_isItUnhiden = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClick()
    {
        m_isItUnhiden = !m_isItUnhiden;
        m_panel.SetActive(m_isItUnhiden);

        int delta = 1 * Convert.ToInt32(m_isItUnhiden);
        int deltaPos = -1; //deltaPos can be -1 or 1
        deltaPos += delta + delta;
        float y = m_deltaPosition.y * deltaPos;
        float x = m_deltaPosition.x * deltaPos;
        Vector3 pos = new Vector3(x, y, 0);
        transform.position = transform.position + pos;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 180);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
