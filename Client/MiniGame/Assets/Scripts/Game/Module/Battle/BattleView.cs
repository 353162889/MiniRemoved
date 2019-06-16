using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Game
{
    public class BattleView : BaseSubView
    {
        private Text m_cTxtScore;
        private Button m_cBtnExit;

        public BattleView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            m_cTxtScore = this.MainGO.FindChildComponentRecursive<Text>("txtScore");
            m_cBtnExit = this.MainGO.FindChildComponentRecursive<Button>("btnExit");
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            UIEventTrigger.Get(m_cBtnExit.gameObject).AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, OnClickExit);
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
            m_cTxtScore.text = "0";
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void OnClickExit(BaseEventData arg0)
        {
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.LobbyState);
        }
    }

}