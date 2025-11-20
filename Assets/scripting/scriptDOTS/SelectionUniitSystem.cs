using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventSystem))]
partial struct SelectionUniitSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach  (RefRO<SelectedUnit> selected in SystemAPI.Query<RefRO<SelectedUnit>>().WithPresent<SelectedUnit>())
        {
            
            if (selected.ValueRO.onSelected)
            {
                Debug.Log("onselected");
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEffect);
                visualLocalTransform.ValueRW.Scale = selected.ValueRO.DepthScale;
            }
            if (selected.ValueRO.deselected)
            {
                Debug.Log("deselected");
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEffect);
                visualLocalTransform.ValueRW.Scale = 0f;
            }


        }

        
    }

   
}
