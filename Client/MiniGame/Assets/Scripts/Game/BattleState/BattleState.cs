using UnityEngine;
using System.Collections;
using Framework;
using Unity.Collections;
using Unity.Entities;

namespace Game
{
    public class BattleState : StateBase
    {
        private GameObjectWorld world;

        private InputSystem inputSystem;
        private TileMoveSystem tileMoveSystem;
        private TileOperateSystem tileOperateSystem;
        private TileSprawnSystem tileSprawnSystem;

        protected override void OnEnter()
        {
            CLog.Log("BattleStateEnter");
            world = new GameObjectWorld("Battle");

            inputSystem = world.GetOrCreateSystem<InputSystem>();
            tileMoveSystem = world.GetOrCreateSystem<TileMoveSystem>();
            tileOperateSystem = world.GetOrCreateSystem<TileOperateSystem>();
            tileSprawnSystem = world.GetOrCreateSystem<TileSprawnSystem>();
            var renderSystem = world.GetOrCreateSystem<RenderSystem>();
            var tileCheckSystem = world.GetOrCreateSystem<TileCheckSystem>();
            var tileDestroySystem = world.GetOrCreateSystem<TileDestroySystem>();

            //初始化各个单例系统
            //出事化地图数据
            var tileMapComponent = world.AddSingletonComponent<TileMapComponent>();
            tileMapComponent.origin = Vector3.zero;
            tileMapComponent.rowCount = 10;
            tileMapComponent.columnCount = 8;
            tileMapComponent.tileWidth = 1;
            tileMapComponent.tileHeight = 1;
            tileMapComponent.entities = new Entity[tileMapComponent.rowCount, tileMapComponent.columnCount];
            tileMapComponent.minRemoveCount = 3;

            var tileCheckComponent = world.AddSingletonComponent<TileCheckComponent>();
            tileCheckComponent.stateCaches = new byte[tileMapComponent.rowCount, tileMapComponent.columnCount];

            var tileOpearteComponent = world.AddSingletonComponent<TileOpeareteComponent>();
            tileOpearteComponent.selectPos = new Vector2Int(-1, -1);

            var tileSprawnComponent = world.AddSingletonComponent<TileSprawnComponent>();
            tileSprawnSystem.InitTiles();

            //打开战斗界面
            ViewSys.Instance.Open("BattleView");
        }

        protected override void OnExit()
        {
            CLog.Log("BattleStateExit");
            ViewSys.Instance.Close("BattleView");
            ViewSys.Instance.Close("ResultView");
            if (world != null)
            {
                world.Dispose();
                world.DestroyWorld();
            }
            world = null;
        }

        protected override void OnUpdate()
        {
            inputSystem.Update();
            tileMoveSystem.Update();
            tileOperateSystem.Update();
            tileSprawnSystem.Update();
        }
    }
}
