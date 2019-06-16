using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class PathTool
{
    public static string GetBasePrefabPath(string name)
    {
        if (!name.EndsWith(".prefab"))
        {
            return name + ".prefab";
        }
        return name;
    }

    public static string BattleEffectDir = "Prefab/Effect/";
    public static string GetBattleEffectPath(string name)
    {
        return GetBasePrefabPath(BattleEffectDir + name);
    }
}
