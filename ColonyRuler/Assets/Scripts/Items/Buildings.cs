using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class of buildings. Each building placed in the territory.
/// </summary>
[Serializable]
public class Buildings : Items
{
    /// <summary> How many items could be stored. Should be one more then it has </summary>
    public override float StorageSize
    {
        get
        {
            return (Count + m_damagedCount + 1) * m_size;
        }
    }
    /// <summary> If it could be used as a house, it is how many houses used for living </summary>
    [NonSerialized]
    public ItemsEffect m_living = null;
    /// <summary> If it could be used as a house, it is how many people is living here </summary>
    [NonSerialized]
    public float m_peopleLive = 0;

    /// <summary>
    /// How many buildings here
    /// </summary>
    public override float Count
    {
        get
        {
            return m_living == null ? m_count : m_count + m_living.m_toolsCount;
        }
        set
        {
            m_count = m_living == null ? value : value - m_living.m_toolsCount;
        }
    }

    /// <summary>
    /// Change production icon to building icon
    /// </summary>
    /// <param name="render"> render </param>
    /// <param name="isc"> target IconScript </param>
    public override void ChangeProductionType(SpriteRenderer render, IconScript isc)
    {
        Vector3 pos = render.transform.position;
        pos.z = -1;
        render.transform.position = pos;
        Texture2D obj = Resources.Load("Pictures/building") as Texture2D;
        render.sprite = Sprite.Create(obj, new Rect(0.0f, 0.0f, obj.width, obj.height), new Vector2(0.5f, 0.5f), 100.0f);
        isc.ChangeSprite("Pictures/Button_up_demolish", "Pictures/Button_up_demolish_gray");
    }

    /// <summary>
    /// Parse building from excel data to current format
    /// </summary>
    /// <param name="abs"> target </param>
    /// <param name="repItm"> source </param>
    /// <returns> parsed item </returns>
    public new static AbstractObject Parse(AbstractObject abs, ExcelLoading.AbstractObject repItm)
    {
        Items itm = abs as Items;
        Items.Parse(itm, repItm);
        Storage st = Camera.main.GetComponent<Storage>();
        st.AddStorageItem(itm as Items);
        Buildings bld = itm as Buildings;
        try
        {
            bld.m_living = bld.m_effects["living"];
        }
        catch (KeyNotFoundException kex)
        {
            Debug.Log("Building " + bld.m_name + " does not contain living effects" + kex.Message);
        }
        return itm;
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public new static void Load(string filename)
    {
        var itms = Load<ExcelLoading.buildings>(filename);

        foreach (var rep in itms.repetative)
        {
            Parse(new Buildings(), rep);
        }
    }

    /// <summary>
    /// Could be current item destroyed?
    /// </summary>
    public override bool CheckUpgradeConditions()
    {
        return m_count >= 0.90f;
    }

    /// <summary>
    /// Event of complite working in GameMaterial
    /// </summary>
    /// <param name="productsFinished"> how many items done </param>
    /// <param name="mulEffect"> not used </param>
    public override void WorkComplite(float productsFinished, float mulEffect)
    {
        float maxProducts = (float)Storage.m_storage.GetValue(ContainerType.territory) / (float)m_size;
        if (productsFinished > maxProducts)
            productsFinished = maxProducts > 0 ? maxProducts : 0;
        base.WorkComplite(productsFinished, 1);
    }


    /// <summary>
    /// How many buildings here in string format
    /// </summary>
    public override string GetCountString()
    {
        return m_count.ToString("F") + (m_living == null || m_living.m_toolsCount  == 0 
            ? "" : "/" + m_living.m_toolsCount.ToString());
    }

    /// <summary>
    /// Destroy building
    /// </summary>
    public override bool Upgrade()
    {
        if (m_count > 1)
            m_count--;
        else
            m_count = 0;
        return true;
    }
}

