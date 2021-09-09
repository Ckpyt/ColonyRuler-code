using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Model Class of population.
/// holds free workers, incising population and calculate max happy
/// </summary>
[Serializable]
public class People : MonoBehaviour
{
    /// <summary> then the game is paused, it should not works</summary>
    public static bool m_isItPaused = false;
    /// <summary> gameObject for showing emoji </summary>
    public Image m_publicEmoji = null;
    /// <summary> gameObject for showing happy boost </summary>
    public GameObject m_emojiBoost = null;
    /// <summary> link to happy boost prefab </summary>
    public GameObject m_emojiBoostPrefab = null;
    /// <summary> link to gameObject for showing when people starving </summary>
    public Image m_hungry = null;

    /// <summary> all emotional sprites. from sad to happy </summary>
    Sprite[] _emoji = new Sprite[5];
    /// <summary> Text for showing how many people in the colony</summary>
    public Text m_peopleText;
    /// <summary> Text for showing how many free workers in the colony </summary>
    public Text m_workersText;
    /// <summary> how many people in the colony </summary>
    long _people = 10;
    /// <summary> how many free workers in the colony </summary>
    long _workers = 10;
    /// <summary> maximum happiness in the colony. Could be increased by making new buildings </summary>
    public int m_maxHappy = 0;
    /// <summary> current happy in the colony. Could be increased by making happy items </summary>
    public int m_happy = 0;
    /// <summary> Link to timeScript </summary>
    public TimeScript m_timeScr;
    /// <summary> Link to Storage </summary>
    public Storage m_storage;
    /// <summary> Last year of last update. Should be replaced into event system </summary>
    public int m_lastYear = 0;
    /// <summary> Day of starting boost. 0 if it is not started </summary>
    int _startBoostDay = 0;
    /// <summary> Is boost started? </summary>
    public bool m_isItBoost = false;
    /// <summary> Is someone starving? Needed for shoving hungry icon </summary>
    public bool m_isSomeoneHungry = false;
    /// <summary> how long boost should works </summary>
    int _boostDuration = 30;

    /// <summary> public access to m_people </summary>
    public long PeopleNumber
    {
        get
        {
            return _people;
        }
        set
        {
            _people = value;
            if (_people < 0)
                _people = 0;
        }
    }

    /// <summary> public access to m_workers </summary>
    public long WorkersNumber
    {
        get
        {
            return _workers;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void MakeBoost()
    {
        m_emojiBoost = Instantiate(m_emojiBoostPrefab);
        m_emojiBoost.transform.SetParent(m_publicEmoji.transform.parent);
        m_emojiBoost.transform.position = m_publicEmoji.transform.position;
        var pos = m_publicEmoji.transform.position;
        pos.x += m_publicEmoji.rectTransform.rect.width;
        m_emojiBoost.transform.position = pos;
        m_isItBoost = true;
        _startBoostDay = m_timeScr.TotalDays;
    }

    /// <summary>
    /// Get free worker.
    /// </summary>
    /// <returns> true, if it was received.</returns>
    public bool GetWorker()
    {
        if (_workers > 0)
        {
            _workers--;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// return free worker
    /// </summary>
    public void ReturnWorker()
    {
        _workers++;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        try
        {
            PrefabStorage str = GetComponent<PrefabStorage>();
            _emoji[0] = str.m_verySadEmoji;
            _emoji[1] = str.m_sadEmoji;
            _emoji[2] = str.m_neuralEmoji;
            _emoji[3] = str.m_moodEmoju;
            _emoji[4] = str.m_veryMoodEmoji;
            m_emojiBoostPrefab = str.m_emojiBoostPrefab;
        }
        catch (Exception ex)
        {
            Debug.LogError("People Start Exception" + ex.Message);
        }
    }

    /// <summary>
    /// Check how many free workers in the colony
    /// </summary>
    public void CheckWorkersCount()
    {
        long wrks = 0;
        foreach (AbstractObject itm in AbstractObject.m_sEverything)
        {
            GameAbstractItem git = itm as GameAbstractItem;
            if (git != null && git.GetType() != typeof(Population) && git.m_workers > 0)
                wrks += (long)git.m_workers;
        }

        if (wrks + _workers != _people)
        {
            _workers += _people - (wrks + _workers);
            int i = AbstractObject.m_sEverything.Count - 1;
            while (_workers < 0 && i > -1)
            {
                GameAbstractItem itm = AbstractObject.m_sEverything[i] as GameAbstractItem;
                if (itm != null)
                {
                    if ((long)itm.m_workers + _workers > 0)
                    {
                        itm.m_workers += _workers;
                        _workers = 0;
                    }
                    else
                    {
                        _workers += (long)itm.m_workers;
                        itm.m_workers = 0;
                    }
                }
                i--;
            }
        }
    }

    /// <summary>
    /// Change emoji icon.
    /// 0-20 very sad, 20-40 sad, 40-60 normal, 60-80 happy, 80-100 very happy
    /// </summary>
    void ChangeEmoji()
    {
        if (m_maxHappy > 10)
        {
            int percent = (int)((m_happy) * 5 / m_maxHappy);
            m_publicEmoji.sprite = _emoji[percent < 0 ? 0 : percent > 4 ? 4 : percent];
        }
        else
        {
            m_publicEmoji.sprite = m_happy < 0 ? _emoji[1] : _emoji[2];
        }
    }

    /// <summary>
    /// check should be boost destroyed and how many time it has
    /// </summary>
    void CheckBoost()
    {
        TimeScript ts = m_timeScr;
        if (m_isItBoost)
        {
            if (m_isItBoost && ts.TotalDays > _startBoostDay + _boostDuration)
            {
                m_isItBoost = false;
                Destroy(m_emojiBoost);
            }
            else
            {
                Slider sldr = m_emojiBoost.transform.Find("Slider").
                    gameObject.GetComponent<Slider>();
                sldr.value = ((float)ts.TotalDays - (float)_startBoostDay) / (float)_boostDuration;
            }
        }
    }

    /// <summary>
    /// How many people can live in the colony.
    /// depends on happy and maxHappy
    /// </summary>
    /// <returns></returns>
    public long CalcMaxPopulation()
    {
        var livings = m_storage.m_livingStorages;
        float maxHappy = 0;
        foreach (var livEff in livings)
        {
            Buildings bld = livEff as Buildings;
            if (bld.m_isItOpen > 0 && bld.m_peopleLive > 0)
                maxHappy = maxHappy > bld.m_happyEffect.m_value ?
                    maxHappy : bld.m_happyEffect.m_value;
        }
        long result = (long)(Mathf.Sqrt(m_happy * maxHappy));
        result = result < 0 ? 0 : result;
        return 10 + result;
    }

    /// <summary>
    /// increase population, if happy > 75 and  maxPeople > m_people 
    /// Once per year
    /// </summary>
    void IncreasePopulation()
    {
        int year = m_timeScr.Year;
        if (year != m_lastYear)
        {
            m_lastYear = year;
            if (_people < 10 && _people > 0) //for rising if population less than 10)
            {
                _people++;
                return;
            }

            long maxPopul = CalcMaxPopulation();
            if (m_maxHappy > 0 && m_happy > 4 * m_maxHappy / 5)  
            {
                long newPeople = _people / 10;
                if (newPeople == 0) newPeople++; //for rising if population less than 10

                maxPopul = maxPopul < m_storage.GetValue(ContainerType.people) ?
                    maxPopul : m_storage.GetValue(ContainerType.people);
                if (_people + newPeople > maxPopul)
                {
                    newPeople = maxPopul - _people;
                    if (newPeople > 0)
                    {
                        _people += newPeople;
                    }
                }
                else
                {
                    _people += newPeople;
                }
            }
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        try
        {
            if (!TimeScript.m_isItPaused && !m_isItPaused)
            {
                IncreasePopulation();
                m_peopleText.text = _people.ToString();
                m_workersText.text = _workers.ToString();
                ChangeEmoji();
                CheckWorkersCount();
                CheckBoost();
                m_hungry.gameObject.SetActive(m_isSomeoneHungry);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("People Update Exception " + ex.Message);
        }
    }
}
