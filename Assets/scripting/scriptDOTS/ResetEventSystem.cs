using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ResetEventSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       foreach (RefRW<SelectedUnit> selected in SystemAPI.Query<RefRW<SelectedUnit>>().WithPresent<SelectedUnit>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.deselected = false;
        }

    }

    
}
