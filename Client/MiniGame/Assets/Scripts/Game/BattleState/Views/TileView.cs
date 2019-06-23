using UnityEngine;
using System.Collections;
using Framework;
using HighlightingSystem;

public class TileView : IBattleView
{
    private string m_sPath;
    private GameObject m_cGo;
    private Transform m_cParent;
    private bool m_bShowEffect;
    private Highlighter m_cHighlighter;

    public void Init(string path, Transform parent)
    {
        m_sPath = path;
        m_cParent = parent;
        m_bShowEffect = false;

        BattleResPool.Instance.GetObject(path, false, OnLoadResource);
    }

    public void ShowEffect(bool showEffect)
    {
        m_bShowEffect = showEffect;
        if (null != m_cHighlighter)
        {
            m_cHighlighter.tween = showEffect;
        }
    }

    private void OnLoadResource(string path, Object go)
    {
        m_cGo = (GameObject)go;
        m_cParent.gameObject.AddChildToParent(m_cGo);
        m_cHighlighter = m_cGo.GetComponent<Highlighter>();
        ShowEffect(m_bShowEffect);
    }

    public void Dispose()
    {
        if (!string.IsNullOrEmpty(m_sPath))
        {
            if(BattleResPool.Instance != null)
                BattleResPool.Instance.RemoveCallback(m_sPath, OnLoadResource);
            if (null != m_cGo)
            {
                if (BattleResPool.Instance != null)
                    BattleResPool.Instance.SaveObject(m_sPath, m_cGo);
            }
        }

        m_cParent = null;
        m_sPath = null;
        m_cHighlighter = null;
    }
}
