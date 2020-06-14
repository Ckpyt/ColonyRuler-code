using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources that cannot be produced by the player.
/// </summary>
public class MineralResource : AbstractObject
{
    /// <summary> Maximum on the territory </summary>
    public float m_maxCount = 0;


    /// <summary>
    /// Change all text data into current language.
    /// </summary>
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
        Localization loc = Localization.GetLocalization();
        m_tooltipCount = loc.m_ui.m_resourceCount;
        m_tooltipProductivity = loc.m_ui.m_abstractDamaged;
    }

    /// <summary>
    /// parsing excel data into current format
    /// </summary>
    /// <param name="itm">target</param>
    /// <param name="repItm">source</param>
    /// <returns> parsed item </returns>
    public static new AbstractObject Parse(AbstractObject itm, ExcelLoading.AbstractObject repItm)
    {
        ExcelLoading.MineralResource repRes = repItm as ExcelLoading.MineralResource;
        MineralResource res = itm as MineralResource;
        res.m_maxCount = decimal.ToInt32(repRes.maximum_in_territory);
        res.m_count = res.m_maxCount;

        return AbstractObject.Parse(itm, repItm);
    }

    /// <summary>
    /// Load data from file and parse it
    /// </summary>
    /// <param name="filename"> file name with full path </param>
    public static void Load(string filename)
    {

        var itms = Load<ExcelLoading.mineralResource>(filename);

        foreach (var rep in itms.repetative)
        {
            if (rep.name == "territory")
                Parse(new Territory(), rep);
            else
                Parse(new MineralResource(), rep);
        }
    }

    /// <summary>
    /// Copy from loaded item.
    /// shouldn't copy non-serialized fields
    /// </summary>
    /// <param name="source"> source for copying </param>
    public override void Copy(AbstractObject source)
    {
        base.Copy(source);
        MineralResource rsc = source as MineralResource;
        m_count = rsc.m_count;
    }

    /// <summary>
    /// A resource cannot be managed by the player manually, buttons should be disabled
    /// </summary>
    /// <param name="render"> render </param>
    /// <param name="isc"> target IconScript </param>
    public override void ChangeProductionType(SpriteRenderer render, IconScript isc)
    {
        base.ChangeProductionType(render, isc);
        DisableAllButtons(render);
        isc.ChangeLanguage();
    }

    /// <summary>
    /// Does not needed for mineral resources.
    /// </summary>
    /// <param name="worked"></param>
    public override void Working(long worked = 0)
    {
        
    }

}
