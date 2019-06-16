using UnityEngine;
using System.Collections;
using Framework;
using System;
using System.Collections.Generic;
using GameData;
using Unity.Entities;

namespace Game
{
    public class GameStarter : MonoBehaviour
    {
        public static StateContainerBase GameGlobalState { get; private set; }

        public static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();
#endif

        }

        private void Awake()
        {
            Application.runInBackground = true;
            Screen.orientation = ScreenOrientation.Portrait;
            GameObject.DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            InitSingleton();
            LoadCfg();
        }

        protected void LoadCfg()
        {
            ResCfgSys.Instance.LoadResCfgs("Config/Data", OnLoadCfg);
        }

        private void OnLoadCfg()
        {
            InitUI();
            InitState();
        }

        private void InitUI()
        {
            List<ResUI> lst = ResCfgSys.Instance.GetCfgLst<ResUI>();
            for (int i = 0; i < lst.Count; i++)
            {
                ViewSys.Instance.RegistUIPath(lst[i].name, PathTool.GetBasePrefabPath(lst[i].prefab));
            }
        }

        protected void InitState()
        {
            GameGlobalState = (StateContainerBase)CreateState(GameStateCfg.GameState);
            GameGlobalState._OnEnter();
        }

        protected StateBase CreateState(GameStateData stateData)
        {
            StateBase state = Activator.CreateInstance(stateData.mClassType) as StateBase;
            if (stateData.mSubStateData != null)
            {
                StateContainerBase stateContainer = (StateContainerBase)state;
                for (int i = 0; i < stateData.mSubStateData.Length; i++)
                {
                    var data = stateData.mSubStateData[i];
                    StateBase subState = CreateState(data);
                    stateContainer.AddState((int)data.mStateType, subState, data.mDefaultState);
                }
            }
            return state;
        }

        void InitSingleton()
        {
            gameObject.AddComponentOnce<ConsoleLogger>();
            gameObject.AddComponentOnce<ResourceSys>();
            bool directLoadMode = true;
#if !UNITY_EDITOR || BUNDLE_MODE
            directLoadMode = false;
#endif
            ResourceSys.Instance.Init(directLoadMode, "Assets/ResourceEx");
            gameObject.AddComponentOnce<UpdateScheduler>();
            gameObject.AddComponentOnce<TouchDispatcher>(); 
            //初始化对象池
            GameObject goPool = new GameObject();
            goPool.name = "GameObjectPool";
            GameObject.DontDestroyOnLoad(goPool);
            goPool.AddComponentOnce<ResourceObjectPool>();
            goPool.AddComponentOnce<PrefabPool>();
            goPool.AddComponentOnce<BattleResPool>();
            goPool.AddComponentOnce<BattleEffectPool>();

            GameObject uiGO = GameObject.Find("UIRoot/Views").gameObject;
            GameObject.DontDestroyOnLoad(uiGO);
            uiGO.AddComponentOnce<ViewSys>();

            gameObject.AddComponentOnce<FPSMono>();

            ResetObjectPool<List<int>>.Instance.Init(4, (List<int> lst) => { lst.Clear();});
            ResetObjectPool<HashSet<Vector2Int>>.Instance.Init(10, (HashSet<Vector2Int> lst) => {lst.Clear(); });
            ResetObjectPool<List<DataComponent>>.Instance.Init(10,(List<DataComponent> lst)=>{lst.Clear();});
            ResetObjectPool<HashSet<Entity>>.Instance.Init(10, (HashSet<Entity> lst) => { lst.Clear(); });
            ResetObjectPool<Dictionary<int, int>>.Instance.Init(5, (Dictionary<int, int> dic) => { dic.Clear(); });
        }

        private void Update()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnUpdate();
            }
        }


        private void LateUpdate()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnLateUpdate();
            }
        }

        private void OnDestroy()
        {
            if(null != GameGlobalState)
            {
                GameGlobalState._OnDispose();
                GameGlobalState = null;
            }
        }

        public void OnApplicationQuit()
        {
            if (null != GameGlobalState)
            {
                GameGlobalState._OnExit();
                GameGlobalState._OnDispose();
                GameGlobalState = null;
            }
        }

    }
}