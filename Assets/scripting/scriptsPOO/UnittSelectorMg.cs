using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnittSelectorMg : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 mouseWorldPosition = MousePointer.instance.GetMousePosition();
            EntityManager entityManager   = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MoveUnitComponent, SelectedUnit>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveUnitComponent> unityMoveArray = entityQuery.ToComponentDataArray<MoveUnitComponent>(Allocator.Temp);

            for (int i = 0; i < unityMoveArray.Length;  i++)
            {
                MoveUnitComponent unitMove = unityMoveArray[i];
                unitMove.TargetPosition = mouseWorldPosition;
                entityManager.SetComponentData(entityArray[i], unitMove);

            }
            


        }
    }


}
