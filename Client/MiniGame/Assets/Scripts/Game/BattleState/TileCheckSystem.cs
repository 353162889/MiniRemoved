using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TileCheckSystem : ComponentSystem
{
    public bool CheckRemoveTiles(TileMapComponent tileMapComponent, HashSet<Vector2Int> lstCheckPos, HashSet<Vector2Int> lstCheckResult)
    {
        var tileCheckComponent = World.GetSingletonComponent<TileCheckComponent>();
        var stateCaches = tileCheckComponent.stateCaches;
        ClearState(stateCaches, TileCheckState.None);
        foreach (var checkPos in lstCheckPos)
        {
            CheckState(tileMapComponent, stateCaches, checkPos.x, checkPos.y);
        }
        //获取所有状态为CheckedSuccess的位置
        for (int row = 0; row < stateCaches.GetLength(0); row++)
        {
            for (int column = 0; column < stateCaches.GetLength(1); column++)
            {
                if (0 != (stateCaches[row, column] & TileCheckState.CheckedSuccess))
                {
                    lstCheckResult.Add(new Vector2Int(row, column));
                }
            }
        }

        bool result = lstCheckResult.Count > 0;
        return result;
    }

    private void ClearState(byte[,] stateCaches, byte state)
    {
        var maxRow = stateCaches.GetLength(0);
        var maxColumn = stateCaches.GetLength(1);
        for (int row = 0; row < maxRow; row++)
        {
            for (int column = 0; column < maxColumn; column++)
            {
                stateCaches[row, column] = state;
            }
        }
    }



    private bool CheckOnePosState(Entity[,] entities, byte[,] stateCaches, int row, int column, int checkTileId, byte checkState)
    {
        var curState = stateCaches[row, column];
        if ((curState & checkState) == 0)
        {
            var entity = entities[row, column];
            if (entity == Entity.Null) return false;
            var tileComponent = World.GetComponent<TileComponent>(entity);
            if (tileComponent.tileId == checkTileId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void UpdateState(byte[,] stateCaches, int startRow, int endRow, int startColumn, int endColumn, byte state, bool replace)
    {
        for (int i = startRow; i <= endRow; i++)
        {
            for (int j = startColumn; j <= endColumn; j++)
            {
                if (replace)
                {
                    stateCaches[i, j] = state;
                }
                else
                {
                    stateCaches[i, j] = (byte)(stateCaches[i, j] | state);
                }
            }
        }
    }

    private void CheckState(TileMapComponent tileMapComponent, byte[,] stateCaches, int row, int column)
    {
        var entities = tileMapComponent.entities;
        var curEntity = entities[row, column];
        var curTileComponent = World.GetComponent<TileComponent>(curEntity);
        var curTileId = curTileComponent.tileId;
        var maxRow = stateCaches.GetLength(0);
        var maxColumn = stateCaches.GetLength(1);
        int left = column - 1;
        while (left >= 0)
        {
            if (!CheckOnePosState(entities, stateCaches, row, left, curTileId, TileCheckState.RowChecked)) break;
            left = left - 1;
        }

        int right = column + 1;
        while (right < maxColumn)
        {
            if (!CheckOnePosState(entities, stateCaches, row, right, curTileId, TileCheckState.RowChecked)) break;
            right = right + 1;
        }
        var rowCount = right - left - 1;
        if (rowCount >= tileMapComponent.minRemoveCount)
        {
            UpdateState(stateCaches, row, row, left + 1, right - 1, TileCheckState.CheckedSuccess, false);
        }
        else
        {
            UpdateState(stateCaches, row, row, left + 1, right - 1, TileCheckState.RowChecked, false);
        }

        int bottom = row - 1;
        while (bottom >= 0)
        {
            if (!CheckOnePosState(entities, stateCaches, bottom, column, curTileId, TileCheckState.ColumnChecked)) break;
            bottom = bottom - 1;
        }

        int up = row + 1;
        while (up < maxRow)
        {
            if (!CheckOnePosState(entities, stateCaches, up, column, curTileId, TileCheckState.ColumnChecked)) break;
            up = up + 1;
        }
        var columnCount = up - bottom - 1;
        if (columnCount >= tileMapComponent.minRemoveCount)
        {
            UpdateState(stateCaches, bottom + 1, up - 1, column, column, TileCheckState.CheckedSuccess, false);
        }
        else
        {
            UpdateState(stateCaches, bottom + 1, up - 1, column, column, TileCheckState.ColumnChecked, false);
        }
    }

    protected override void OnUpdate()
    {
        
    }
}
