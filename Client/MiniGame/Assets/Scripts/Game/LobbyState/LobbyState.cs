using UnityEngine;
using System.Collections;
using Framework;
namespace Game
{
    public class LobbyState : StateBase
    {
        protected override void OnEnter()
        {
            CLog.Log("LobbyStateEnter");
            ViewSys.Instance.Open("LobbyView");
        }

        protected override void OnExit()
        {
            CLog.Log("LobbyStateExit");
            ViewSys.Instance.Close("LobbyView");
        }
    }
}
