using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class CreepAI : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] bool isRanged;
    [SerializeField] float avoidanceDistance;
    private GameObject target;
    private float stoppingDistance;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isRanged)
        {
            stoppingDistance = 5;
        }
        else
        {
            stoppingDistance = 1.3f;
        }
    }

    private void Update()
    {
        if(target == null)
        {
            TargettingLogic();
        }
        else
        {
            MovementLogic();
        }
    }

    private void TargettingLogic()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("BadCreep");

        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in objectsWithTag)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        target = bestTarget;

    }
    private void MovementLogic()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 direction = (targetPosition - currentPosition).normalized;

        // Check for nearby obstacles
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(currentPosition, avoidanceDistance);
        Vector3 avoidanceMove = Vector3.zero;

        foreach (Collider2D obstacle in obstacles)
        {
            if (obstacle.CompareTag(gameObject.tag))
            {
                // Calculate a move vector to avoid the obstacle
                Vector3 obstacleDirection = (currentPosition - obstacle.transform.position).normalized;
                avoidanceMove += obstacleDirection;
            }
        }

        if (avoidanceMove != Vector3.zero)
        {
            direction += avoidanceMove.normalized;
        }

        if (Mathf.Abs((currentPosition - targetPosition).sqrMagnitude) > stoppingDistance)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
