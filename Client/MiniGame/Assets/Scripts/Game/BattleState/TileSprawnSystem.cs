using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework;
using GameData;
using Unity.Entities;
using UnityEditor.Build.Reporting;
using Random = UnityEngine.Random;

public class TileSprawnSystem : ComponentSystem
{
    private TileMoveSystem tileMoveSystem;
    protected override void OnCreate()
    {
        tileMoveSystem = World.GetOrCreateSystem<TileMoveSystem>();
    }

    protected override void OnUpdate()
    {
        var tileSprawnComponent = World.GetSingletonComponent<TileSprawnComponent>();
        if (tileSprawnComponent.isTileDowning)
        {
            var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
            if (tileSprawnComponent.lstDownPos.Count > 0)
            {
                bool isMoving = false;
                foreach (var pos in tileSprawnComponent.lstDownPos)
                {
                    var entity = tileMapComponent.entities[pos.x, pos.y];
                    if (tileMoveSystem.IsMoving(entity))
                    {
                        isMoving = true;
                        break;
                    }
                }

                if (!isMoving)
                {
                    tileSprawnComponent.isTileDowning = false;
                    //移动完成
                    CheckAndRemoveTiles(tileMapComponent, tileSprawnComponent.lstDownPos);
                }
            }
            else
            {
                tileSprawnComponent.isTileDowning = false;
            }
        }
    }

    private void CheckAndRemoveTiles(TileMapComponent tileMapComponent, HashSet<Vector2Int> lstCheck)
    {
        var tileCheckSystem = World.GetExistingSystem<TileCheckSystem>();
        var lstClear = ResetObjectPool<HashSet<Vector2Int>>.Instance.GetObject();
        var result = tileCheckSystem.CheckRemoveTiles(tileMapComponent, lstCheck, lstClear);
        if (result)
        {
            RemoveTiles(tileMapComponent, lstClear);
        }

        ResetObjectPool<HashSet<Vector2Int>>.Instance.SaveObject(lstClear);
    }

    public void InitTiles()
    {
        var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
        //创建tile
        var entities = tileMapComponent.entities;
        for (int i = 0; i < tileMapComponent.rowCount; i++)
        {
            for (int j = 0; j < tileMapComponent.columnCount; j++)
            {
                if (entities[i, j] == Entity.Null)
                {
                    entities[i, j] = RandomCreateTile(tileMapComponent, i, j);
                }
            }
        }
    }

    /// <summary>
    /// 随机生成，现在只保证不会出现可直接消除的行或列
    /// </summary>
    /// <param name="tileMapComponent"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public Entity RandomCreateTile(TileMapComponent tileMapComponent, int row, int column)
    {
        var lstCfg = ResCfgSys.Instance.GetCfgLst<ResTile>();
        int excludeLeftId = -1;
        if (row > tileMapComponent.minRemoveCount - 2)
        {
            var minIdx = row - tileMapComponent.minRemoveCount + 1;
            var leftEntity = tileMapComponent.entities[row - 1, column];
            excludeLeftId = World.GetComponent<TileComponent>(leftEntity).tileId;
            for (int i = row - 1; i > minIdx; i--)
            {
                var leftTwoEntity = tileMapComponent.entities[i - 1, column];
                if (World.GetComponent<TileComponent>(leftTwoEntity).tileId != excludeLeftId)
                {
                    excludeLeftId = -1;
                    break;
                }
            }
        }

        int excludeDownId = -1;
        if (column > tileMapComponent.minRemoveCount - 2)
        {
            var minIdx = column - tileMapComponent.minRemoveCount + 1;
            var downEntity = tileMapComponent.entities[row, column - 1];
            excludeDownId = World.GetComponent<TileComponent>(downEntity).tileId;
            for (int i = column - 1; i > minIdx; i--)
            {
                var downTwoEntity = tileMapComponent.entities[row, i - 1];
                if (World.GetComponent<TileComponent>(downTwoEntity).tileId != excludeDownId)
                {
                    excludeDownId = -1;
                    break;
                }
            }
        }

        var lst = ResetObjectPool<List<int>>.Instance.GetObject();
        for (int i = 0; i < lstCfg.Count; i++)
        {
            var id = lstCfg[i].id;
            if (id != excludeLeftId && id != excludeDownId)
            {
                lst.Add(id);
            }
        }
        var index = Random.Range(0, lst.Count);
        var cfg = ResCfgSys.Instance.GetCfg<ResTile>(lst[index]);
        ResetObjectPool<List<int>>.Instance.SaveObject(lst);

        var entity = CreateTile(cfg, GetTileWorldPosition(tileMapComponent, new Vector2Int(row, column)));
        return entity;
    }

    public Entity CreateTile(ResTile cfg, Vector3 position)
    {
        var entity = World.CreateEntity();
        var tileComponent = World.AddComponentOnce<TileComponent>(entity);
        tileComponent.tileId = cfg.id;

        var prefabComponent = World.AddComponentOnce<PrefabComponent>(entity);
        prefabComponent.go = prefabComponent.gameObject;
        prefabComponent.trans = prefabComponent.transform;
        prefabComponent.trans.position = position;
        var tileView = new TileView();
        tileView.Init(cfg.path, prefabComponent.trans);
        prefabComponent.view = tileView;

        var tileMoveComponent = World.AddComponentOnce<TileMoveComponent>(entity);
        var tileDestroyComponent = World.AddComponentOnce<TileDestroyComponent>(entity);

        return entity;
    }

    public Vector3 GetTileWorldPosition(TileMapComponent tileMapComponent, Vector2Int pos)
    {
        float x = tileMapComponent.origin.x + (pos.y + tileMapComponent.tileWidth * 0.5f) * tileMapComponent.tileWidth;
        float y = tileMapComponent.origin.y + (pos.x + tileMapComponent.tileHeight * 0.5f) * tileMapComponent.tileHeight;
        float z = tileMapComponent.origin.z;
        return new Vector3(x,y,z);
    }

    public Vector2Int GetTileIdxPosition(TileMapComponent tileMapComponent, Vector3 worldPos)
    {
        var column = Mathf.FloorToInt((worldPos.x - tileMapComponent.origin.x) / tileMapComponent.tileWidth);
        var row = Mathf.FloorToInt((worldPos.y - tileMapComponent.origin.y) / tileMapComponent.tileHeight);
        return new Vector2Int(row, column);
    }

    public void RemoveTile(TileMapComponent tileMapComponent, Vector2Int pos, float time = 0)
    {
        var entities = tileMapComponent.entities;
        var entity = entities[pos.x, pos.y];
        World.GetOrCreateSystem<TileDestroySystem>().DestroyEntity(entity, time);
        entities[pos.x, pos.y] = Entity.Null;
    }

    //交换entity位置
    public void SwapTile(TileMapComponent tileMapComponent, Vector2Int aPos, Vector2Int bPos)
    {
        var temp = tileMapComponent.entities[aPos.x, aPos.y];
        tileMapComponent.entities[aPos.x, aPos.y] = tileMapComponent.entities[bPos.x, bPos.y];
        tileMapComponent.entities[bPos.x, bPos.y] = temp;
    }

    public void RemoveTiles(TileMapComponent tileMapComponent, HashSet<Vector2Int> lstClear, float time = 0.5f)
    {
        var tileSprawnComponent = World.GetSingletonComponent<TileSprawnComponent>();
        foreach (Vector2Int pos in lstClear)
        {
            RemoveTile(tileMapComponent, pos, time);
        }

        int maxRow = tileMapComponent.rowCount;
        int maxColumn = tileMapComponent.columnCount;
        var lstMovePos = ResetObjectPool<HashSet<Vector2Int>>.Instance.GetObject();
        var dicMinNullColumnToRow = ResetObjectPool<Dictionary<int, int>>.Instance.GetObject();
        for (int i = 0; i < maxColumn; i++)
        {
            dicMinNullColumnToRow.Add(i, maxRow);
        }
      
        foreach (Vector2Int pos in lstClear)
        {
            var clearEntity = tileMapComponent.entities[pos.x, pos.y];
            if (clearEntity != Entity.Null)continue;
            int row = pos.x + 1;
            int nullRow = pos.x;
            while (row < maxRow)
            {
                var entity = tileMapComponent.entities[row, pos.y];
                if (entity != Entity.Null)
                {
                    var nullPos = new Vector2Int(nullRow, pos.y);
                    var curEntityPos = new Vector2Int(row, pos.y);
                    SwapTile(tileMapComponent, nullPos, curEntityPos);
                    nullRow++;
                }
                row++;
            }

            if (dicMinNullColumnToRow[pos.y] > nullRow) dicMinNullColumnToRow[pos.y] = nullRow;
        }

        var lstCfg = ResCfgSys.Instance.GetCfgLst<ResTile>();
        foreach (var item in dicMinNullColumnToRow)
        {
            var column = item.Key;
            var startNullRow = item.Value;
            for (int i = 0; i < startNullRow; i++)
            {
                if (startNullRow < maxRow)
                {
                    lstMovePos.Add(new Vector2Int(i, column));
                }
            }
            for (int i = startNullRow; i < maxRow; i++)
            {
                var index = Random.Range(0, lstCfg.Count);
                var cfg = lstCfg[index];
                var worldPos = GetTileWorldPosition(tileMapComponent, new Vector2Int(maxRow + i - startNullRow, column));
                var entity = CreateTile(cfg, worldPos);
                tileMapComponent.entities[i, column] = entity;
                var newEntityPos = new Vector2Int(i, column);
                lstMovePos.Add(newEntityPos);
            }
        }

        tileSprawnComponent.lstDownPos.Clear();
        foreach (var pos in lstMovePos)
        {
            var entity = tileMapComponent.entities[pos.x, pos.y];
            var prefabComponent = World.GetComponent<PrefabComponent>(entity);
            tileMoveSystem.MoveTile(entity, prefabComponent.trans.position, GetTileWorldPosition(tileMapComponent, pos), tileSprawnComponent.downDuration, 0.3f);
            tileSprawnComponent.lstDownPos.Add(pos);
        }

        tileSprawnComponent.isTileDowning = true;

        ResetObjectPool<HashSet<Vector2Int>>.Instance.SaveObject(lstMovePos);
        ResetObjectPool<Dictionary<int,int>>.Instance.SaveObject(dicMinNullColumnToRow);
    }
}
