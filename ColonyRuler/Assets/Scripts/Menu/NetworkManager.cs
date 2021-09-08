using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Working with site api
/// Used only on Unity WebGL version
/// While an answer does not received, the user can see "networking" text only and can do nothing.
/// After the answer received, the user should return to the previous screen.
/// </summary>
public class NetworkManager : SubMenu
{
    class Request
    {
        public UnityWebRequest m_request;
        public ReceiveAnswer m_answer;
    }

    private Queue<Request> m_request = new Queue<Request>();
    /// <summary> server answer </summary>
    static string _sAnswer;
    /// <summary> current request </summary>
    static UnityWebRequest _sCurrent;

    /// <summary> global name of my site for WinApi</summary>
    const string CSiteName = "https://ckpyt.com/api/colonyrulerapi?";

    /// <summary> time of sending current request </summary>
    float _requestTime;
    /// <summary> 
    /// Receive answer Event.
    /// Unity WebGL has a single-thread model. 
    /// The game thread cannot wait for an answer, it should be received by 
    /// Unity network subsystem called once per update cycle
    /// </summary>
    public delegate void ReceiveAnswer(string answer);
    public ReceiveAnswer m_onReceiveAnswer;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Send a save request to server
    /// </summary>
    /// <param name="name"> username </param>
    /// <param name="saveName"> name of save </param>
    /// <param name="value"> save json </param>
    public static void SetText(string name, string saveName, string value)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("save", Encoding.UTF8.GetBytes(value));
        UnityWebRequest www = UnityWebRequest.Post(CSiteName + "save=" + saveName + "&name=" + name, form);
        www.SendWebRequest();
    }

    /// <summary>
    /// Save settings
    /// </summary>
    /// <param name="name">user name</param>
    /// <param name="requestId"> should be 1 or 2 </param>
    /// <param name="value"> settings json </param>
    public static void SetText(string name, int requestId, string value)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("settings", Encoding.UTF8.GetBytes(value));
        UnityWebRequest www = UnityWebRequest.Post(CSiteName + "id=" + requestId.ToString() + "&name=" + name, form);
        www.SendWebRequest();
    }

    /// <summary>
    /// receive text answer from server request
    /// Answer will be saved to s_answer
    /// </summary>
    /// <param name="request"> request to server. Should be prepared </param>
    /// <param name="answ">callback for answer</param>
    private IEnumerator GetText(UnityWebRequest request, ReceiveAnswer answ)
    {

        _requestTime = Time.time;
        _sCurrent = request;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            _sAnswer = null;
            Debug.Log(request.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(request.downloadHandler.text);

            // and retrieve results as binary data
            _sAnswer = request.downloadHandler.text;
        }

    }
    /// <summary>
    /// making a Get request for sending into server
    /// </summary>
    /// <param name="request"> request for sending </param>
    /// <param name="answ">callback for answer</param>
    void GetRequest(UnityWebRequest request, ReceiveAnswer answ)
    {
        if (m_onReceiveAnswer == null || m_onReceiveAnswer.Target == null)
        {
            ShowMenu();
            m_onReceiveAnswer += answ;
            StartCoroutine(GetText(request, answ));
        }
        else
        {
            m_request.Enqueue(new Request { m_request = request, m_answer = answ });
        }
    }

    /// <summary>
    /// Get one localization item
    /// </summary>
    /// <param name="loc"> localization name </param>
    /// <param name="id"> request id. 1 - UI, 2 - items, 3 - history </param>
    /// <param name = "answ" > callback for answer</param>
    public void GetLocalization(string loc, int id, ReceiveAnswer answ)
    {
        UnityWebRequest www = UnityWebRequest.Get(CSiteName + "id=" + (id + 3) + "&name=" + loc);
        GetRequest(www, answ);
    }

    ///<param name = "answ" > callback for answer</param>
    public void GetLocalizationLanguages(ReceiveAnswer answ)
    {
        UnityWebRequest www = UnityWebRequest.Get(CSiteName + "id=" + 7 + "&name=" + "loc");
        GetRequest(www, answ);
    }

    /// <summary>
    /// Get save from server
    /// </summary>
    /// <param name="name"> user name  </param>
    /// <param name="saveName"> save name </param>
    /// <param name="answ">callback for answer</param> 
    public void GetText(string name, string saveName, ReceiveAnswer answ)
    {
        UnityWebRequest www = UnityWebRequest.Get(CSiteName + "save=" + saveName + "&name=" + name);
        GetRequest(www, answ);
    }

    /// <summary>
    /// Get text from server. Could be settings or save names
    /// </summary>
    /// <param name="name"> user name </param>
    /// <param name="requestId"> could 1 for settings or 2 for save name </param>
    /// <param name="answ">callback for answer</param>
    public void GetText(string name, int requestId, ReceiveAnswer answ)
    {
        UnityWebRequest www = UnityWebRequest.Get(CSiteName + "id=" + requestId.ToString() + "&name=" + name);
        GetRequest(www, answ);
    }

    public static string ByteToJson(string arr)
    {
        string tmp = arr.Replace("\\t", "");
        tmp = tmp.Replace("\\r", "");
        tmp = tmp.Replace("\\n", "");
        string conv = tmp.Replace("\\", "");

        if (conv.Length > 3)
        {
            var start = conv.IndexOf('{');
            var end = conv.IndexOf('}', conv.Length - 3);
            if (start != -1 && end != -1)
                conv = conv.Substring(start, end);
        }
        Debug.Log(conv);
        return conv;
    }

    /// <summary>
    /// waiting for recive an answer and closing menu
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        try
        {
            base.Update();
            if (_sCurrent != null && _sCurrent.isDone && _sAnswer != null)
            {
                //The previous menu should be activated first!
                Resume();
                m_onReceiveAnswer(_sAnswer);
                m_onReceiveAnswer = null;
                _sCurrent = null;
                _sAnswer = null;
                if (m_request.Count > 0)
                {
                    Request req = m_request.Dequeue();
                    GetRequest(req.m_request, req.m_answer);
                }
            }

            if (Time.time - _requestTime > 2.0f)
            {
                if (m_request.Count > 0)
                {
                    Request req = m_request.Dequeue();
                    GetRequest(req.m_request, req.m_answer);
                }
                m_onReceiveAnswer = null;
                _sCurrent = null;
                _sAnswer = null;
                Resume();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("NetworkManager Update Exception:" + ex.Message);
        }
    }
}
