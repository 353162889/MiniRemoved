using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Game
{
    public class ResultView : BaseSubView
    {
        private Text m_cTxtScore;
        private Button m_cBtnReturn;
        private Button m_cBtnAgain;
        public ResultView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            m_cTxtScore = this.MainGO.FindChildComponentRecursive<Text>("txtScore");
            m_cBtnReturn = this.MainGO.FindChildComponentRecursive<Button>("btnReturn");
            m_cBtnAgain = this.MainGO.FindChildComponentRecursive<Button>("btnAgain");
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            UIEventTrigger.Get(m_cBtnReturn.gameObject).AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, OnClickReturn);
            UIEventTrigger.Get(m_cBtnReturn.gameObject).AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, OnClickAgain);
        }

        private void OnClickAgain(BaseEventData arg0)
        {

        }

        private void OnClickReturn(BaseEventData arg0)
        {
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.LobbyState);
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
            m_cTxtScore.text = "0";
        }
    }
}