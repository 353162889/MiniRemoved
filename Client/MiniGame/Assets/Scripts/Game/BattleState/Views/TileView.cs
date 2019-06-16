using UnityEngine;
using System.Collections;
using Framework;

public class TileView : IBattleView
{
    private string m_sPath;
    private GameObject m_cGo;
    private Transform m_cParent;

    public void Init(string path, Transform parent)
    {
        m_sPath = path;
        m_cParent = parent;
        BattleResPool.Instance.GetObject(path, false, OnLoadResource);
    }

    private void OnLoadResource(string path, Object go)
    {
        m_cGo = (GameObject)go;
        m_cParent.gameObject.AddChildToParent(m_cGo);
    }

    public void Dispose()
    {
        if (!string.IsNullOrEmpty(m_sPath))
        {
            BattleResPool.Instance.RemoveCallback(m_sPath, OnLoadResource);
            if (null != m_cGo)
            {
                BattleResPool.Instance.SaveObject(m_sPath, m_cGo);
            }
        }

        m_cParent = null;
        m_sPath = null;
    }
}
