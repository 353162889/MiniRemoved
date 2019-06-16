using UnityEngine;
using System.Collections;
using Framework;
using Game;

public class EffectView : IBattleView
{

    private string m_sName;
    private GameObject m_cGo;

    public void Init(string name,bool autoDestroy, Transform parent)
    {
        m_sName = name;
        m_cGo = BattleEffectPool.Instance.CreateEffect(name, autoDestroy, parent);
    }


    public void Dispose()
    {
        if (!string.IsNullOrEmpty(m_sName))
        {
            BattleEffectPool.Instance.DestroyEffectGO(m_cGo);
        }
        m_sName = null;
        m_cGo = null;
    }
}
