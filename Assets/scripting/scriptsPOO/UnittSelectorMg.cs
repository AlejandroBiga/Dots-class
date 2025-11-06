using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.Transforms;


public class UnittSelectorMg : MonoBehaviour
{

    public static UnittSelectorMg Instance { get; private set; }        

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        Instance = this;
    }

   
    private void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SelectedUnit>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<SelectedUnit>(entityArray[i], false);
            }

            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<SelectedUnit>().Build(entityManager);

            Rect selectionAreaRect = GetSelectionAreaRect();


            entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<LocalTransform> localtransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            for (int i = 0; i < localtransformArray.Length; i++)
            {
                LocalTransform unitlocalTransform = localtransformArray[i];
                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitlocalTransform.Position);
               if(selectionAreaRect.Contains(unitScreenPosition))
                {
                    //aca va si la unidad desta dentro
                    entityManager.SetComponentEnabled<SelectedUnit>(entityArray[i], true);
                }

            }
            
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonDown(1))
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

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y
            );
    }


}
