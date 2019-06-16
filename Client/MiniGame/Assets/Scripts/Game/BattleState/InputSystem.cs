using UnityEngine;
using System.Collections;
using Framework;
using Unity.Entities;

public class InputSystem : ComponentSystem
{
    private bool m_bIsClick;
    private Vector2 m_sClickPosition;
    protected override void OnCreateManager()
    {
        TouchDispatcher.Instance.touchBeganListeners += InstanceOnTouchBeganListeners;
    }

    protected override void OnDestroyManager()
    {
        TouchDispatcher.Instance.touchBeganListeners -= InstanceOnTouchBeganListeners;
    }

    private void InstanceOnTouchBeganListeners(TouchEventParam obj)
    {
        if (obj.TouchCount < 1) return;
        var touchInfo = obj.GetTouch(0);
        m_bIsClick = true;
        m_sClickPosition = touchInfo.position;
    }

    protected override void OnUpdate()
    {
        if (m_bIsClick)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(m_sClickPosition);
            var tileMapComponent = World.GetSingletonComponent<TileMapComponent>();
            var pos = World.GetOrCreateSystem<TileSprawnSystem>().GetTileIdxPosition(tileMapComponent, worldPos);
            if (pos.x > -1 && pos.x < tileMapComponent.rowCount && pos.y > -1 && pos.y < tileMapComponent.columnCount)
            {
                var tileOperateSystem = World.GetOrCreateSystem<TileOperateSystem>();
                tileOperateSystem.SelectTile(pos);
            }
        }

        m_bIsClick = false;
    }
}
