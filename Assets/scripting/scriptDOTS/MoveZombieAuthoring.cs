using Unity.Entities;
using UnityEngine;

public class MoveZombieAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public float RotationSpeed;
    public class Baker : Baker<MoveZombieAuthoring>
    {
        public override void Bake(MoveZombieAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveUnitComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                RotationSpeed = authoring.RotationSpeed     
            });
                

            
        }
    }


}
public struct MoveUnitComponent : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;

}
