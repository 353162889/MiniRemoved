using DG.Tweening;
using Framework;
using Unity.Entities;
using UnityEngine;

public class TileMoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((TileMoveComponent moveComponent) =>
        {
            if (moveComponent.tweener != null)
            {
                if (!moveComponent.tweener.IsPlaying())
                {
                    moveComponent.tweener = null;
                }
            }
        });
    }

    public void MoveTile(Entity entity, Vector3 startPos, Vector3 endPos, float duration)
    {
        var tileMoveComponent = World.GetComponent<TileMoveComponent>(entity);
        if (tileMoveComponent == null) return;
        tileMoveComponent.tweener?.Kill(true);
        var prefabComponent = World.GetComponent<PrefabComponent>(entity);
        prefabComponent.trans.position = startPos;
        tileMoveComponent.tweener = prefabComponent.trans.DOMove(endPos, duration);

    }

    public bool IsMoving(Entity entity)
    {
        var tileMoveComponent = World.GetComponent<TileMoveComponent>(entity);
        if (tileMoveComponent == null || tileMoveComponent.tweener == null) return false;
        return tileMoveComponent.tweener.IsPlaying();
    }
}
