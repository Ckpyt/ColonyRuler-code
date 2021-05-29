using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for hide/unhide information panels.
/// Works for the color legend panel, the storage panel(top-right) and the population panel(top-left)
/// </summary>
public class HidingPanel : MonoBehaviour
{

    /// <summary> panel to hide / unhide </summary>
    public GameObject m_panel;
    public Vector2 m_deltaPosition;
    public bool m_isItEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCliick()
    {
        m_isItEnabled = !m_isItEnabled;
        m_panel.SetActive(m_isItEnabled);

        int delta = 1 * Convert.ToInt32(m_isItEnabled);
        int deltaPos = -1; //deltaPos can be -1 or 1
        deltaPos += delta + delta;
        float y = m_deltaPosition.y * deltaPos;
        Vector3 pos = new Vector3(m_deltaPosition.x, y, 0);
        transform.position = transform.position + pos;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 180);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
