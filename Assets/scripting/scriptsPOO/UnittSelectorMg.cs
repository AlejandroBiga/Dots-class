using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.Transforms;
using Unity.Physics;


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

            //definicion del maximo y minimo de un area de deteccion para tomar solamente 1 unidad
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSIzeMin = 40f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionSIzeMin;

            if (isMultipleSelection)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<SelectedUnit>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localtransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                for (int i = 0; i < localtransformArray.Length; i++)
                {
                    LocalTransform unitlocalTransform = localtransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitlocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        //aca va si la unidad desta dentro
                        entityManager.SetComponentEnabled<SelectedUnit>(entityArray[i], true);
                    }

                }
            }
            else
            {
                // seleccionar solo 1
                
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>(); 
                CollisionWorld collisionWolrd = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int unitsLayer = 6;
                RaycastInput raycastInput = new RaycastInput {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter {
                        //como no se puede convertir un int en unidad se le pone la "u" al 1 para hacer bien el bitmask
                        BelongsTo = ~0u,
                           CollidesWith = 1u << unitsLayer,
                                GroupIndex = 0,
                    }
                };
                if(collisionWolrd.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Unit>(raycastHit.Entity))
                    {
                        // selecciona la unidad
                        entityManager.SetComponentEnabled<SelectedUnit>(raycastHit.Entity, true);
                    }
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
