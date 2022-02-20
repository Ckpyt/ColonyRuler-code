
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endif

/// <summary>
/// Get username from website javascript
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{

    /*[DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);*/
#if UNITY_WEBGL && !UNITY_EDITOR
    /// <summary>  </summary>
    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    [DllImport("__Internal")]
    private static extern string SessionIDReturnValueFunction();
#else
    /// <summary>  </summary>
    private static string StringReturnValueFunction() { return "Ckpyt"; }

    private static string SessionIDReturnValueFunction() { return "0"; }
#endif


    public static string GetUserName()
    {
        return StringReturnValueFunction();
    }

    public static int GetSessionID()
    {
        return int.Parse( SessionIDReturnValueFunction());
    }

    //[DllImport("__Internal")]
    //private static extern void BindWebGLTexture(int texture);

    void Start()
    {
        /*Hello();

        HelloString("This is a string.");

        float[] myArray = new float[10];
        PrintFloatArray(myArray, myArray.Length);

        int result = AddNumbers(5, 7);
        Debug.Log(result);*/

        //Debug.Log(StringReturnValueFunction());

        //var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        //BindWebGLTexture(texture.GetNativeTextureID());
    }

    void Update()
    {

    }
}