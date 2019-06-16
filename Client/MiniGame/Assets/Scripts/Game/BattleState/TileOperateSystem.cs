using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Framework;
using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Tilemaps;

public class TileOperateSystem : ComponentSystem
{
    private RenderSystem renderSystem;
    private TileSprawnSystem tileSprawnSystem;
    private TileMoveSystem tileMoveSystem;
    protected override void OnCreate()
    {
        renderSystem = World.GetOrCreateSystem<RenderSystem>();
        tileSprawnSystem = World.GetOrCreateSystem<TileSprawnSystem>();
        tileMoveSystem = World.GetOrCreateSystem<TileMoveSystem>();
    }

    protected override void OnUpdate()
    {
        var tileOperateComponent = World.GetSingletonComponent<TileOpeareteComponent>();
        if (tileOperateComponent.isSwapOperating)
        {
            var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
            var posA = tileOperateComponent.swapPosA;
            var posB = tileOperateComponent.swapPosB;
            var entityA = tileMapComponent.entities[posA.x, posA.y];
            var entityB = tileMapComponent.entities[posB.x, posB.y];
            if (!tileMoveSystem.IsMoving(entityA) && !tileMoveSystem.IsMoving(entityB))
            {
                tileOperateComponent.isSwapOperating = false;
                var lstClear = ResetObjectPool<HashSet<Vector2Int>>.Instance.GetObject();
                if (SwapTilePos(tileMapComponent, posA, posB, lstClear))
                {
                    tileSprawnSystem.RemoveTiles(tileMapComponent, lstClear);
                }
                else
                {
                    var startPosA = World.GetComponent<PrefabComponent>(entityA).trans.position;
                    var endPosA = tileSprawnSystem.GetTileWorldPosition(tileMapComponent, posA);
                    var startPosB = World.GetComponent<PrefabComponent>(entityB).trans.position;
                    var endPosB = tileSprawnSystem.GetTileWorldPosition(tileMapComponent, posB);
                    tileMoveSystem.MoveTile(entityA, startPosA, endPosA, tileOperateComponent.swapDuration);
                    tileMoveSystem.MoveTile(entityB, startPosB, endPosB, tileOperateComponent.swapDuration);
                }
                ResetObjectPool<HashSet<Vector2Int>>.Instance.SaveObject(lstClear);
            }
        }
    }

    public void SelectTile(Vector2Int pos)
    {
        var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
        if (pos.x < 0 || pos.y < 0 || pos.x >= tileMapComponent.rowCount ||
            pos.y >= tileMapComponent.columnCount) return;
        var entity = tileMapComponent.entities[pos.x, pos.y];
        if (entity == Entity.Null) return;
        var tileOperateComponent = World.GetSingletonComponent<TileOpeareteComponent>();
        if(tileOperateComponent.selectPos.x > -1 && tileOperateComponent.selectPos.y > -1)
        {
            if (Mathf.Abs(tileOperateComponent.selectPos.x - pos.x) +
                Mathf.Abs(tileOperateComponent.selectPos.y - pos.y) == 1)
            {
                if (tileOperateComponent.isSwapOperating) return;
                var posA = tileOperateComponent.selectPos;
                var posB = pos;
                tileOperateComponent.isSwapOperating = true;
                tileOperateComponent.swapPosA = posA;
                tileOperateComponent.swapPosB = posB;
                var entityA = tileMapComponent.entities[posA.x, posA.y];
                var entityB = tileMapComponent.entities[posB.x, posB.y];

                var startPosA = tileSprawnSystem.GetTileWorldPosition(tileMapComponent, posA);
                var startPosB = tileSprawnSystem.GetTileWorldPosition(tileMapComponent, posB);

                var endPosA = startPosB;
                var endPosB = startPosA;
                tileMoveSystem.MoveTile(entityA, startPosA, endPosA, tileOperateComponent.swapDuration);
                tileMoveSystem.MoveTile(entityB, startPosB, endPosB, tileOperateComponent.swapDuration);
                //隐藏特效
                SetSelectPos(tileOperateComponent, new Vector2Int(-1, -1), false);
            }
            else
            {
                SetSelectPos(tileOperateComponent, pos, true);
            }
        }
        else
        {
            SetSelectPos(tileOperateComponent, pos, true);
        }
    }

    private void SetSelectPos(TileOpeareteComponent tileOperateComponent, Vector2Int pos, bool effectActive )
    {
        tileOperateComponent.selectPos = pos;
        var prefabComponent = World.GetComponent<PrefabComponent>(tileOperateComponent.effectEntity);
        prefabComponent.go.SetActive(effectActive);
        var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
        prefabComponent.trans.position =
            tileSprawnSystem.GetTileWorldPosition(tileMapComponent, tileOperateComponent.selectPos);
    }

    public Entity CreateEffectEntity(string name)
    {
        var effectEntity = World.CreateEntity();
        var prefabComponent = World.AddComponentOnce<PrefabComponent>(effectEntity);
        prefabComponent.go = prefabComponent.gameObject;
        prefabComponent.trans = prefabComponent.transform;
        var view = new EffectView();
        view.Init(name, false, prefabComponent.transform);
        prefabComponent.view = view;
        return effectEntity;
    }

    

//    private void CheckStateAll(TileMapComponent tileMapComponent, byte[,] stateCaches)
//    {
//        var entities = tileMapComponent.entities;
//
//        var maxRow = stateCaches.GetLength(0);
//        var maxColumn = stateCaches.GetLength(1);
//        for (int row = 0; row < maxRow; row++)
//        {
//            for (int column = 0; column < maxColumn; column++)
//            {
//                var state = stateCaches[row, column];
//                if (state == TileCheckState.WaitCheck)
//                {
//                    CheckState(tileMapComponent, stateCaches, row, column);
//                }
//            }
//        }
//    }

    public bool SwapTilePos(TileMapComponent tileMapComponent, Vector2Int aPos, Vector2Int bPos, HashSet<Vector2Int> lstClear)
    {
        //先交换数据
        tileSprawnSystem.SwapTile(tileMapComponent, aPos, bPos);
        var tileCheckSystem = World.GetExistingSystem<TileCheckSystem>();
        var lstCheck = ResetObjectPool<HashSet<Vector2Int>>.Instance.GetObject();
        lstCheck.Add(aPos);
        lstCheck.Add(bPos);
        var result = tileCheckSystem.CheckRemoveTiles(tileMapComponent, lstCheck, lstClear);
        if(!result)
        {
            //如果没有成功，将数据还原回去
            tileSprawnSystem.SwapTile(tileMapComponent, aPos, bPos);
        }

        return result;
    }
}
