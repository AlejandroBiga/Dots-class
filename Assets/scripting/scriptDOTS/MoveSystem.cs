using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
partial struct MoveSystem : ISystem
{
   
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
         foreach((RefRW<LocalTransform> localTransform,
                 RefRO<MoveUnitComponent> moveUnit,
                 RefRW<PhysicsVelocity> physicsVelocity)
                    in SystemAPI.Query<
                        RefRW<LocalTransform>,
                        RefRO<MoveUnitComponent>,
                        RefRW<PhysicsVelocity>>())
        {

            float3 targetPosition = MousePointer.instance.GetMousePosition();
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRW.Rotation, 
                quaternion.LookRotation(moveDirection, math.up()), SystemAPI.Time.DeltaTime * moveUnit.ValueRO.RotationSpeed);



            physicsVelocity.ValueRW.Linear = moveDirection * moveUnit.ValueRO.MoveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
        
            
      
        
    }

}
