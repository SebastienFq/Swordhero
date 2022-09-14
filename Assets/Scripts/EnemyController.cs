using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private const float c_RecalculatePathDelay = 0.2f;

    [Header("References")]
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Rigidbody m_Body;
    [SerializeField] private Health m_Health;

    [Header("Settings")]
    [SerializeField] private int m_TotalHealth = 100;
    [SerializeField] private float m_MoveSpeed = 10;
    [SerializeField] private float m_MinMoveDistance = 2;
    [SerializeField] private float m_OffsetMoveDistance = 4;
    [SerializeField] private float m_AttackDuration = 8;
    [SerializeField] private float m_AttackDurationOffset = 4;
    [SerializeField] private float m_WanderDuration = 8;
    [SerializeField] private float m_WanderDurationOffset = 4;
    [SerializeField] private float m_WanderPauseDuration = 2;
    [SerializeField] private float m_WanderPauseDurationOffset = 2;

    //private IEnumerator moveRoutine;
    private float m_DistanceFromPlayer = 0;
    private bool m_IsAttacking = false;
    private bool m_IsWandering = false;
    private float m_TimerRecalculatePath;
    private PlayerController m_Player;
    private NavMeshPath path;
    private int m_NextTarget;
    private Vector3 m_Destination;
    private Vector3 m_MovingDirection;
    private float m_TimerAction;
    private float m_TimerPause;

    public static Action<EnemyController> onDeath;
    public Health Health => m_Health;
    public float DistanceFromPlayer => m_DistanceFromPlayer;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;

        Health.onDeath += Death;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;

        Health.onDeath -= Death;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Constants.Tags.c_WeaponHitbox)
        {
            m_Health.AddHealth(-other.GetComponent<HitBox>().Damages);
        }
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        if (m_IsAttacking)
        {
            m_TimerRecalculatePath -= Time.deltaTime;

            if (m_TimerRecalculatePath <= 0)
            {
                NavMesh.CalculatePath(transform.position, m_Player.transform.position, NavMesh.AllAreas, path);
                m_TimerRecalculatePath = c_RecalculatePathDelay;
            }

            Vector3 lastPoint = transform.position;

            for (int i = 0; i < path.corners.Length; i++)
            {
                Debug.DrawLine(lastPoint, path.corners[i], Color.cyan, Time.deltaTime);
                lastPoint = path.corners[i];
            }

            Debug.DrawLine(lastPoint, m_Player.transform.position, Color.cyan, Time.deltaTime);

            Vector3 dir = ((path.corners.Length > 1 ? path.corners[1] : m_Player.transform.position) - transform.position).normalized;
            dir.y = 0;

            SetMovingDirection(dir);

            m_TimerAction -= Time.deltaTime;

            if(m_TimerAction <= 0)
            {
                StopAttack();
            }
        }
        else
        {
            if(m_IsWandering)
            {
                if (path.corners.Length == 0) return;

                Vector3 lastPoint = path.corners[0];

                for (int i = 0; i < path.corners.Length; i++)
                {
                    Debug.DrawLine(lastPoint, path.corners[i], Color.cyan, Time.deltaTime);
                    lastPoint = path.corners[i];
                }

                if (path.corners.Length > 1)
                {
                    if (Vector3.Distance(transform.position, path.corners[m_NextTarget]) < 0.5f)
                    {
                        if (m_NextTarget + 1 >= path.corners.Length)
                        {
                            ReachDestination();
                            return;
                        }
                        else
                        {
                            m_NextTarget++;
                        }
                    }
                }

                Vector3 dir = ((path.corners.Length > 1 ? path.corners[m_NextTarget] : m_Player.transform.position) - transform.position).normalized;
                dir.y = 0;

                SetMovingDirection(dir);             
            }
            else
            {
                m_TimerPause -= Time.deltaTime;

                if (m_TimerPause <= 0)
                {
                    m_IsWandering = true;
                }
            }

            m_TimerAction -= Time.deltaTime;

            if(m_TimerAction <= 0)
            {
                StartAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_IsAttacking || m_IsWandering)
        {
            m_Body.MovePosition(m_Body.position + m_MovingDirection * Time.fixedDeltaTime * m_MoveSpeed);
        }
    }

    public void Init()
    {
        path = new NavMeshPath();
        m_Health.Init(m_TotalHealth);
        m_Player = UnitManager.Instance.Player;
        StopAttack();
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.RESET:
                break;
        }
    }

    private bool FindRandomDestination()
    {
        Vector2 c = UnityEngine.Random.insideUnitCircle;
        var randomDirection = new Vector3(c.x, 0, c.y);

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(transform.position + randomDirection * (m_MinMoveDistance + m_OffsetMoveDistance * UnityEngine.Random.value), out hit, m_MinMoveDistance + m_OffsetMoveDistance, NavMesh.AllAreas))
        {
            return false;
        }

        m_Destination = hit.position;
        NavMesh.CalculatePath(transform.position, m_Destination, NavMesh.AllAreas, path);
        m_NextTarget = 1;
        ExtensionMethods.DrawDot(m_Destination, 1, Color.magenta, 5f);
        return true;
    }

    private void StartAttack()
    {
        m_IsAttacking = true;
        m_IsWandering = false;
        m_TimerAction = m_AttackDuration + m_AttackDurationOffset * Random.value;
        m_Renderer.material.SetColor("_EmissioNnColor", Color.red);
    }

    private void StopAttack()
    {
        m_IsAttacking = false;
        m_IsWandering = false;
        m_TimerAction = m_WanderDuration + m_WanderDurationOffset * Random.value;
        m_Renderer.material.SetColor("_EmissioNnColor", Color.black);
    }

    private void ReachDestination()
    {
        m_TimerPause = m_WanderPauseDuration = m_WanderPauseDurationOffset * Random.value;
        m_IsWandering = false;       
    }

    private void SetMovingDirection(Vector3 direction)
    {
        m_MovingDirection = direction;
    }

    public void SetDistanceFromPlayer(float _Dist)
    {
        m_DistanceFromPlayer = _Dist;
    }

    public void Destroy()
    {
        m_Health.Destroy();
        Destroy(gameObject);
    }

    private void Death()
    {
        onDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
