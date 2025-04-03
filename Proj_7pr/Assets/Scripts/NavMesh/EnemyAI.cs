using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public MovePlayer target;
    public Player player;

    [SerializeField]
    private TMP_Text boomTextPrefab;
    [SerializeField]
    private float hideRangeMax = 30f, trackingRange = 10f, detectionRange = 3f, speed = 3.5f, attackInterval = 1f, explosionTime = 5f, explosionRadius = 5f, patrolRadius = 5f, hideSearchRadius = 8f, hideDurationMin = 10f, hideDurationMax = 20f;
    [SerializeField]
    private int explosionDamage = 20;

    private NavMeshAgent agent;
    private Renderer enemyRenderer;
    private bool isAttacking = false, isPatrolling = false, isHiding = false;
    private float attackTimer = 0f, explosionTimer = 0f;
    private Vector3 lastSeenPlayerPosition;
    private Color originalColor;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;
        agent.speed = speed;
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (target == null || player == null)
        {
            target = FindAnyObjectByType<MovePlayer>();
            player = FindAnyObjectByType<Player>();
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        bool canSeePlayer = HasLineOfSight();

        if (distanceToTarget <= detectionRange)
        {
            isPatrolling = false;
            isHiding = false;

            if (!isAttacking)
            {
                isAttacking = true;
                agent.isStopped = true;
            }

            attackTimer += Time.deltaTime;
            explosionTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                StartCoroutine(FlashRed());
            }

            if (explosionTimer >= explosionTime)
            {
                Explode();
            }
        }
        else if (distanceToTarget > trackingRange && distanceToTarget <= hideRangeMax && canSeePlayer && !isHiding)
        {
            lastSeenPlayerPosition = target.transform.position;
            isHiding = true;
            isPatrolling = false;
            isAttacking = false;
            Vector3 hideSpot = FindHidingSpot();

            if (hideSpot != Vector3.zero)
            {
                agent.SetDestination(hideSpot);
                StartCoroutine(HideRoutine());
            }
            else
            {
                agent.SetDestination(lastSeenPlayerPosition);
            }
        }
        else if (distanceToTarget <= trackingRange && canSeePlayer)
        {
            lastSeenPlayerPosition = target.transform.position;
            isHiding = false;
            isPatrolling = false;
            isAttacking = false;
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
            enemyRenderer.material.color = originalColor;
        }
        else if (!isPatrolling && !isHiding)
        {
            StartCoroutine(PatrolRoutine());
        }
    }

    IEnumerator HideRoutine()
    {
        float hideTime = Random.Range(hideDurationMin, hideDurationMax);
        yield return new WaitForSeconds(hideTime);
        isHiding = false;
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = true;

        while (isPatrolling)
        {
            Vector3 patrolPoint = GetRandomPatrolPoint();
            agent.SetDestination(patrolPoint);
            agent.isStopped = false;

            while (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                yield return null;
            }
        }
    }

    Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    Vector3 FindHidingSpot()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, hideSearchRadius);
        Vector3 bestHidingSpot = Vector3.zero;
        float bestDistance = 0f;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Cover"))
            {
                Vector3 potentialSpot = col.transform.position;
                float distanceFromPlayer = Vector3.Distance(potentialSpot, target.transform.position);
                float distanceFromEnemy = Vector3.Distance(potentialSpot, transform.position);

                if (distanceFromPlayer > distanceFromEnemy && distanceFromPlayer > bestDistance)
                {
                    bestHidingSpot = potentialSpot;
                    bestDistance = distanceFromPlayer;
                }
            }
        }

        return bestHidingSpot;
    }

    bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            return hit.collider.gameObject == target.gameObject;
        }
        return false;
    }

    void Explode()
    {
        ShowBoomText();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<MovePlayer>(out MovePlayer hitPlayer))
            {
                player.TakeDamage(explosionDamage);
            }
        }

        StopAllCoroutines();
        Destroy(gameObject);
    }

    void ShowBoomText()
    {
        if (boomTextPrefab != null)
        {
            TMP_Text boomText = Instantiate(boomTextPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            Destroy(boomText.gameObject, 1f);
        }
    }

    IEnumerator FlashRed()
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = originalColor;
    }
}
