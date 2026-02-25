using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float refreshTime;

    [SerializeField] private UnityEvent OnTouchPlayer;

    private WaitForSeconds wait;
    private NavMeshAgent agent;


    // GAME //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wait = new WaitForSeconds(refreshTime);
    }

    void Start()
    {
        StartCoroutine(RefreshDestination());
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
    IEnumerator RefreshDestination()
    {
        while (true)
        {
            agent.SetDestination(target.position);
            yield return wait;
        }
    }
}
