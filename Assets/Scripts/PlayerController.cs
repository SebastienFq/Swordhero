using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator m_Animator = null;
    [SerializeField] private Joystick m_Joystick = null;
    [SerializeField] private Health m_Health = null;
    [SerializeField] private Transform m_Graphics = null;
    [SerializeField] private GameObject m_TargetIndicator = null;

    [Header("Settings")]
    [SerializeField] private int m_MaxHealth = 100;

    private float m_MoveSpeed = 5;
    private bool m_IsMoving;
    private NavMeshHit m_NavMeshHit;
    private EnemyController m_NearestUnit;
    private EnemyController m_Target;
    private bool m_HasTarget;
    private bool m_HasNearestUnit;
    private Vector2 m_JoyDir;
    private Vector3 m_WorldDir;

    public Health Health => m_Health;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded += OnGamePhaseEnded;
        LevelManager.onLevelSpawned += OnLevelSpawned;
        UnitManager.onUnitsUpdated += OnUnitsUpdated;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded -= OnGamePhaseEnded;
        LevelManager.onLevelSpawned -= OnLevelSpawned;
        UnitManager.onUnitsUpdated -= OnUnitsUpdated;
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        UpdateMovement();

        if(m_IsMoving)
        {
            if (m_HasNearestUnit)
            {
                if(!m_TargetIndicator.activeSelf)
                    m_TargetIndicator.gameObject.SetActive(true);

                m_TargetIndicator.transform.position = m_NearestUnit.transform.position;
            }
            else
                m_TargetIndicator.gameObject.SetActive(false);
        }
        else
        {
            if(m_HasTarget)
            {
                if (!m_TargetIndicator.activeSelf)
                    m_TargetIndicator.gameObject.SetActive(true);

                m_TargetIndicator.transform.position = m_Target.transform.position;
            }            
            else
                m_TargetIndicator.gameObject.SetActive(false);
        }
    }

    private void OnLevelSpawned()
    {
        transform.position = LevelManager.Instance.PlayerOrigin.position;
        transform.rotation = LevelManager.Instance.PlayerOrigin.rotation;
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.RESET:
                m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
                m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
                Health.SetHealth(m_MaxHealth);
                m_Target = null;
                m_TargetIndicator.gameObject.SetActive(false);
                break;

            case GamePhase.GAME:
                m_TargetIndicator.gameObject.SetActive(true);
                break;
        }
    }

    private void OnGamePhaseEnded(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.GAME:
                m_TargetIndicator.gameObject.SetActive(false);
                break;
        }
    }

    private void OnUnitsUpdated(bool _HasUnits, EnemyController _Enemy)
    {
        m_HasNearestUnit = _HasUnits;

        if(_HasUnits)
        {
            m_NearestUnit = _Enemy;
        }
    }

    private void UpdateMovement()
    {
        m_JoyDir = m_Joystick.GetJoystickDirection();
        var isMoving = m_Joystick.IsTouching() && m_JoyDir != Vector2.zero;

        if (isMoving)
        {
            if(!m_IsMoving)
            {
                StopAttacking();
                StartMoving();
            }

            Move();
        }
        else
        {
            if (m_IsMoving)
            {
                StopMoving();
                StartAttacking();
            }

            Attack();
        }

        m_Graphics.rotation = Quaternion.LookRotation(m_WorldDir, Vector3.up);
    }

    private void StartAttacking()
    {
        if(m_HasNearestUnit)
        {
            m_HasTarget = true;
            m_Target = m_NearestUnit;
        }
            

        m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, true);
    }

    private void Attack()
    {
        if (!m_HasTarget)
            return;

        var dir = m_Target.transform.position - transform.position;
        dir.y = 0;
        dir.Normalize();

        m_WorldDir = dir;
    }

    private void StopAttacking()
    {
        m_Target = null;
        m_HasTarget = false;
        m_Animator.SetBool(Constants.AnimatorValues.c_IsAttacking, false);
    }

    private void StartMoving()
    {
        m_IsMoving = true;
        m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, true);
    }

    private void Move()
    {
        var pos = transform.position;

        m_WorldDir = Vector3.ProjectOnPlane(new Vector3(m_JoyDir.x, 0, m_JoyDir.y), Vector3.up).normalized;
        var dist = m_MoveSpeed * Time.deltaTime;
        var dest = pos + m_WorldDir * dist;

        if (NavMesh.SamplePosition(dest, out m_NavMeshHit, dist, NavMesh.AllAreas))
        {
            transform.position = m_NavMeshHit.position;          
        }
    }

    private void StopMoving()
    {
        m_IsMoving = false;
        m_Animator.SetBool(Constants.AnimatorValues.c_IsMoving, false);
    }
}
