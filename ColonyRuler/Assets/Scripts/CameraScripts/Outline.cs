using UnityEngine;
using UnityEngine.UI;

public class Outline : MonoBehaviour
{
    public Rect m_OutlineRect = new Rect();
    public bool mb_IsItCanvas = false;

    bool mb_IsItInitialized = false;
    const int СCanvasScale = 5;
    const float СPositionZ = -1.5f;

    public GameObject m_LeftLine;
    public GameObject m_RightLine;
    public GameObject m_TopLine;
    public GameObject m_BottomLine;


    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    void ChangePositionToChild(GameObject child, float scaleX, float scaleY, float posX, float posY)
    {
        child.transform.position = new Vector3(posX, posY, СPositionZ);
        child.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

    void CanvasImplematationToChild(GameObject child)
    {
        var sprite = child.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        var image = child.GetComponent<Image>();
        image.enabled = true;
    }

    void Initialization()
    {
        mb_IsItInitialized = true;
        m_OutlineRect.position += (Vector2)transform.position;
        float scale = m_TopLine.transform.localScale.y;
        if (mb_IsItCanvas)
        {
            scale = СCanvasScale;
            transform.parent = Camera.main.GetComponent<MainScript>().m_mainCanvas.transform;
            CanvasImplematationToChild(m_TopLine);
            CanvasImplematationToChild(m_BottomLine);
            CanvasImplematationToChild(m_LeftLine);
            CanvasImplematationToChild(m_RightLine);
        }

        ChangePositionToChild(m_TopLine, m_OutlineRect.width + scale, scale, m_OutlineRect.center.x, m_OutlineRect.yMax);
        ChangePositionToChild(m_BottomLine, m_OutlineRect.width + scale, scale, m_OutlineRect.center.x, m_OutlineRect.yMin);
        ChangePositionToChild(m_LeftLine, scale, m_OutlineRect.height + scale, m_OutlineRect.xMin, m_OutlineRect.center.y);
        ChangePositionToChild(m_RightLine, scale, m_OutlineRect.height + scale, m_OutlineRect.xMax, m_OutlineRect.center.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_OutlineRect.size.x > 0)
        {
            if (mb_IsItInitialized == false)
            {
                Initialization();
            }
        }

    }
}
