
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public enum STATE { PATROL = 1, CHASE = 2, RETURN = 3 }
    protected NavMeshAgent agent;
    
    [SerializeField] protected STATE state;
    [SerializeField] private float updatePathInterval;
    private float lastPathUpdateTime;

    [Header("// WHAT HE SEE -----------------------------------------------------------------------------------------")]
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float viewAngle = 120f;

    [Header("// CHASE MEMORY -----------------------------------------------------------------------------------------")]
    [SerializeField] private float loseTargetDelay = 1.5f;
    private float lastTimeSawTarget;

    [Header("// PATROL TURN -----------------------------------------------------------------------------------------")]
    [SerializeField] private float patrolTurnInterval = 3f;
    [SerializeField] private float patrolTurnAngle = 90f;
    protected float lastTurnTime;
    private bool turned = false;

    [Header("// MATERIALS -----------------------------------------------------------------------------------------")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material chaseMaterial;

    protected Vector3 startPosition;
    protected Quaternion startRotation;

    [SerializeField] private UnityEvent OnTouchPlayer;


    // GAME //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        startPosition = transform.position;
        startRotation = transform.rotation;

        state = STATE.PATROL;
    }

    void Update()
    {
        switch (state)
        {
            case STATE.PATROL:
                PatrolUpdate();
                if (SeeTheTarget())
                {
                    state = STATE.CHASE;
                    SetChaseMaterial(true);
                }
                break;

            case STATE.CHASE:
                ChaseUpdate();
                if (SeeTheTarget()) lastTimeSawTarget = Time.time;
                else if (Time.time - lastTimeSawTarget > loseTargetDelay)
                {
                    state = STATE.RETURN;
                    SetChaseMaterial(false);
                }
                break;

            case STATE.RETURN:
                ReturnUpdate();
                if (SeeTheTarget())
                {
                    state = STATE.CHASE;
                    SetChaseMaterial(true);
                }
                break;
        }

        if (SeeTheTarget()) lastTimeSawTarget = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player Hit");
            OnTouchPlayer.Invoke();
        }
    }


    // FUNCTIONS //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private bool SeeTheTarget()
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f; 
        Vector3 directionToTarget = target.position + Vector3.up * 0.3f - origin;
        float distance = directionToTarget.magnitude;

        if (distance > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > viewAngle / 2f) return false;

        // RAYCAST
        if (Physics.Raycast(origin, directionToTarget.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.transform != target && ((1 << hit.transform.gameObject.layer) & obstacleMask) != 0) return false;
        }

        return true;
    }


    // STATES ----------------------------------------------------------------------------------------------------
    protected virtual void PatrolUpdate()
    {
        if (Time.time - lastTurnTime > patrolTurnInterval)
        {
            agent.updateRotation = true;
            float angle = turned ? -patrolTurnAngle : patrolTurnAngle;
            transform.Rotate(0f, angle, 0f);
            turned = !turned;
            lastTurnTime = Time.time;
        }
    }

    private void ChaseUpdate()
    {
        if (target != null && Time.time - lastPathUpdateTime > updatePathInterval)
        {
            agent.updateRotation = true;
            agent.SetDestination(target.position);
            lastPathUpdateTime = Time.time;
        }
    }

    private void SetChaseMaterial(bool chasing)
    {
        if (enemyRenderer == null) return;
        enemyRenderer.material = chasing ? chaseMaterial : normalMaterial;
    }

    protected virtual void ReturnUpdate()
    {
        agent.updateRotation = true;
        agent.SetDestination(startPosition);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            state = STATE.PATROL;
            lastTurnTime = Time.time;
            SetChaseMaterial(false);

            StartCoroutine(SnapRotationAfterReturn());
        }
    }

    IEnumerator SnapRotationAfterReturn()
    {
        agent.isStopped = true;
        yield return null; // 1 frame

        transform.rotation = startRotation;
        agent.isStopped = false;
    }

    // GIZMO ----------------------------------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward * viewDistance;
        Quaternion leftRayRotation = Quaternion.Euler(0, -viewAngle / 2, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, viewAngle / 2, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftRayRotation * forward);
        Gizmos.DrawRay(transform.position, rightRayRotation * forward);
    }
}