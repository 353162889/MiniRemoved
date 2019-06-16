using UnityEngine;
using System.Collections;

public class PrefabComponent : DataComponent
{
    public GameObject go;
    public Transform trans;
    public IBattleView view;

    public override void OnDestroy()
    {
        go = null;
        trans = null;
        if (null != view)
        {
            view.Dispose();
            view = null;
        }
    }
}
