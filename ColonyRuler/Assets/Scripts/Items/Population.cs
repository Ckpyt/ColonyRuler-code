using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class for counsuming happy items/food
/// controller for population.
/// View is IconScript
/// </summary>
[Serializable]
class Population : GameMaterial
{
    /// <summary> link to population model </summary>
    [NonSerialized]
    public People m_people;
    /// <summary> how happy our population </summary>
    [NonSerialized]
    public float m_happy;
    /// <summary> maximum happiness for population </summary>
    [NonSerialized]
    public float m_maxHappy;

    /// <summary> how many people straved and how long it happens </summary>
    public Queue<float> m_starved = new Queue<float>();
    /// <summary> how many sleeping places here </summary>
    [NonSerialized]
    public List<ItemsEffect> m_livingPlaces = new List<ItemsEffect>();

    /// <summary>
    /// This class not genered from Excel Data, 
    /// it should make all data manually
    /// </summary>
    public Population()
    {
        m_name = "People";
        m_text = m_name;
        m_isItIterable = false;
        Localization.m_onLanguageChanged += ChangeLanguage;
        m_defaultX = -0.66f;
        m_defaultY = -1.0f;
    }

    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
        Localization loc = Localization.GetLocalization();
        m_tooltipProductivity = loc.m_ui.m_abstractDamaged;
        m_tooltipCount = loc.m_ui.m_populationTooltip;
    }

    /// <summary>
    /// The population cannot be managed by the player manually, buttons should be disabled
    /// </summary>
    /// <param name="render"> render </param>
    /// <param name="isc"> target IconScript </param>
    public override void ChangeProductionType(SpriteRenderer render, IconScript isc)
    {
        base.ChangeProductionType(render, isc);
        DisableAllButtons(render);
    }

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject itm)
    {
        base.Copy(itm);
        Population pop = (Population)itm;
        m_starved = pop.m_starved;
    }

    /// <summary>
    /// Ordering food for eating
    /// </summary>
    public override float CountMaxProduction()
    {
        if (m_dependencyCount == null || m_dependencyCount[0] == null)
            return 0;

        int foodOpened = 0;
        foreach (var food in m_dependencyCount[0].m_dependency)
            foodOpened += food.m_isItOpen > 0 ? 1 : 0;

        foreach (var food in m_dependencyCount[0].m_dependency)
            if (food.m_isItOpen > 0)
                food.m_oredered += m_workers / foodOpened;

        return 0;
    }

    /// <summary>
    /// Here: eating food and using happy items
    /// </summary>
    /// <param name="worked"> should be 0 </param>
    public override void Working(long worked)
    {
        if (m_dependencyCount == null || m_dependencyCount.Length == 0) return;
        m_happy = 0;
        m_workers = m_people.PeopleNumber;
        m_count = m_workers;

        #region calc food
        float foodNeeded = m_workers;
        float dishesCount = 0;
        foreach (var food in m_dependencyCount[0].m_dependency)
            if (food.m_isItOpen > 0)
                dishesCount++;

        float countFood = (float)m_workers / dishesCount;
        float numberOfFoodItems = dishesCount * m_workers;
        float starv = 0;

        foreach (var food in m_dependencyCount[0].m_dependency)
        {
            if (food.m_isItOpen > 0)
                if (food.m_count > countFood)
                {
                    food.m_consumed += countFood;
                    food.m_count -= countFood;
                    numberOfFoodItems -= m_workers;
                    foodNeeded -= countFood;
                }
                else
                {
                    numberOfFoodItems -= (m_workers -
                        (countFood - food.m_count) * m_dependencyCount[0].m_dependency.Count);
                    foodNeeded -= food.m_count;
                    food.m_consumed += food.m_count;
                    food.m_count = 0;
                }

        }

        if (foodNeeded > 0)
            foreach (var food in m_dependencyCount[0].m_dependency)
            {
                if (food.m_isItOpen > 0)
                    if (food.m_count > foodNeeded)
                    {
                        food.m_count -= foodNeeded;
                        food.m_consumed += foodNeeded;
                        foodNeeded = 0;
                    }

                    else
                    {
                        food.m_consumed += food.Count;
                        foodNeeded -= food.m_count;
                        food.m_count = 0;
                    }
            }

        starv = (int)foodNeeded;
        starv = starv > 0 ? starv : -m_people.PeopleNumber;
        #endregion

        //calc unhappiness of low range food
        m_maxHappy = 0;
        Storage st = Camera.main.GetComponent<Storage>();
        foreach (var itm in st.m_livingStorages)
        {
            Buildings bld = itm as Buildings;
            m_maxHappy += bld.m_happyEffect.m_value * bld.m_peopleLive;
            bld.m_living.ToolUsed((long)bld.m_living.m_toolsCount);
        }
        m_happy -= numberOfFoodItems;

        // calc happy of happy items
        foreach (var happy in m_tools)
        {
            if (happy.m_toolLink.m_isItOpen > 0 && happy.m_toolLink.GetType() == typeof(Items)
                && m_happy < m_maxHappy)
            {
                long needTools = m_workers - happy.m_toolsCount;
                long maxTools = (long)((m_maxHappy - m_happy) / happy.m_value);
                needTools = needTools > maxTools - happy.m_toolsCount ? maxTools - happy.m_toolsCount : needTools;
                needTools = (long)(needTools > happy.m_toolLink.m_count ?
                    happy.m_toolLink.m_count : needTools);
                happy.m_toolsCount += needTools;
                if (happy.m_toolsCount > maxTools)
                {
                    needTools = maxTools - happy.m_toolsCount;
                    happy.m_toolsCount = maxTools;
                }
                happy.m_toolLink.Count -= needTools;

                long wrks = (happy.m_toolsCount > m_workers ? m_workers : happy.m_toolsCount);
                m_happy += wrks * happy.m_value;
                happy.ToolUsed((long)wrks);
            }
        }

        m_people.m_happy = m_people.m_isItBoost ? (int)m_maxHappy : (int)m_happy;
        m_people.m_maxHappy = (int)m_maxHappy;


        //calc straving
        m_people.m_isSomeoneHungry = starv > 0;

        while (m_starved.Count < 31)
            m_starved.Enqueue((int)starv);
        m_starved.Dequeue();

        float oldStarv = m_starved.Sum() / m_starved.Count;

        if (oldStarv >= 1 && starv > 0)
        {
            int died = (int)oldStarv;
            m_people.PeopleNumber -= died;
            if (m_people.PeopleNumber < 0)
                m_people.PeopleNumber = 0;
            int i = 0;
            while (i < m_starved.Count)
            {
                i++;
                float strv = m_starved.Dequeue();
                strv = strv > m_people.PeopleNumber ? m_people.PeopleNumber : strv;
                m_starved.Enqueue(strv);
            }
        }
    }

    /// <summary>
    /// Open item at startup.
    /// Here: getting buildings for living and happy tools for furver using
    /// Could be called only in Start/Load game
    /// </summary>
    public override void OpenItem()
    {
        for (int i = 0; i < 30; i++)
            m_starved.Enqueue(-m_people.PeopleNumber);

        m_isItIterable = false;
        m_tools = new List<ItemsEffect>();

        m_dependencyCount = new DependencyCount[1];
        m_dependencyCount[0] = new DependencyCount();
        m_dependencyCount[0].m_dependency = new List<AbstractObject>();
        foreach (var food in Productions.GetProductions("food"))
            m_dependencyCount[0].m_dependency.Add(food as GameMaterial);

        foreach (var building in Productions.GetTools("living"))
            m_livingPlaces.Add(new ItemsEffect(building));

        m_livingPlaces.Sort((ItemsEffect pair1, ItemsEffect pair2) =>
        {
            Buildings bld1 = pair1.m_toolLink as Buildings;
            Buildings bld2 = pair2.m_toolLink as Buildings;
            return (int)(bld1.m_happyEffect.m_value - bld2.m_happyEffect.m_value);
        });
        Productions.AddProduction(this, "happy");

        m_people = Camera.main.GetComponent<People>();
        while (m_people.GetWorker())
            ((GameMaterial)(m_dependencyCount[0].m_dependency[0])).m_workers++;
    }

    /// <summary>
    /// Cannot be upgraded
    /// </summary>
    public override bool CheckUpgradeConditions()
    {
        return false;
    }
}

