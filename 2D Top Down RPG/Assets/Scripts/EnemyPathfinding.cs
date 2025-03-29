using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float avoidanceForce = 1.5f;
    [SerializeField] private float detectionRadius = 1.2f;

    [Header("Obstacle Settings")]
    [SerializeField] private LayerMask obstacleLayer;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void MoveTo(Vector2 targetPosition, float speedMultiplier = 1f)
    {
        Vector2 desiredDirection = (targetPosition - rb.position).normalized;
        Vector2 avoidance = GetAvoidanceDirection();

        Vector2 movementDirection = (desiredDirection + avoidance).normalized;
        rb.linearVelocity = movementDirection * (moveSpeed * speedMultiplier);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private Vector2 GetAvoidanceDirection()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, obstacleLayer);
        if (hits.Length == 0) return Vector2.zero;

        Vector2 avoidance = Vector2.zero;
        foreach (var hit in hits)
        {
            // Basic avoidance with slight distance weighting
            float distanceFactor = Mathf.Clamp01(1 - (Vector2.Distance(transform.position, hit.transform.position) / detectionRadius));
            avoidance += (Vector2)(transform.position - hit.transform.position).normalized * distanceFactor;
        }

        return avoidance.normalized * avoidanceForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}