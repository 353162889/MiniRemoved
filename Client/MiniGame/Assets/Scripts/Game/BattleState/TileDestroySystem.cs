using DG.Tweening;
using Unity.Entities;
using UnityEngine;

public class TileDestroySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, TileDestroyComponent tileDestroyComponent) =>
        {
            if (tileDestroyComponent.tweener != null)
            {
                if (!tileDestroyComponent.tweener.IsPlaying())
                {
                    tileDestroyComponent.tweener = null;
                    World.DestroyEntity(entity);
                }
            }
        });
    }

    public void DestroyEntity(Entity entity, float time = 0)
    {
        var tileDestroyComponent = World.GetComponent<TileDestroyComponent>(entity);
        var prefabComponent = World.GetComponent<PrefabComponent>(entity);
        if (time > 0)
        {
            tileDestroyComponent.tweener = prefabComponent.trans.DOScale(Vector3.zero, time).SetEase(Ease.InOutExpo);
        }
        else
        {
            World.DestroyEntity(entity);
        }
    }
}
