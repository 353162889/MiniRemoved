using System.Numerics;
using Unity.Entities;

public class RenderSystem : ComponentSystem
{

    public void SetActive(Entity entity, bool active)
    {
        var prefabComponent = World.GetComponent<PrefabComponent>(entity);
        if (null != prefabComponent)
        {
            if (null != prefabComponent.go && prefabComponent.go.activeSelf != active)
            {
                prefabComponent.go.SetActive(active);
            }
        }
    }

    public void SetPosition(Entity entity, Vector3 position)
    {

    }

    protected override void OnUpdate()
    {
    }
}
