using UnityEngine;
using UnityEngine.AI;

public class MoveAlongPoints : Enemy
{
    [Header("// POINT PATH -----------------------------------------------------------------------------------------")]
    [SerializeField] private Transform[] waypoints; 
    [SerializeField] private float reachThreshold; 

    private int currentPointIndex = 0;


    // FUNCTIONS //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    protected override void PatrolUpdate()
    {
        if (waypoints.Length == 0) return;

        Transform targetPoint = waypoints[currentPointIndex];
        if (!agent.hasPath || Vector3.Distance(transform.position, targetPoint.position) < reachThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentPointIndex].position);
        }
    }

    protected override void ReturnUpdate()
    {
        agent.SetDestination(startPosition);

        if (Vector3.Distance(transform.position, startPosition) < 0.5f)
        {
            transform.rotation = startRotation;
            lastTurnTime = Time.time;

            state = STATE.PATROL;
        }
    }
}