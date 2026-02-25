using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{ 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private NavMeshAgent agent;

    private Ray pointToRayMouse;


    // GAME //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointToRayMouse = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(pointToRayMouse,out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}