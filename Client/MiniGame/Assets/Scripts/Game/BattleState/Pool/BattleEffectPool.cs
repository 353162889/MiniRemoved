﻿using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{ 
    /// <summary>
    /// 场景特效池（切换场景时清理）
    /// </summary>
    public class BattleEffectPool : EffectPool<BattleEffectPool>
    {
        public void CacheObject(string name, int count, Action<string> callback)
        {
            string path = PathTool.GetBattleEffectPath(name);
            base._CacheObject(path, true, count, callback);
        }

        public void RemoveCacheObject(string name, Action<string> callback)
        {
            string path = PathTool.GetBattleEffectPath(name);
            base._RemoveCacheObject(path, callback);
        }

        public GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathTool.GetBattleEffectPath(name);
            return base._CreateEffect(path, autoDestory, parent);
        }
    }
}
