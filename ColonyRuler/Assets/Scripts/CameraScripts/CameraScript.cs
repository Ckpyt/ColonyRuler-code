using System;
using UnityEngine;

/// <summary>
/// Class for moving and scaling a camera
/// </summary>
public class CameraScript : MonoBehaviour
{
    /// <summary> if the game is paused, camera should not move</summary>
    public static bool m_isItPaused = false;
    /// <summary> If a key pressed on the screen, the camera should be moved</summary>
    bool _isKeyPressed = false;
    /// <summary> for android only - two touches can scale the camera</summary>
    public static bool m_isTwoKeyPressed = false;
    /// <summary> link to main camera. </summary>
    public static Camera m_main;
    /// <summary> mouse position, then a button is pressed down</summary>
    Vector3 _mousePos;
    /// <summary> how long camera was moved, during this click</summary>
    Vector3 _mouseShift;
    /// <summary> maximum scaling rate. It should be different for Android and PC </summary>
    const float CLimitMax =
#if UNITY_ANDROID
        30;
#else
        20;
#endif


    // Start is called before the first frame update
    void Start()
    {
        m_main = Camera.main;
    }

    /// <summary>
    /// For scaling the main camera.
    /// it stricted between c_limit_max and 2
    /// </summary>
    /// <param name="value"></param>
    void ScaleCamera(float value)
    {
        if (!LearningTip.m_sIsSomethingShown)
        {
            m_main.orthographicSize -= value;
            if (m_main.orthographicSize < 2.0f)
                m_main.orthographicSize = 2.0f;
            if (m_main.orthographicSize > CLimitMax)
                m_main.orthographicSize = CLimitMax;
        }
    }

    void MoveCamera()
    {
        if (Input.anyKey && !MainScript.m_sIsButtonPressed && !LearningTip.m_sIsSomethingShown)
        {
            if (!_isKeyPressed)
            {
                _isKeyPressed = true;
                _mousePos = (Input.mousePosition);

            }
            else
            {

                float z = m_main.transform.position.z;
#if UNITY_ANDROID
                        if (Input.touches.Length > 1 || m_IsTwoKeyPressed)
                        {
                            ScaleCamera(-(Input.touches[0].deltaPosition.y / 20));
                            m_IsTwoKeyPressed = true;
                        }
                        else
                        {
#endif
                Vector3 newPos = ((Vector2)(Input.mousePosition));

                _mouseShift = (_mousePos - newPos);
                if (_mouseShift.magnitude > 0)
                {
                    _mouseShift = m_main.ScreenToWorldPoint(_mousePos) - m_main.ScreenToWorldPoint(newPos);
                    _mouseShift.x += m_main.transform.position.x;
                    _mouseShift.y += m_main.transform.position.y;
                    _mouseShift.z = z;
                    m_main.transform.position = _mouseShift;
                    _mousePos = newPos;
                }
#if UNITY_ANDROID
                        }
#else

#endif
            }
        }
        else
        {
            _isKeyPressed = false;
            m_isTwoKeyPressed = false;
        }
    }

    /// <summary>
    /// 
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                MoveCamera();

                if (Input.mouseScrollDelta.y != 0)
                {
                    ScaleCamera(Input.mouseScrollDelta.y);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("CameraScript Update Exception" + ex.Message);
        }
    }
}
