using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Game
{
    public class LobbyView : BaseSubView
    {
        private Button m_cBtnStart;
        private Button m_cBtnExit;
        public LobbyView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            base.BuidUI();
            m_cBtnStart = this.MainGO.FindChildComponentRecursive<Button>("btnStart");
            m_cBtnExit = this.MainGO.FindChildComponentRecursive<Button>("btnExit");
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            UIEventTrigger.Get(m_cBtnStart.gameObject).AddListener(EventTriggerType.PointerClick,OnClickStart);
            UIEventTrigger.Get(m_cBtnExit.gameObject).AddListener(EventTriggerType.PointerClick, OnClickExit);

        }

        private void OnClickExit(BaseEventData arg0)
        {
            GameStarter.Exit();
        }

        private void OnClickStart(BaseEventData arg0)
        {
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.BattleState);
        }
    }
}
