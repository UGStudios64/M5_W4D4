using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour
{ 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private NavMeshAgent agent;

    [Header("// MATERIALS -----------------------------------------------------------------------------------------")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Material deadMaterial;

    private Ray pointToRayMouse;


    // GAME //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        if (!CompareTag("Player")) return;

        if (Input.GetMouseButtonDown(0))
        {
            pointToRayMouse = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(pointToRayMouse,out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }


    // FUNCTIONS //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    public void Death()
    { 
        gameObject.tag = ("DEAD");
        rend.material = deadMaterial;
    }
}