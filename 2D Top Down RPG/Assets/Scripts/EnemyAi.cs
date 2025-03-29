using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private enum State
    {
        Idle,
        Roaming,
        Chasing
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private Transform playerTransform;
    private Vector2 roamPosition;

    [Header("Movement Settings")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float roamSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float roamPositionRange = 3f;

    [Header("Timing Settings")]
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    [SerializeField] private float minRoamTime = 2f;
    [SerializeField] private float maxRoamTime = 5f;

    private float currentStateTime;
    private float currentStateDuration;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetState(State.Idle);
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Always check for player chasing
        if (distanceToPlayer < chaseRange && state != State.Chasing)
        {
            SetState(State.Chasing);
            return;
        }
        else if (distanceToPlayer > chaseRange && state == State.Chasing)
        {
            SetState(State.Idle);
            return;
        }

        currentStateTime += Time.deltaTime;
        if (currentStateTime >= currentStateDuration)
        {
            switch (state)
            {
                case State.Idle:
                    SetState(State.Roaming);
                    break;
                case State.Roaming:
                    SetState(State.Idle);
                    break;
            }
        }

        if (state == State.Roaming)
        {
            enemyPathfinding.MoveTo(roamPosition, roamSpeed);

            // If close to roam position, get new one
            if (Vector2.Distance(transform.position, roamPosition) < 0.5f)
            {
                GetNewRoamPosition();
            }
        }
        else if (state == State.Chasing)
        {
            enemyPathfinding.MoveTo(playerTransform.position, chaseSpeed);
        }
        else // Idle state
        {
            enemyPathfinding.StopMoving();
        }
    }

    private void SetState(State newState)
    {
        state = newState;
        currentStateTime = 0f;

        switch (state)
        {
            case State.Idle:
                currentStateDuration = Random.Range(minIdleTime, maxIdleTime);
                break;
            case State.Roaming:
                currentStateDuration = Random.Range(minRoamTime, maxRoamTime);
                GetNewRoamPosition();
                break;
            case State.Chasing:
                // Chase until player leaves range
                currentStateDuration = float.MaxValue;
                break;
        }
    }

    private void GetNewRoamPosition()
    {
        roamPosition = (Vector2)transform.position + Random.insideUnitCircle * roamPositionRange;
    }
}