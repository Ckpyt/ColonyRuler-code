
/// <summary>
/// Current territory. 
/// Now only one object in the game, on the next stage it should be one in each zone
/// !!!Move functional here from Storage!!!
/// </summary>
public class Territory : Resource
{
    /// <summary> territory size at last day </summary>
    int _lastStorage = 0;
    /// <summary> count at last day </summary>
    float _lastCount = 0;
    bool _isItInitialized = false;

    /// <summary>
    /// Manipulating territory
    /// </summary>
    /// <param name="worked">not used</param>
    public override void Working(long worked = 0)
    {
        if(Storage.m_storage != null)
        {
            if (!_isItInitialized)
            {
                Storage.m_storage.m_territory = (int)m_count;
                _lastCount = m_count;
                _lastStorage = Storage.m_storage.m_territory;

                _isItInitialized = true;
            }
            else
            {
                int delTerr = Storage.m_storage.m_territory - _lastStorage;
                float delCount = m_count - _lastCount;
                int delt = 0;

                if (delCount > 1 || delCount < -1)
                {
                    delt = (int)delCount;
                    Storage.m_storage.m_territoryMax += delt;
                    Storage.m_storage.m_territory += delt;
                    m_count = Storage.m_storage.m_territory;
                    _lastCount = m_count;
                    m_count += (delCount - delt);
                    
                }
                else if(delTerr != 0)
                {
                    m_count += delTerr;
                    _lastCount += delTerr;
                }

                _lastStorage = Storage.m_storage.m_territory;
            }
        }
    }
}

